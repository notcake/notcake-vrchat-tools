using System;
using System.Collections.Generic;
using System.Linq;
using notcake.Functional;
using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a <c>Transform</c> within a Unity <see cref="PrefabFile"/>.
    /// </summary>
    /// <remarks>
    ///     A <c>Transform</c> has the following format:
    ///     <code>
    ///         Transform:
    ///           ...
    ///           m_CorrespondingSourceObject: {fileID: 0}
    ///           m_PrefabInstance: {fileID: 0}
    ///           m_PrefabAsset: {fileID: 0}
    ///           m_GameObject: {fileID: ...}
    ///           ...
    ///           m_Children:
    ///           - {fileID: ...}
    ///           ...
    ///           m_Father: {fileID: 0 | ...}
    ///           m_RootOrder: ...
    ///           ...
    ///     </code>
    /// </remarks>
    public class Transform : Component
    {
        private readonly List<Either<Transform, PrefabInstance>> knownChildrenInPrefabFileOrder =
            new();
        private readonly List<Either<Transform, PrefabInstance>> knownChildrenInRootOrder = new();
        private bool knownChildrenInRootOrderNeedsSorting = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Transform"/> class.
        /// </summary>
        /// <inheritdoc cref="Object(PrefabFile, string, bool, YamlDocument)"/>
        internal Transform(
            PrefabFile prefabFile,
            string tag,
            bool isInstance,
            YamlDocument document
        ) :
            base(prefabFile, tag, isInstance, document)
        {
        }

        /// <summary>
        ///     Gets the known child <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> of the <c>Transform</c>.
        /// </summary>
        /// <remarks>
        ///     This is a list of <see cref="Transform">Transforms</see> in the
        ///     <see cref="PrefabFile"/> whose <c>m_Father</c> points to this
        ///     <see cref="Transform"/> and <see cref="PrefabInstance">PrefabInstances</see> whose
        ///     <c>m_TransformParent</c> points to this <see cref="Transform"/>.
        ///     <para/>
        ///     <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> appear in the same order as in the
        ///     <c>.prefab</c> file, which is not necessarily the same order as given by their
        ///     <c>m_RootOrder</c>s. To get child <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> in the order given by their
        ///     <c>m_RootOrder</c>s, use <see cref="KnownChildrenInRootOrder"/> instead.
        ///     <para/>
        ///     To get the child <see cref="Transform">Transforms</see> of a <see cref="Transform"/>
        ///     that has not been instantiated by a <c>PrefabInstance</c>, <see cref="Children"/>
        ///     can be used instead.
        /// </remarks>
        public IReadOnlyList<Either<Transform, PrefabInstance>> KnownChildrenInPrefabFileOrder =>
            this.knownChildrenInPrefabFileOrder;

        /// <summary>
        ///     Gets the known child <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> of the <c>Transform</c>.
        /// </summary>
        /// <remarks>
        ///     This is a list of <see cref="Transform">Transforms</see> in the
        ///     <see cref="PrefabFile"/> whose <c>m_Father</c> points to this
        ///     <see cref="Transform"/> and <see cref="PrefabInstance">PrefabInstances</see> whose
        ///     <c>m_TransformParent</c> points to this <see cref="Transform"/>.
        ///     <para/>
        ///     <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> appear in the order given by their
        ///     <c>m_RootOrder</c>s. To get child <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> in the order in which they appear
        ///     in the <c>.prefab</c> file, use <see cref="KnownChildrenInPrefabFileOrder"/>
        ///     instead.
        ///     <para/>
        ///     To get the child <see cref="Transform">Transforms</see> of a <see cref="Transform"/>
        ///     that has not been instantiated by a <c>PrefabInstance</c>, <see cref="Children"/>
        ///     can be used instead.
        /// </remarks>
        public IReadOnlyList<Either<Transform, PrefabInstance>> KnownChildrenInRootOrder
        {
            get
            {
                if (this.knownChildrenInRootOrderNeedsSorting)
                {
                    this.SortKnownChildrenInRootOrder();
                }
                return this.knownChildrenInRootOrder;
            }
        }

        /// <summary>
        ///     Adds a <see cref="Transform"/> that has not been instantiated by a
        ///     <c>PrefabInstance</c>to the known children of the <c>Transform</c>.
        /// </summary>
        /// <param name="transform">
        ///     The <see cref="Transform"/> to add to the known children of the <c>Transform</c>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="transform"/> is a <c>Transform</c> that has been
        ///     instantiated by a <c>PrefabInstance</c>.
        /// </exception>
        internal void AddKnownChild(Transform transform)
        {
            if (transform.IsInstance)
            {
                throw new ArgumentOutOfRangeException(nameof(transform));
            }

            this.AddKnownChild(new Either<Transform, PrefabInstance>(transform));
        }

        /// <summary>
        ///     Adds a <see cref="PrefabInstance"/> to the known children of the <c>Transform</c>.
        /// </summary>
        /// <param name="prefabInstance">
        ///     The <see cref="PrefabInstance"/> to add to the known children of the
        ///     <c>Transform</c>.
        /// </param>
        internal void AddKnownChild(PrefabInstance prefabInstance)
        {
            this.AddKnownChild(new Either<Transform, PrefabInstance>(prefabInstance));
        }

        /// <summary>
        ///     Adds a <see cref="Transform"/> or <see cref="PrefabInstance"/> to the known children
        ///     of the <c>Transform</c>.
        /// </summary>
        /// <param name="transformOrPrefabInstance">
        ///     The <see cref="Transform"/> or <see cref="PrefabInstance"/> to add to the known
        ///     children of the <c>Transform</c>.
        /// </param>
        internal void AddKnownChild(Either<Transform, PrefabInstance> transformOrPrefabInstance)
        {
            this.knownChildrenInPrefabFileOrder.Add(transformOrPrefabInstance);
            this.knownChildrenInRootOrder.Add(transformOrPrefabInstance);

            if (!this.knownChildrenInRootOrderNeedsSorting)
            {
                // `knownChildrenInRootOrder` has already been sorted, which means that other code
                // may already hold a reference to it. Ensure that it remains sorted.
                this.SortKnownChildrenInRootOrder();
            }
        }

        /// <summary>
        ///     Gets the <c>Transform</c>'s <c>m_RootOrder</c>.
        /// </summary>
        /// <remarks>
        ///     Does not work for <c>Transform</c>s instantiated by <c>PrefabInstance</c>s. For the
        ///     <c>Transform</c> instantiated at the root of a <c>PrefabInstance</c>, use
        ///     <see cref="Object.PrefabInstance"/>'s <see cref="PrefabInstance.RootOrder"/>
        ///     instead.
        /// </remarks>
        public long? RootOrder =>
            this.YamlMapping?.TryGetValue<YamlInteger>("m_RootOrder")?.Int64Value;

        /// <summary>
        ///     Gets the <see cref="FileID"/> of the <c>Transform</c>'s parent <c>Transform</c>.
        /// </summary>
        public FileID? FatherFileID => this.YamlMapping?.GetFileIDValue("m_Father");

        /// <summary>
        ///     Gets the <c>Transform</c>'s parent <see cref="Transform"/>.
        /// </summary>
        public Transform? Father => this.FatherFileID is FileID fileID ?
            this.Prefab.GetObjectByFileID<Transform>(fileID) :
            null;

        /// <summary>
        ///     Gets the <see cref="FileID">FileIDs</see> of the <c>Transform</c>'s child
        ///     <c>Transform</c>s.
        /// </summary>
        public IReadOnlyList<FileID?>? ChildrenFileIDs =>
            this.YamlMapping?
                .TryGetValue<YamlSequence>("m_Children")?
                .Select(x => x.ToFileID())
                .ToList() ??
            null;

        /// <summary>
        ///     Gets the <c>Transform</c>'s child <see cref="Transform">Transforms</see>.
        /// </summary>
        /// <remarks>
        ///     Does not work for <c>Transform</c>s instantiated by <c>PrefabInstance</c>s. For such
        ///     <c>Transform</c>s, <see cref="KnownChildrenInPrefabFileOrder"/> should be used
        ///     instead.
        /// </remarks>
        public IReadOnlyList<Transform?>? Children => this.ChildrenFileIDs?.Select(
            x => x is FileID fileID ? this.Prefab.GetObjectByFileID<Transform>(fileID) : null
        ).ToList();

        /// <summary>
        ///     Sorts <see cref="knownChildrenInRootOrder"/> in the order given by their
        ///     <c>m_RootOrder</c>s.
        /// </summary>
        private void SortKnownChildrenInRootOrder()
        {
            this.knownChildrenInRootOrder.Sort(
                (a, b) => Nullable.Compare(
                    a.Match(
                        transform => transform.RootOrder,
                        prefabInstance => prefabInstance.RootOrder
                    ),
                    b.Match(
                        transform => transform.RootOrder,
                        prefabInstance => prefabInstance.RootOrder
                    )
                )
            );
            this.knownChildrenInRootOrderNeedsSorting = false;
        }
    }
}
