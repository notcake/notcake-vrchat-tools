using System.Collections.Generic;
using System.Linq;
using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>GameObject</c> within a Unity <see cref="PrefabFile"/>.
    /// </summary>
    /// <remarks>
    ///     A <c>GameObject</c> has the following format:
    ///     <code>
    ///         GameObject:
    ///           ...
    ///           m_CorrespondingSourceObject: {fileID: 0}
    ///           m_PrefabInstance: {fileID: 0}
    ///           m_PrefabAsset: {fileID: 0}
    ///           ...
    ///           m_Component:
    ///           - component: {fileID: ...}
    ///           ...
    ///     </code>
    /// </remarks>
    public class GameObject : Object
    {
        private readonly List<Component> knownComponents = new();

        /// <summary>
        ///     Gets the known <see cref="Component">Components</see> of the <c>GameObject</c>.
        /// </summary>
        /// <remarks>
        ///     This is a list of <see cref="Component">Components</see> in the
        ///     <see cref="PrefabFile"/> whose <c>m_GameObject</c> points to this
        ///     <see cref="GameObject"/>.
        ///     <para/>
        ///     <see cref="Component">Components</see> appear in the same order as in the
        ///     <c>.prefab</c> file, which is usually the same order as in <c>m_Component</c>.
        ///     <para/>
        ///     To get the <see cref="Component">Components</see> of a <see cref="GameObject"/> that
        ///     has not been instantiated by a <c>PrefabInstance</c>, <see cref="Components"/> can
        ///     be used instead.
        /// </remarks>
        public IReadOnlyList<Component> KnownComponents => this.knownComponents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameObject"/> class.
        /// </summary>
        /// <inheritdoc cref="Object(PrefabFile, string, bool, YamlDocument)"/>
        internal GameObject(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Adds a <see cref="Component"/> to the known components of the <c>GameObject</c>.
        /// </summary>
        /// <param name="component">
        ///     The <see cref="Component"/> to add to the known components of the <c>GameObject</c>.
        /// </param>
        internal void AddKnownComponent(Component component)
        {
            this.knownComponents.Add(component);
        }

        /// <summary>
        ///     Gets the <c>GameObject</c>'s name.
        /// </summary>
        public string? Name => this.YamlMapping?.TryGetValue<YamlString>("m_Name")?.Value ?? null;

        /// <summary>
        ///     Gets the <c>GameObject</c>'s <c>Component</c>s' <see cref="FileID">FileIDs</see>.
        /// </summary>
        public IReadOnlyList<FileID?>? ComponentFileIDs =>
            this.YamlMapping?
                .TryGetValue<YamlSequence>("m_Component")?
                .Select(x => (x as YamlMapping)?.GetFileIDValue("component"))
                .ToList() ??
            null;

        /// <summary>
        ///     Gets the <c>GameObject</c>'s <see cref="Component">Components</see>.
        /// </summary>
        /// <remarks>
        ///     Does not work for <c>GameObject</c>s instantiated by <c>PrefabInstance</c>s. For
        ///     such <c>GameObject</c>s, <see cref="KnownComponents"/> should be used instead.
        /// </remarks>
        public IReadOnlyList<Component?>? Components => this.ComponentFileIDs?.Select(
            x => x is FileID fileID ? this.Prefab.GetObjectByFileID<Component>(fileID) : null
        ).ToList();

        /// <summary>
        ///     Gets the <c>GameObject</c>'s <see cref="Transform"/>.
        /// </summary>
        public Transform? Transform =>
            this.Components?.FirstOrDefault(x => x is Transform) as Transform;

        /// <summary>
        ///     Gets the <c>GameObject</c>'s parent <see cref="GameObject"/>.
        /// </summary>
        public GameObject? Parent => this.Transform?.Father?.GameObject;
    }
}
