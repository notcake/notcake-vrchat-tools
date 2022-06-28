using System.Collections.Generic;
using System.Linq;

namespace notcake.Unity.Prefab
{
    public partial class PrefabFile
    {
        private readonly List<PrefabInstance> prefabInstances = new();

        /// <summary>
        ///     Gets the <c>PrefabInstance</c>s in the prefab.
        /// </summary>
        public IReadOnlyList<PrefabInstance> PrefabInstances => this.prefabInstances;

        private readonly List<Object> otherObjects;
        private readonly HashSet<Object> otherObjectsSet;

        /// <summary>
        ///     Gets a list of non-<c>GameObject</c>, non-<c>Component</c> and
        ///     non-<c>PrefabInstance</c> Unity objects in the <see cref="PrefabFile"/>.
        /// </summary>
        public IReadOnlyList<Object> OtherObjects => this.otherObjects;

        private readonly List<Transform> rootTransforms;
        private readonly List<GameObject?> rootGameObjects;
        private readonly List<PrefabInstance> rootPrefabInstances;

        /// <summary>
        ///     Gets the <see cref="Transform">Transforms</see> with no <c>m_Father</c>.
        /// </summary>
        /// <remarks>
        ///     Has the same number of elements as <see cref="RootGameObjects"/>.
        ///     <para/>
        ///     Only <see cref="Transform">Transforms</see> with an <c>m_Father</c>
        ///     <see cref="FileID"/> of <c>0</c> are returned.
        ///     <see cref="Transform">Transforms</see> with malformed YAML or an unresolvable
        ///     <c>m_Father</c> are not considered roots.
        /// </remarks>
        public IReadOnlyList<Transform> RootTransforms => this.rootTransforms;

        /// <summary>
        ///     Gets the <see cref="GameObject">GameObjects</see> whose <c>Transform</c>s have no
        ///     <c>m_Father</c>.
        /// </summary>
        /// <remarks>
        ///     Has the same number of elements as <see cref="RootTransforms"/>.
        ///     <para/>
        ///     Contains <c>null</c>s for <c>Transform</c>s with malformed or unresolvable
        ///     <c>m_GameObject</c>s.
        ///     <para/>
        ///     Only the <see cref="GameObject">GameObjects</see> of <c>Transform</c>s with an
        ///     <c>m_Father</c> <see cref="FileID"/> of <c>0</c> are returned. <c>Transform</c>s
        ///     with malformed YAML or an unresolvable <c>m_Father</c> are not considered roots.
        /// </remarks>
        public IReadOnlyList<GameObject?> RootGameObjects => this.rootGameObjects;

        /// <summary>
        ///     Gets the <see cref="PrefabInstance">PrefabInstances</see> with no
        ///     <c>m_Modification.m_TransformParent</c>.
        /// </summary>
        /// <remarks>
        ///     Only <see cref="PrefabInstance">PrefabInstances</see> with an
        ///     <c>m_Modification.m_TransformParent</c> <see cref="FileID"/> of <c>0</c> are
        ///     returned. <see cref="PrefabInstance">PrefabInstances</see> with malformed YAML or an
        ///     unresolvable <c>m_Modification.m_TransformParent</c> are not considered roots.
        /// </remarks>
        public IReadOnlyList<PrefabInstance> RootPrefabInstances => this.rootPrefabInstances;

        /// <summary>
        ///     Gets the root <see cref="Transform"/> of the <see cref="PrefabFile"/>.
        /// </summary>
        /// <remarks>
        ///     Returns <c>null</c> when the <see cref="PrefabFile"/> has multiple or no roots, or
        ///     the root of the <see cref="PrefabFile"/> is a <see cref="PrefabInstance"/>.
        /// </remarks>
        public Transform? RootTransform =>
            this.RootTransforms.Count == 1 && this.RootPrefabInstances.Count == 0 ?
                this.RootTransforms[0] :
                null;

        /// <summary>
        ///     Gets the root <see cref="GameObject"/> of the <see cref="PrefabFile"/>.
        /// </summary>
        /// <remarks>
        ///     Returns <c>null</c> when the <see cref="PrefabFile"/> has multiple or no roots, or
        ///     the root of the <see cref="PrefabFile"/> is a <see cref="PrefabInstance"/>, or the
        ///     root <see cref="Transform"/>'s <c>m_GameObject</c> is malformed or unresolvable.
        /// </remarks>
        public GameObject? RootGameObject =>
            this.RootGameObjects.Count == 1 && this.RootPrefabInstances.Count == 0 ?
                this.RootGameObjects[0] :
                null;

