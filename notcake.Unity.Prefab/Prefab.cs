using notcake.Unity.Yaml;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>Prefab</c> within a Unity <see cref="PrefabFile"/>.
    /// </summary>
    /// <remarks>
    ///     A <c>Prefab</c> has the following format:
    ///     <code>
    ///         Prefab:
    ///           ...
    ///           m_Modification:
    ///             m_TransformParent: {fileID: 0}
    ///             m_Modifications: []
    ///             m_RemovedComponents: []
    ///           m_ParentPrefab: {fileID: 0}
    ///           m_RootGameObject: {fileID: ...}
    ///           m_IsPrefabParent: 1
    ///     </code>
    /// </remarks>
    public class Prefab : Object
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Prefab"/> class.
        /// </summary>
        /// <inheritdoc cref="Object(PrefabFile, string, bool, YamlDocument)"/>
        internal Prefab(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Gets the <see cref="FileID"/> of the <c>Prefab</c>'s root <c>GameObject</c>.
        /// </summary>
        public FileID? RootGameObjectFileID => this.YamlMapping?.GetFileIDValue("m_RootGameObject");

        /// <summary>
        ///     Gets the <c>Prefab</c>'s root <see cref="GameObject"/>.
        /// </summary>
        public GameObject? RootGameObject => this.RootGameObjectFileID is FileID fileID ?
            this.Prefab.GetObjectByFileID<GameObject>(fileID) :
            null;
    }
}
