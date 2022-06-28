using System.Collections.Generic;
using System.Linq;
using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>PrefabInstance</c> within a Unity <see cref="PrefabFile"/>.
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
    ///
    ///     A <c>PrefabInstance</c> has the following format:
    ///     <code>
    ///         PrefabInstance:
    ///           ...
    ///           m_Modification:
    ///             m_TransformParent: {fileID: 0 | ...}
    ///             m_Modifications:
    ///             - target: {fileID: $prefabRootGameObjectFileID, guid: $prefabGuid, type: 3}
    ///               propertyPath: m_Name
    ///               value: ...
    ///             - target: {fileID: $prefabRootTransformFileID, guid: $prefabGuid, type: 3}
    ///               propertyPath: m_RootOrder
    ///               value: ...
    ///               ...
    ///             ...
    ///           ...
    ///           m_SourcePrefab: {fileID: 100100000, guid: $prefabGuid, type: 3}
    ///     </code>
    /// </remarks>
    public class PrefabInstance : Object
    {
        private readonly List<Object> knownInstantiatedDescendants = new();

        /// <summary>
        ///     Gets the known instantiated descendants of the <c>PrefabInstance</c>.
        /// </summary>
        public IReadOnlyList<Object> KnownInstantiatedDescendants =>
            this.knownInstantiatedDescendants;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrefabInstance"/> class.
        /// </summary>
        /// <inheritdoc cref="Object(PrefabFile, string, bool, YamlDocument)"/>
        internal PrefabInstance(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Adds a Unity object instance to the known instantiated descendants of the
        ///     <c>PrefabInstance</c>.
        /// </summary>
        /// <param name="object">
        ///     The Unity object instance to add to the known instantiated descendants of the
        ///     <c>PrefabInstance</c>.
        /// </param>
        internal void AddKnownInstantiatedDescendant(Object @object)
        {
            this.knownInstantiatedDescendants.Add(@object);
        }

        /// <summary>
        ///     Gets the <see cref="FileID"/> of the <c>PrefabInstance</c>'s parent
        ///     <c>Transform</c>.
        /// </summary>
        public FileID? TransformParentFileID =>
            this.YamlMapping?
                .TryGetValue<YamlMapping>("m_Modification")?
                .GetFileIDValue("m_TransformParent");

        /// <summary>
        ///     Gets the <c>PrefabInstance</c>'s parent <see cref="Transform"/>.
        /// </summary>
        public Transform? TransformParent => this.TransformParentFileID is FileID fileID ?
            this.Prefab.GetObjectByFileID<Transform>(fileID) :
            null;

        /// <summary>
        ///     Gets the <c>PrefabInstance</c>'s <c>m_RootOrder</c>.
        /// </summary>
        public long? RootOrder =>
            this.YamlMapping?
                .TryGetValue<YamlMapping>("m_Modification")?
                .TryGetValue<YamlSequence>("m_Modifications")?
                .Select(
                    modification =>
                        (modification is YamlMapping yamlMapping &&
                         yamlMapping.TryGetValue<YamlString>("propertyPath")?.Value == "m_RootOrder") ?
                            yamlMapping.TryGetValue<YamlInteger>("value")?.Int64Value :
                            null
                )
                // Descendants of a `PrefabInstance` cannot be reordered, so assume that any
                // `m_RootOrder` modification applies to the root `Transform` of the
                // `PrefabInstance`.
                .FirstOrDefault(rootOrder => rootOrder != null);

        /// <summary>
        ///     Gets the GUID and <see cref="FileID"/> of the <c>PrefabInstance</c>'s source prefab.
        /// </summary>
        public (string, FileID)? SourcePrefabGuidFileID =>
            this.YamlMapping?
                .TryGetValue<YamlMapping>("m_SourcePrefab")?
                .ToGuidFileID();
    }
}
