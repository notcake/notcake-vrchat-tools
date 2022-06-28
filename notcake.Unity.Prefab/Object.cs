using System;
using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a Unity object within a <see cref="Prefab"/>.
    /// </summary>
    /// <remarks>
    ///     May not may not be a Unity object instantiated by a <c>PrefabInstance</c>.
    ///     <para/>
    ///     A Unity object instantiated by a <c>PrefabInstance</c> has the following format:
    ///     <code>
    ///         Transform:
    ///           m_CorrespondingSourceObject: {fileID: ..., guid: $prefabGuid, type: 3}
    ///           m_PrefabInstance: {fileID: ...}
    ///           m_PrefabAsset: {fileID: 0}
    ///     </code>
    /// </remarks>
    public class Object
    {
        /// <summary>
        ///     Gets the <see cref="Prefab"/> containing this Unity object.
        /// </summary>
        public PrefabFile Prefab { get; }

        /// <summary>
        ///     Gets or sets the YAML tag for this Unity object.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        ///     Gets a boolean indicating whether this Unity object was instantiated by a
        ///     <c>PrefabInstance</c>.
        /// </summary>
        public bool IsInstance { get; }

        /// <summary>
        ///     Gets the <see cref="YamlDocument"/> for the Unity object.
        /// </summary>
        public YamlDocument Document { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Object"/> class.
        /// </summary>
        /// <param name="prefabFile">The <see cref="Prefab"/> containing this Unity object.</param>
        /// <param name="tag">The YAML tag for this Unity object.</param>
        /// <param name="isInstance">
        ///     A boolean indicating whether this Unity object was instantiated by a
        ///     <c>PrefabInstance</c>.
        /// </param>
        /// <param name="document">
        ///     The <see cref="YamlDocument"/> for the Unity object.
        /// </param>
        internal Object(PrefabFile prefabFile, string tag, bool isInstance, YamlDocument document)
        {
            this.Prefab = prefabFile;
            this.Tag = tag;
            this.IsInstance = isInstance;
            this.Document = document;
        }

        #region Object
        public override string ToString()
        {
            return $"{this.Tag} &{this.FileID}" +
                   (this.IsInstance ? " stripped" : "") +
                   $" {this.Type ?? "<malformed>"}";
        }
        #endregion

        #region Object
        /// <summary>
        ///     Gets the <see cref="FileID"/> of the Unity object.
        /// </summary>
        public FileID FileID => this.Prefab.GetObjectFileID(this) ??
                                throw new InvalidOperationException();

        /// <summary>
        ///     Gets the root <see cref="Yaml.Nodes.YamlMapping"/> for the Unity object.
        /// </summary>
        public YamlMapping? YamlMapping => this.Document.RootNode.GetUnityObjectMapping();

        /// <summary>
        ///     Gets the type of the Unity object.
        /// </summary>
        /// <remarks>
        ///     eg. <c>GameObject</c>, <c>Transform</c>, <c>PrefabInstance</c>, etc.
        /// </remarks>
        public string? Type
        {
            get
            {
                if (this.Document.RootNode is not YamlMapping rootNode) { return null; }
                if (rootNode.Count != 1) { return null; }

                return (rootNode.Keys[0] as YamlString)?.Value;
            }
        }

        /// <summary>
        ///     Gets the <see cref="notcake.Unity.Prefab.FileID"/> of the template object from which
        ///     the Unity object was instantiated.
        /// </summary>
        public FileID? CorrespondingSourceObjectFileID =>
            this.YamlMapping?.GetFileIDValue("m_CorrespondingSourceObject");

        /// <summary>
        ///     Gets the <see cref="notcake.Unity.Prefab.FileID"/> of the <c>PrefabInstance</c> that
        ///     instantiated this Unity object.
        /// </summary>
        public FileID? PrefabInstanceFileID => this.YamlMapping?.GetFileIDValue("m_PrefabInstance");

        /// <summary>
        ///     Gets the <see cref="notcake.Unity.Prefab.PrefabInstance"/> that instantiated this
        ///     Unity object.
        /// </summary>
        public PrefabInstance? PrefabInstance => this.PrefabInstanceFileID is FileID fileID ?
            this.Prefab.GetObjectByFileID<PrefabInstance>(fileID) :
            null;
        #endregion
    }
}
