using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a Unity <c>.prefab</c> file.
    /// </summary>
    /// <remarks>
    ///     Some <c>.prefab</c> files order their Unity objects by <c>fileID</c>, ascending.
    ///     <para/>
    ///     Some <c>.prefab</c> files order their Unity objects as follows:
    ///     <list type="number">
    ///         <item>
    ///             Groups for all <c>GameObject</c>s, including those instantiated by
    ///             <c>PrefabInstance</c>s, ordered by <c>fileID</c>, ascending.
    ///             <list type="number">
    ///                 <item>
    ///                     The <c>GameObject</c>, if not instantiated by a <c>PrefabInstance</c>.
    ///                 </item>
    ///                 <item>
    ///                     The <c>GameObject</c>'s components in <c>m_Component</c> order.
    ///                     <para/>
    ///                     If the <c>GameObject</c> has been instantiated by a
    ///                     <c>PrefabInstance</c>, the <c>m_Component</c> order is usually, but not
    ///                     always, ascending <c>fileID</c> order.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///         <item>
    ///             Groups for <c>PrefabInstance</c>s, ordered by <c>fileID</c>, ascending.
    ///             <list type="number">
    ///                 <item>The <c>PrefabInstance</c>.</item>
    ///                 <item>
    ///                     The <c>PrefabInstance</c>'s instantiated objects, ordered in an unknown
    ///                     way.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    ///     Other <c>.prefab</c> files order their Unity objects as follows:
    ///     <list type="number">
    ///         <item>
    ///             Groups for non-<c>PrefabInstance</c> instantiated <c>GameObject</c>s and
    ///             <c>PrefabInstance</c>s, ordered by <c>fileID</c>, ascending.
    ///             <list type="number">
    ///                 <item>
    ///                     The <c>GameObject</c>, if the group is for a <c>GameObject</c>.
    ///                 </item>
    ///                 <item>
    ///                     The <c>GameObject</c>'s components in <c>m_Component</c> order, if the
    ///                     group is for a <c>GameObject</c>.
    ///                 </item>
    ///                 <item>
    ///                     Groups for the <c>PrefabInstance</c>'s <c>GameObject</c> instances,
    ///                     ordered in an unknown way, if the group is for a <c>PrefabInstance</c>.
    ///                     <list type="number">
    ///                         <item>
    ///                             The <c>GameObject</c> instance's components in
    ///                             <c>m_Component</c> order.
    ///                             <para/>
    ///                             The <c>m_Component</c> order is usually, but not always,
    ///                             ascending <c>fileID</c> order.
    ///                         </item>
    ///                     </list>
    ///                 </item>
    ///             </list>
    ///         </item>
    ///         <item>
    ///             Groups for <c>PrefabInstance</c>s, ordered by <c>fileID</c>, ascending.
    ///             <list type="number">
    ///                 <item>The <c>PrefabInstance</c>.</item>
    ///                 <item>
    ///                     The <c>PrefabInstance</c>'s object instances, ordered in an unknown
    ///                     way.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </remarks>
    public partial class PrefabFile
    {
        /// <summary>
        ///     Gets the directive lines at the start of the Unity <c>.prefab</c> file, including
        ///     their line endings.
        /// </summary>
        public IList<string> Directives { get; }

        /// <summary>
        ///     Gets or sets a boolean indicating whether the Unity <c>.prefab</c> file ends with a
        ///     line break.
        /// </summary>
        public bool TrailingLineBreak { get; set; }

        private Dictionary<FileID, Object> objectsByFileID = new();
        private Dictionary<Object, FileID> objectFileIDs = new();
        private readonly List<Object> objects = new();

        /// <summary>
        ///     Gets the Unity objects in the prefab.
        /// </summary>
        public IReadOnlyList<Object> Objects => this.objects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrefabFile"/> class.
        /// </summary>
        /// <param name="string">The string from which to deserialize the Unity prefab.</param>
        /// <exception cref="InvalidDataException">
        ///     Thrown when the YAML stream is malformed.
        /// </exception>
        private PrefabFile(string @string)
        {
            const string lineBreak = "(?:\r\n|\r|\n|\u0085|\u2028|\u2029|$)";
            Match match = Regex.Match(
                @string,
                "^" +
                $"(.*?{lineBreak})*?" +
                "(?:" +
                    $"--- (.*?){lineBreak}" +
                    $"((?:.*?{lineBreak})*?)" +
                ")*" +
                "$"
            );

            this.Directives = match.Groups[1].Captures.Select(x => x.Value).ToList();

            bool trailingLineBreak = false;

            for (int i = 0; i < match.Groups[2].Captures.Count; i++)
            {
                string tagAnchorStripped = match.Groups[2].Captures[i].Value;
                Match tagAnchorStrippedMatch =
                    Regex.Match(tagAnchorStripped, "^(.*?) &(-?[0-9]+)( stripped)?$");
                if (!tagAnchorStrippedMatch.Success)
                {
                    throw new InvalidDataException($"Invalid tag: {tagAnchorStripped}");
                }

                string tag = tagAnchorStrippedMatch.Groups[1].Value;
                FileID fileID = new(long.Parse(tagAnchorStrippedMatch.Groups[2].Value));
                bool stripped = tagAnchorStrippedMatch.Groups[3].Success;

                YamlDocument document = YamlDocument.Deserialize(match.Groups[3].Captures[i].Value);
                trailingLineBreak = document.TrailingLineBreak;
                document.TrailingLineBreak = true;
                Object @object = tag switch
                {
                    "!u!1"    => new GameObject(this, tag, stripped, document),
                    "!u!4"    => new Transform(this, tag, stripped, document),
                    "!u!114"  => new MonoBehaviour(this, tag, stripped, document),
                    "!u!1001" => fileID == new FileID(100100000) ?
                                     new Prefab(this, tag, stripped, document) :
                                     new PrefabInstance(this, tag, stripped, document),
                    _         => (stripped ||
                                  document.RootNode.GetUnityObjectMapping()?.ContainsKey("m_GameObject") != null) ?
                                     new Component(this, tag, stripped, document) :
                                     new Object(this, tag, stripped, document)
                };
                this.objectFileIDs[@object] = fileID;
                this.objectsByFileID[fileID] = @object;
                this.objects.Add(@object);

                if (@object is PrefabInstance prefabInstance)
                {
                    this.prefabInstances.Add(prefabInstance);
                }
            }

            // Build lists of `PrefabInstance`-instantiated descendants.
            foreach (Object @object in this.objects)
            {
                if (@object.IsInstance)
                {
                    @object.PrefabInstance?.AddKnownInstantiatedDescendant(@object);
                }
                else if (@object is Transform transform && !@object.IsInstance)
                {
                    transform.Father?.AddKnownChild(transform);
                    transform.GameObject?.AddKnownComponent(transform);
                }
                else if (@object is Component component)
                {
                    component.GameObject?.AddKnownComponent(component);
                }
                else if (@object is PrefabInstance prefabInstance)
                {
                    prefabInstance?.TransformParent?.AddKnownChild(prefabInstance);
                }
            }

            this.otherObjects = this.ComputeOtherObjects();
            this.otherObjectsSet = new HashSet<Object>(this.otherObjects);

            (
                this.rootTransforms,
                this.rootGameObjects,
                this.rootPrefabInstances
            ) = this.ComputeRoots();

            this.orphanObjects = this.ComputeOrphanObjects(this.otherObjects);

            this.TrailingLineBreak = trailingLineBreak;
        }

        /// <summary>
        ///     Determines whether the prefab file contains the given <see cref="Object"/>.
        /// </summary>
        /// <param name="object">
        ///     The <see cref="Object"/> whose presence in the prefab file is to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="object"/> is in the prefab;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Contains(Object @object)
        {
            return this.objectFileIDs.ContainsKey(@object);
        }

        /// <summary>
        ///     Determines whether the prefab file contains a Unity object with the given
        ///     <see cref="FileID"/>.
        /// </summary>
        /// <param name="fileID">
        ///     The <see cref="FileID"/> whose presence in the prefab file is to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if a Unity object with the given <see cref="FileID"/> is in the
        ///     prefab;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Contains(FileID fileID)
        {
            return this.objectsByFileID.ContainsKey(fileID);
        }

        /// <summary>
        ///     Gets the <see cref="Object"/> with the given <see cref="FileID"/>.
        /// </summary>
        /// <param name="fileID">
        ///     The <see cref="FileID"/> of the <see cref="Object"/> to retrieve.
        /// </param>
        /// <returns>
        ///     The <see cref="Object"/> with the given <see cref="FileID"/>, if it exists;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public Object? GetObjectByFileID(FileID fileID)
        {
            return this.objectsByFileID.TryGetValue(fileID, out Object? @object) ? @object : null;
        }

        /// <summary>
        ///     Gets the <see cref="Object"/> with the given <see cref="FileID"/> and type.
        /// </summary>
        /// <typeparam name="ObjectT">The type of the <see cref="Object"/> to retrieve.</typeparam>
        /// <returns>
        ///     The <see cref="Object"/> with the given <see cref="FileID"/> and type, if it
        ///     exists;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        /// <inheritdoc cref="GetObjectByFileID(FileID)"/>
        public ObjectT? GetObjectByFileID<ObjectT>(FileID fileID)
            where ObjectT : Object
        {
            return this.GetObjectByFileID(fileID) as ObjectT;
        }

        /// <summary>
        ///     Gets the <see cref="FileID"/> of the given <see cref="Object"/>.
        /// </summary>
        /// <param name="object">
        ///     The <see cref="Object"/> whose <see cref="FileID"/> is to be retrieved.
        /// </param>
        /// <returns>
        ///     The <see cref="FileID"/> of <paramref name="object"/>, if it is contained in the
        ///     <see cref="PrefabFile"/>;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public FileID? GetObjectFileID(Object @object)
        {
            return this.objectFileIDs.TryGetValue(@object, out FileID fileID) ? fileID : null;
        }

        /// <summary>
        ///     Serializes the Unity prefab file into a string.
        /// </summary>
        /// <returns>The serialized Unity prefab.</returns>
        public string Serialize()
        {
            using StringWriter stringWriter = new();
            this.Serialize(stringWriter);
            return stringWriter.ToString();
        }

        /// <summary>
        ///     Serializes the Unity prefab file into a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="Stream"/> into which to serialize the Unity prefab.
        /// </param>
        /// <inheritdoc cref="Serialize(TextWriter)"/>
        public void Serialize(Stream stream)
        {
            using StreamWriter streamWriter = new(stream, leaveOpen: true);
            this.Serialize(streamWriter);
        }

        /// <summary>
        ///     Serializes the Unity prefab file into a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">
        ///     The <see cref="TextWriter"/> into which to serialize the YAML document.
        /// </param>
        public void Serialize(TextWriter textWriter)
        {
            foreach (string directive in this.Directives)
            {
                textWriter.Write(directive);
            }

            foreach (Object @object in this.objects)
            {
                bool isLast = @object == this.objects[^1];
                @object.Document.TrailingLineBreak = !isLast || this.TrailingLineBreak;

                textWriter.Write($"--- {@object.Tag} &{@object.FileID}");
                if (@object.IsInstance)
                {
                    textWriter.Write(" stripped");
                }
                textWriter.Write(@object.Document.LineBreakStyle.GetString());
                @object.Document.Serialize(textWriter);
            }
        }

        /// <summary>
        ///     Deserializes a Unity prefab file from a string.
        /// </summary>
        /// <returns>The deserialized Unity <see cref="PrefabFile"/>.</returns>
        /// <inheritdoc cref="PrefabFile(string)"/>
        public static PrefabFile Deserialize(string @string)
        {
            return new(@string);
        }

        /// <summary>
        ///     Deserializes a Unity prefab file from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="Stream"/> from which to deserialize the Unity prefab.
        /// </param>
        /// <inheritdoc cref="Deserialize(TextReader)"/>
        public static PrefabFile Deserialize(Stream stream)
        {
            using StreamReader streamReader = new(stream, leaveOpen: true);
            return PrefabFile.Deserialize(streamReader);
        }

        /// <summary>
        ///     Deserializes a Unity prefab file from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">
        ///     The <see cref="TextReader"/> from which to deserialize the Unity prefab.
        /// </param>
        /// <inheritdoc cref="Deserialize(string)"/>
        public static PrefabFile Deserialize(TextReader textReader)
        {
            string @string = textReader.ReadToEnd();
            return PrefabFile.Deserialize(@string);
        }
    }
}
