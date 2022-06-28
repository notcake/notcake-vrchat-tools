using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using notcake.Algorithms;
using notcake.Unity.Prefab;
using Object = notcake.Unity.Prefab.Object;

namespace notcake.Unity.UnityPrefabFileIDDiff
{
    /// <summary>
    ///     Contains the entry point of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Gets the <see cref="System.CommandLine.RootCommand"/> of the application.
        /// </summary>
        public static RootCommand RootCommand { get; }

        /// <summary>
        ///     Initializes the static data of the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
            Argument<FileInfo> leftFileArgument =
                new("left", "The first .prefab file to diff.");
            Argument<FileInfo> rightFileArgument =
                new("right", "The second .prefab file to diff.");

            Program.RootCommand = new(
                "Diffs the fileIDs between two .prefab files. " +
                "Both .prefab files must be well-formed."
            );
            Program.RootCommand.AddArgument(leftFileArgument);
            Program.RootCommand.AddArgument(rightFileArgument);
            Program.RootCommand.SetHandler(
                invocationContext =>
                {
                    IConsole console = invocationContext.Console;

                    FileInfo leftFileInfo =
                        invocationContext.ParseResult.GetValueForArgument(leftFileArgument);
                    FileInfo rightFileInfo =
                        invocationContext.ParseResult.GetValueForArgument(rightFileArgument);

                    invocationContext.ExitCode =
                        Program.PrintFileIDDiff(leftFileInfo, rightFileInfo, console);
                }
            );
        }

        /// <summary>
        ///     The entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments for the application.</param>
        public static void Main(string[] args)
        {
            Program.RootCommand.Invoke(args);
        }

        /// <summary>
        ///     Prints the <c>fileID</c> diff between two <c>.prefab</c> files.
        /// </summary>
        /// <param name="leftFileInfo">
        ///     A <see cref="FileInfo"/> specifying the left <c>.prefab</c> file.
        /// </param>
        /// <param name="rightFileInfo">
        ///     A <see cref="FileInfo"/> specifying the right <c>.prefab</c> file.
        /// </param>
        /// <param name="console">
        ///     The <see cref="IConsole"/> to which to write output and errors.
        /// </param>
        /// <returns>
        ///     <c>0</c> if the <c>.prefab</c> files have the same <c>fileID</c>s.<br/>
        ///     <c>1</c> if the <c>.prefab</c> files have different <c>fileID</c>s or different
        ///     objects.<br/>
        ///     <c>2</c> if there was an error.
        /// </returns>
        public static int PrintFileIDDiff(
            FileInfo leftFileInfo,
            FileInfo rightFileInfo,
            IConsole console
        )
        {
            PrefabFile? leftPrefabFile  = Program.LoadPrefabFile(leftFileInfo, console);
            PrefabFile? rightPrefabFile = Program.LoadPrefabFile(rightFileInfo, console);

            if (leftPrefabFile == null || rightPrefabFile == null)
            {
                return 2;
            }

            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(leftPrefabFile, rightPrefabFile);

            // Print other objects.
            foreach (
                (Object? leftObject, Object? rightObject)
                in Program.EnumerateCommonLeftRight(
                    prefabFileBijection,
                    leftPrefabFile.OtherObjects,
                    rightPrefabFile.OtherObjects
                )
            )
            {
                Program.PrintMappingLine(console, 0, leftObject, rightObject);
            }

            // Print orphan objects.
            foreach (
                (Object? leftObject, Object? rightObject)
                in Program.EnumerateCommonLeftRight(
                    prefabFileBijection,
                    leftPrefabFile.OrphanObjects,
                    rightPrefabFile.OrphanObjects
                )
            )
            {
                Program.PrintMappingLine(console, 0, leftObject, rightObject);
            }

            // Print the `GameObject` tree.
            foreach (
                (Object? leftObject, Object? rightObject)
                in Program.EnumerateCommonLeftRight(
                    prefabFileBijection,
                    leftPrefabFile.RootGameObjects
                        .Cast<Object?>()
                        .Concat(leftPrefabFile.RootPrefabInstances),
                    rightPrefabFile.RootGameObjects
                        .Cast<Object?>()
                        .Concat(rightPrefabFile.RootPrefabInstances)
                )
            )
            {
                int indentation = 0;
                DepthFirstSearch.Enumerate(
                    (leftObject, rightObject),
                    ((Object? LeftObject, Object? RightObject) mapping) =>
                    {
                        GameObject? leftGameObject  = mapping.LeftObject  as GameObject;
                        GameObject? rightGameObject = mapping.RightObject as GameObject;
                        Transform? leftTransform  = mapping.LeftObject  as Transform;
                        Transform? rightTransform = mapping.RightObject as Transform;
                        switch (leftGameObject, rightGameObject)
                        {
                            case (GameObject, GameObject):
                            case (GameObject, null):
                            case (null, GameObject):
                                leftTransform  = leftGameObject?.Transform;
                                rightTransform = rightGameObject?.Transform;
                                break;
                            default:
                                break;
                        }

                        // Return the children of `GameObject`s or `Transform`s.
                        switch (leftTransform, rightTransform)
                        {
                            case (Transform, Transform):
                            case (Transform, null):
                            case (null, Transform):
                                IEnumerable<Object>? leftChildren =
                                    leftTransform?.KnownChildrenInRootOrder.Select(
                                        transformOrPrefabInstance =>
                                            transformOrPrefabInstance.Match(
                                                transform =>
                                                    transform.GameObject as Object ??
                                                    transform,
                                                prefabInstance => prefabInstance
                                            )
                                    );
                                IEnumerable<Object>? rightChildren =
                                    rightTransform?.KnownChildrenInRootOrder.Select(
                                        transformOrPrefabInstance =>
                                            transformOrPrefabInstance.Match(
                                                transform =>
                                                    transform.GameObject as Object ??
                                                    transform,
                                                prefabInstance => prefabInstance
                                            )
                                    );
                                return Program.EnumerateCommonLeftRight(
                                    prefabFileBijection,
                                    leftChildren,
                                    rightChildren
                                );
                            default:
                                break;
                        }

                        // Return the children of `PrefabInstance`s.
                        PrefabInstance? leftPrefabInstance =
                            mapping.LeftObject as PrefabInstance;
                        PrefabInstance? rightPrefabInstance =
                            mapping.RightObject as PrefabInstance;

                        switch (leftPrefabInstance, rightPrefabInstance)
                        {
                            case (PrefabInstance, PrefabInstance):
                            case (PrefabInstance, null):
                            case (null, PrefabInstance):
                                return Program.EnumerateCommonLeftRight(
                                    prefabFileBijection,
                                    leftPrefabInstance?.KnownInstantiatedDescendants,
                                    rightPrefabInstance?.KnownInstantiatedDescendants
                                );
                            default:
                                break;
                        }

                        return Enumerable.Empty<(Object?, Object?)>();
                    },
                    ((Object? LeftObject, Object? RightObject) mapping) =>
                    {
                        Program.PrintMappingLine(
                            console,
                            indentation,
                            mapping.LeftObject,
                            mapping.RightObject
                        );
                        indentation += 2;

                        // Handle `GameObject` `Component`s
                        GameObject? leftGameObject  = mapping.LeftObject  as GameObject;
                        GameObject? rightGameObject = mapping.RightObject as GameObject;
                        switch (leftGameObject, rightGameObject)
                        {
                            case (GameObject, GameObject):
                            case (GameObject, null):
                            case (null, GameObject):
                                IReadOnlyList<Component?>? leftComponents =
                                    (leftGameObject?.IsInstance ?? false) ?
                                        leftGameObject?.KnownComponents :
                                        leftGameObject?.Components;
                                IReadOnlyList<Component?>? rightComponents =
                                    (rightGameObject?.IsInstance ?? false) ?
                                        rightGameObject?.KnownComponents :
                                        rightGameObject?.Components;
                                foreach (
                                    (Component? leftComponent, Component? rightComponent)
                                    in Program.EnumerateCommonLeftRight(
                                        prefabFileBijection,
                                        leftComponents,
                                        rightComponents
                                    )
                                )
                                {
                                    Program.PrintMappingLine(
                                        console,
                                        indentation,
                                        leftComponent,
                                        rightComponent
                                    );
                                }
                                break;
                        }
                    },
                    ((Object? LeftObject, Object? RightObject) mapping) =>
                    {
                        indentation -= 2;
                    }
                );
            }

            bool fileIDsSame = prefabFileBijection.Count == leftPrefabFile.Objects.Count &&
                               prefabFileBijection.Count == rightPrefabFile.Objects.Count;
            if (fileIDsSame)
            {
                foreach (Object leftObject in leftPrefabFile.Objects)
                {
                    Object? rightObject = prefabFileBijection.MapLeftToRight(leftObject);
                    if (leftObject.FileID != rightObject?.FileID)
                    {
                        fileIDsSame = false;
                        break;
                    }
                }
            }

            return fileIDsSame ? 0 : 1;
        }

        /// <summary>
        ///     Reads a <see cref="PrefabFile"/> from a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="fileInfo">
        ///     The <see cref="FileInfo"/> from which to read the <see cref="PrefabFile"/>.
        /// </param>
        /// <param name="console">
        ///     The <see cref="IConsole"/> to which to write errors.
        ///     <para/>
        ///     <paramref name="console"/> will only be written to on failure.
        /// </param>
        /// <returns>
        ///     The <see cref="PrefabFile"/> read from <paramref name="fileInfo"/>, if
        ///     successful;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        private static PrefabFile? LoadPrefabFile(FileInfo fileInfo, IConsole console)
        {
            try
            {
                using FileStream fileStream = fileInfo.OpenRead();
                return PrefabFile.Deserialize(fileStream);
            }
            catch (UnauthorizedAccessException)
            {
                if (Directory.Exists(fileInfo.FullName))
                {
                    console.Error.Write(
                        $"Expected \"{fileInfo}\" to be a prefab file, " +
                        "but found a directory." +
                        Environment.NewLine
                    );
                }
                else
                {
                    console.Error.Write(
                        $"Cannot read \"{fileInfo}\" because of filesystem " +
                        "permissions." +
                        Environment.NewLine
                    );
                }
            }
            catch (FileNotFoundException)
            {
                console.Error.Write(
                    $"Prefab file \"{fileInfo}\" does not exist." +
                    Environment.NewLine
                );
            }
            catch (DirectoryNotFoundException)
            {
                console.Error.Write(
                    $"The directory containing \"{fileInfo}\" does not exist." +
                    Environment.NewLine
                );
            }
            catch (IOException)
            {
                console.Error.Write(
                    $"Cannot read \"{fileInfo}\" because it is locked by another " +
                    "process." +
                    Environment.NewLine
                );
            }
            catch (InvalidDataException invalidDataException)
            {
                console.Error.Write(
                    $"Could not parse \"{fileInfo}\" because it is malformed: " +
                    invalidDataException.Message + Environment.NewLine
                );
            }

            return null;
        }

        /// <summary>
        ///     Enumerates the mapping between the given left objects and right objects.
        /// </summary>
        /// <typeparam name="ObjectT">The type of <see cref="Object"/>.</typeparam>
        /// <param name="prefabFileBijection">
        ///     The mapping between <paramref name="leftObjects"/> and
        ///     <paramref name="rightObjects"/>.
        /// </param>
        /// <param name="leftObjects">The left objects whose mappings are to be enumerated.</param>
        /// <param name="rightObjects">
        ///     The right objects whose mappings are to be enumerated.
        /// </param>
        /// <returns>
        ///     An enumerable containing pairs of left objects and corresponding right objects,
        ///     followed by left objects with no corresponding right object, followed by right
        ///     objects with no corresponding left object.
        /// </returns>
        private static IEnumerable<(ObjectT?, ObjectT?)> EnumerateCommonLeftRight<ObjectT>(
            PrefabFileBijection prefabFileBijection,
            IEnumerable<ObjectT?>? leftObjects,
            IEnumerable<ObjectT?>? rightObjects
        )
            where ObjectT : Object
        {
            if (leftObjects != null)
            {
                foreach (ObjectT? leftObject in leftObjects)
                {
                    if (leftObject == null) { continue; }
                    if (prefabFileBijection.MapLeftToRight(leftObject) is ObjectT rightObject)
                    {
                        // `rightObject` may not be in `rightObjects`.
                        // This is fine because  call to `EnumerateCommonLeftRight` with a
                        // `rightObjects` containing `rightObject` will not yield
                        // `(leftObject, rightObject)` again.
                        yield return (leftObject, rightObject);
                    }
                }
                foreach (ObjectT? leftObject in leftObjects)
                {
                    if (leftObject == null) { continue; }
                    if (prefabFileBijection.ContainsLeft(leftObject)) { continue; }

                    yield return (leftObject, null);
                }
            }
            if (rightObjects != null)
            {
                foreach (ObjectT? rightObject in rightObjects)
                {
                    if (rightObject == null) { continue; }
                    if (prefabFileBijection.ContainsRight(rightObject)) { continue; }

                    yield return (null, rightObject);
                }
            }
        }

        /// <summary>
        ///     Prints the <c>fileID</c> mapping between two <see cref="Object">Objects</see>.
        /// </summary>
        /// <param name="console">The <see cref="IConsole"/> to which to write the mapping.</param>
        /// <param name="indentationLevel">
        ///     The indentation level with which to write the mapping, in columns.
        /// </param>
        /// <param name="leftObject">The left <see cref="Object"/> of the mapping.</param>
        /// <param name="rightObject">The right <see cref="Object"/> of the mapping.</param>
        private static void PrintMappingLine(
            IConsole console,
            int indentationLevel,
            Object? leftObject,
            Object? rightObject
        )
        {
            string indentation = new(' ', indentationLevel);
            string? leftName  = (leftObject  as GameObject)?.Name ?? leftObject?.Type;
            string? rightName = (rightObject as GameObject)?.Name ?? rightObject?.Type;
            switch (leftObject, rightObject)
            {
                case (Object, Object):
                    if (leftObject.FileID     == rightObject.FileID &&
                        leftObject.Type       == rightObject.Type   &&
                        leftName              == rightName          &&
                        leftObject.IsInstance == rightObject.IsInstance)
                    {
                        console.WriteLine(
                            $"{indentation}* " +
                            $"{leftName} &{leftObject.FileID}" +
                            (leftObject.IsInstance ? " stripped" : "")
                        );
                    }
                    else if (leftObject.Type       == rightObject.Type &&
                             leftName              == rightName        &&
                             leftObject.IsInstance == rightObject.IsInstance)
                    {
                        console.WriteLine(
                            $"{indentation}* " +
                            $"{leftName} &{leftObject.FileID} -> " +
                            $"&{rightObject.FileID}" +
                            (rightObject.IsInstance ? " stripped" : "")
                        );
                    }
                    else
                    {
                        console.WriteLine(
                            $"{indentation}* " +
                            $"{leftName} &{leftObject.FileID}" +
                            (leftObject.IsInstance ? " stripped" : "") +
                            " -> " +
                            $"{rightName} &{rightObject.FileID}" +
                            (rightObject.IsInstance ? " stripped" : "")
                        );
                    }
                    break;
                case (Object, null):
                    console.WriteLine(
                        $"{indentation}< " +
                        $"{leftName} &{leftObject.FileID}" +
                        (leftObject.IsInstance ? " stripped" : "")
                    );
                    break;
                case (null, Object):
                    console.WriteLine(
                        $"{indentation}> " +
                        $"{rightName} &{rightObject.FileID}" +
                        (rightObject.IsInstance ? " stripped" : "")
                    );
                    break;
            }
        }
    }
}