        /// <summary>
        ///     Gets the root <see cref="PrefabInstance"/> of the <see cref="PrefabFile"/>.
        /// </summary>
        /// <remarks>
        ///     Returns <c>null</c> when the <see cref="PrefabFile"/> has multiple or no roots, or
        ///     the root of the <see cref="PrefabFile"/> is a <see cref="GameObject"/>.
        /// </remarks>
        public PrefabInstance? RootPrefabInstance =>
            this.RootTransforms.Count == 0 && this.RootPrefabInstances.Count == 1 ?
                this.RootPrefabInstances[0] :
                null;

        private readonly List<Object> orphanObjects;

        /// <summary>
        ///     Gets a list of <c>GameObject</c>s, <c>Component</c>s and <c>PrefabInstance</c>s in
        ///     the <see cref="PrefabFile"/> that are not reachable from
        ///     <see cref="RootGameObjects"/>, <see cref="RootTransforms"/> or
        ///     <see cref="RootPrefabInstances"/> via <see cref="GameObject.Components"/>,
        ///     <see cref="GameObject.KnownComponents"/>,
        ///     <see cref="Transform.KnownChildrenInRootOrder"/>, <see cref="Component.GameObject"/>
        ///     and <see cref="PrefabInstance.KnownInstantiatedDescendants"/>.
        /// </summary>
        public IReadOnlyList<Object> OrphanObjects => this.orphanObjects;

        /// <summary>
        ///     Computes a list of non-<c>GameObject</c>, non-<c>Component</c> and
        ///     non-<c>PrefabInstance</c> Unity objects in the <see cref="PrefabFile"/>.
        /// </summary>
        /// <returns>
        ///     A list of non-<c>GameObject</c>, non-<c>Component</c> and non-<c>PrefabInstance</c>
        ///     Unity objects in the <see cref="PrefabFile"/>.
        /// </returns>
        private List<Object> ComputeOtherObjects()
        {
            List<Object> otherObjects = new();

            foreach (Object @object in this.Objects)
            {
                if (@object is GameObject or Component or PrefabInstance) { continue; }

                otherObjects.Add(@object);
            }

            return otherObjects;
        }

        /// <summary>
        ///     Computes the root <see cref="Transform">Transforms</see>,
        ///     <see cref="GameObject">GameObjects</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> of the <see cref="PrefabFile"/>.
        /// </summary>
        /// <remarks>
        ///     Root <see cref="Transform">Transforms</see>,
        ///     <see cref="GameObject">GameObjects</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> are those whose parent
        ///     <see cref="FileID"/> is explicitly <c>0</c>.
        ///     <para/>
        ///     <see cref="Transform">Transforms</see>, <see cref="GameObject">GameObjects</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> with malformed YAML or unresolvable
        ///     parent <see cref="FileID">FileIDs</see> are not considered root objects.
        /// </remarks>
        /// <returns>
        ///     A tuple containing:<br/>
        ///     <list type="bullet">
        ///         <item>
        ///             A list of <see cref="Transform">Transforms</see> with no <c>m_Father</c>.
        ///         </item>
        ///         <item>
        ///             A list of <see cref="GameObject">GameObjects</see> containing the
        ///             <see cref="Transform">Transforms</see> with no <c>m_Father</c>.
        ///         </item>
        ///         <item>
        ///             A list of <see cref="PrefabInstance">PrefabInstances</see> with no
        ///             <c>m_TransformParent</c>.
        ///         </item>
        ///     </list>
        /// </returns>
        private (List<Transform>, List<GameObject?>, List<PrefabInstance>) ComputeRoots()
        {
            List<Transform> rootTransforms = new();
            List<PrefabInstance> rootPrefabInstances = new();
            List<GameObject?> rootGameObjects = new();

            foreach (Object @object in this.objects)
            {
                if (@object is Transform transform)
                {
                    if (transform.FatherFileID == FileID.Zero)
                    {
                        rootTransforms.Add(transform);
                        rootGameObjects.Add(transform.GameObject);
                    }
                }
                else if (@object is PrefabInstance prefabInstance)
                {
                    if (prefabInstance.TransformParentFileID == FileID.Zero)
                    {
                        rootPrefabInstances.Add(prefabInstance);
                    }
                }
            }

            return (rootTransforms, rootGameObjects, rootPrefabInstances);
        }

