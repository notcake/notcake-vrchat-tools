using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using notcake.Unity.Prefab;

namespace notcake.Unity.UnityPrefabFileIDSelfRebase
{
    /// <summary>
    ///     Contains the entry point of the application.
    /// </summary>
    public class Program
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
            Argument<FileInfo> sourceFileArgument =
                new("source", "The .prefab file from which to copy fileIDs.");
            Argument<FileInfo> destinationFileArgument =
                new("destination", "The .prefab file to which to copy fileIDs.");
            Option<FileInfo> outputFileOption =
                new("--output", "The path to which to write the new .prefab file.");

            Program.RootCommand = new(
                "Copies the fileIDs from one .prefab file to another. " +
                "Both .prefab files must be well-formed."
            );
            Program.RootCommand.AddArgument(sourceFileArgument);
            Program.RootCommand.AddArgument(destinationFileArgument);
            Program.RootCommand.AddOption(outputFileOption);
            Program.RootCommand.SetHandler(
                invocationContext =>
                {
                    IConsole console = invocationContext.Console;

                    FileInfo sourceFileInfo =
                        invocationContext.ParseResult.GetValueForArgument(sourceFileArgument);
                    FileInfo destinationFileInfo =
                        invocationContext.ParseResult.GetValueForArgument(destinationFileArgument);
                    FileInfo? outputFileInfo =
                        invocationContext.ParseResult.GetValueForOption(outputFileOption);

                    invocationContext.ExitCode = Program.RemapFileIDs(
                        sourceFileInfo,
                        destinationFileInfo,
                        outputFileInfo,
                        console
                    );
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
        ///     Copies the <c>fileID</c>s from the given source prefab file to the given destination
        ///     prefab file and writes the new prefab file to standard output or another file.
        /// </summary>
        /// <param name="sourceFileInfo">
        ///     A <see cref="FileInfo"/> specifying the <c>.prefab</c> file from which to copy
        ///     <c>fileID</c>s.
        /// </param>
        /// <param name="destinationFileInfo">
        ///     A <see cref="FileInfo"/> specifying the <c>.prefab</c> file to which to copy
        ///     <c>fileID</c>s.
        /// </param>
        /// <param name="outputFileInfo">
        ///     A <see cref="FileInfo"/> specifying the output <c>.prefab</c> file.
        ///     <para/>
        ///     When <c>null</c>, the new prefab file is written to standard output.
        /// </param>
        /// <param name="console">
        ///     The <see cref="IConsole"/> to which to write output and errors.
        /// </param>
        /// <returns>
        ///     <c>0</c> on success.<br/>
        ///     <c>1</c> if there was an error.
        /// </returns>
        public static int RemapFileIDs(
            FileInfo sourceFileInfo,
            FileInfo destinationFileInfo,
            FileInfo? outputFileInfo,
            IConsole console
        )
        {
            PrefabFile? sourcePrefabFile =
                Program.LoadPrefabFile(sourceFileInfo, console);
            PrefabFile? destinationPrefabFile =
                Program.LoadPrefabFile(destinationFileInfo, console);

            if (sourcePrefabFile == null || destinationPrefabFile == null)
            {
                return 1;
            }

            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(destinationPrefabFile, sourcePrefabFile);
            List<FileID> sourcePrefabOrdering =
                sourcePrefabFile.Objects.Select(@object => @object.FileID).ToList();

            try
            {
                destinationPrefabFile.RemapFileIDs(
                    prefabFileBijection.ToLeftToRightFileIDMapping(),
                    sourcePrefabOrdering
                );
            }
            catch (InvalidOperationException exception)
            {
                console.Error.Write(exception.Message + Environment.NewLine);
                return 1;
            }

            if (outputFileInfo == null)
            {
                console.Write(destinationPrefabFile.Serialize());
                return 0;
            }
            else
            {
                return Program.SavePrefabFile(outputFileInfo, console, destinationPrefabFile) ?
                    0 :
                    1;
            }
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
        ///     Writes a <see cref="PrefabFile"/> to a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="fileInfo">
        ///     The <see cref="FileInfo"/> to which to write the <see cref="PrefabFile"/>.
        /// </param>
        /// <param name="console">
        ///     The <see cref="IConsole"/> to which to write errors.
        ///     <para/>
        ///     <paramref name="console"/> will only be written to on failure.
        /// </param>
        /// <param name="prefabFile">
        ///     The <see cref="PrefabFile"/> to write to <paramref name="fileInfo"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="PrefabFile"/> was successfully written to
        ///     <paramref name="fileInfo"/>;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private static bool SavePrefabFile(
            FileInfo fileInfo,
            IConsole console,
            PrefabFile prefabFile
        )
        {
            try
            {
                using FileStream fileStream = fileInfo.OpenWrite();
                prefabFile.Serialize(fileStream);
                return true;
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
                        $"Cannot write \"{fileInfo}\" because of filesystem " +
                        "permissions." +
                        Environment.NewLine
                    );
                }
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
                    $"Cannot write \"{fileInfo}\" because it is locked by another " +
                    "process." +
                    Environment.NewLine
                );
            }

            return false;
        }
    }
}
