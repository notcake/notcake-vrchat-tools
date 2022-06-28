using notcake.Unity.Yaml;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>Component</c> within a Unity <see cref="PrefabFile"/>.
    /// </summary>
    /// <remarks>
    ///     A <c>Component</c> has the following format:
    ///     <code>
    ///         RotationConstraint:
    ///           ...
    ///           m_CorrespondingSourceObject: {fileID: 0}
    ///           m_PrefabInstance: {fileID: 0}
    ///           m_PrefabAsset: {fileID: 0}
    ///           m_GameObject: {fileID: ...}
    ///           ...
    ///     </code>
    /// </remarks>
    public class Component : Object
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <inheritdoc cref="Object(PrefabFile, string, bool, YamlDocument)"/>
        internal Component(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Gets the <see cref="FileID"/> of the <c>Component</c>'s parent <c>GameObject</c>.
        /// </summary>
        public FileID? GameObjectFileID => this.YamlMapping?.GetFileIDValue("m_GameObject");

        /// <summary>
        ///     Gets the <c>Component</c>'s parent <see cref="GameObject"/>.
        /// </summary>
        public GameObject? GameObject => this.GameObjectFileID is FileID fileID ?
            this.Prefab.GetObjectByFileID<GameObject>(fileID) :
            null;
    }
}