        /// <summary>
        ///     Computes a list of <c>GameObject</c>s, <c>Component</c>s and <c>PrefabInstance</c>s
        ///     in the <see cref="PrefabFile"/> that are not reachable from
        ///     <see cref="RootGameObjects"/>, <see cref="RootTransforms"/> or
        ///     <see cref="RootPrefabInstances"/> via <see cref="GameObject.Components"/>,
        ///     <see cref="GameObject.KnownComponents"/>,
        ///     <see cref="Transform.KnownChildrenInRootOrder"/>, <see cref="Component.GameObject"/>
        ///     and <see cref="PrefabInstance.KnownInstantiatedDescendants"/>.
        /// </summary>
        /// <param name="otherObjects">The set of <see cref="OtherObjects"/> to exclude.</param>
        /// <returns>
        ///     A list of <c>GameObject</c>s, <c>Component</c>s and <c>PrefabInstance</c>s that are
        ///     not reachable from <see cref="RootGameObjects"/>, <see cref="RootTransforms"/> or
        ///     <see cref="RootPrefabInstances"/>.
        /// </returns>
        private List<Object> ComputeOrphanObjects(IEnumerable<Object> otherObjects)
        {
            HashSet<Object> reachableObjects = new(otherObjects);
            Queue<Object> queue = new();

            // Enqueue the root `GameObject`s, `Transform`s and `PrefabInstance`s.
            foreach (GameObject? gameObject in this.rootGameObjects)
            {
                if (gameObject == null) { continue; }
                queue.Enqueue(gameObject);
                reachableObjects.Add(gameObject);
            }

            foreach (Transform transform in this.rootTransforms)
            {
                queue.Enqueue(transform);
                reachableObjects.Add(transform);
            }

            foreach (PrefabInstance prefabInstance in this.rootPrefabInstances)
            {
                queue.Enqueue(prefabInstance);
                reachableObjects.Add(prefabInstance);
            }

            while (queue.Count > 0)
            {
                Object @object = queue.Dequeue();

                IEnumerable<Object?>? children = null;
                if (@object is GameObject gameObject)
                {
                    children = gameObject.IsInstance ?
                        gameObject.KnownComponents :
                        gameObject.Components;
                }
                else if (@object is Transform transform)
                {
                    children = transform.KnownChildrenInRootOrder.Select(
                        transformOrPrefabInstance => transformOrPrefabInstance.Match<Object>(
                            transform => transform,
                            prefabInstance => prefabInstance
                        )
                    );

                    GameObject? parentGameObject = transform.GameObject;
                    if (parentGameObject != null && !reachableObjects.Contains(parentGameObject))
                    {
                        reachableObjects.Add(parentGameObject);
                        queue.Enqueue(parentGameObject);
                    }
                }
                else if (@object is PrefabInstance prefabInstance)
                {
                    children = prefabInstance.KnownInstantiatedDescendants;
                }

                if (children == null) { continue; }

                foreach (Object? childObject in children)
                {
                    if (childObject == null) { continue; }
                    if (reachableObjects.Contains(childObject)) { continue; }

                    reachableObjects.Add(childObject);
                    queue.Enqueue(childObject);
                }
            }

            List<Object> orphanObjects = new();
            foreach (Object @object in this.objects)
            {
                if (reachableObjects.Contains(@object)) { continue; }

                orphanObjects.Add(@object);
            }

            return orphanObjects;
        }

        /// <summary>
        ///     Ensures that all lists of objects are in the same order as <see cref="Objects"/>.
        /// </summary>
        private void UpdateAuxiliaryObjectListOrdering()
        {
            Dictionary<Object, int> positions = new();
            for (int i = 0; i < this.objects.Count; i++)
            {
                positions[this.objects[i]] = i;
            }

            this.prefabInstances.Sort((a, b) => positions[a].CompareTo(positions[b]));
            this.otherObjects.Sort((a, b) => positions[a].CompareTo(positions[b]));
            this.orphanObjects.Sort((a, b) => positions[a].CompareTo(positions[b]));
        }
    }
}
