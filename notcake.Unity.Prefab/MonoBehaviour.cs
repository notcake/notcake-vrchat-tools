using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>MonoBehaviour</c> within a Unity <see cref="PrefabFile"/>.
    /// </summary>
    /// <remarks>
    ///     A <c>MonoBehaviour</c> has the following format:
    ///     <code>
    ///         MonoBehaviour:
    ///           ...
    ///           m_CorrespondingSourceObject: {fileID: 0}
    ///           m_PrefabInstance: {fileID: 0}
    ///           m_PrefabAsset: {fileID: 0}
    ///           m_GameObject: {fileID: ...}
    ///           ...
    ///           m_Script: {fileID: ..., guid: ..., type: 3}
    ///           ...
    ///     </code>
    /// </remarks>
    public class MonoBehaviour : Component
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MonoBehaviour"/> class.
        /// </summary>
        /// <inheritdoc cref="Component(PrefabFile, string, bool, YamlDocument)"/>
        internal MonoBehaviour(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Gets the GUID and <see cref="FileID"/> of the <c>MonoBehaviour</c>'s script.
        /// </summary>
        public (string, FileID)? ScriptGuidFileID =>
            this.YamlMapping?
                .TryGetValue<YamlMapping>("m_Script")?
                .ToGuidFileID();
    }
}
