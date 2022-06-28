using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using notcake.Functional;

namespace notcake.Unity.Prefab
{
    public partial class PrefabFileBijection
    {
        /// <summary>
        ///     Computes the mapping between the Unity objects in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <returns>The computed <see cref="PrefabFileBijection"/>.</returns>
        /// <inheritdoc cref="PrefabFileBijection(PrefabFile, PrefabFile)"/>
        public static PrefabFileBijection Compute(
            PrefabFile leftPrefabFile,
            PrefabFile rightPrefabFile
        )
        {
            PrefabFileBijection prefabFileBijection = new(leftPrefabFile, rightPrefabFile);
            Queue<Action> queue = new();

            queue.Enqueue(
                () => PrefabFileBijection.ComputeRootMapping(
                    prefabFileBijection,
                    queue,
                    leftPrefabFile,
                    rightPrefabFile
                )
            );

            while (queue.Count > 0)
            {
                queue.Dequeue()();
            }

            PrefabFileBijection.ComputeOtherMapping(
                prefabFileBijection,
                leftPrefabFile,
                rightPrefabFile
            );

            return prefabFileBijection;
        }

        /// <summary>
        ///     Computes the mapping between the root Unity objects and their descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <param name="prefabFileBijection">
        ///     The <see cref="PrefabFileBijection"/> in which to store the computed mapping.
        /// </param>
        /// <param name="queue">
        ///     A queue to which to append further mapping computations to be done.
        /// </param>
        /// <param name="leftPrefabFile">The left <see cref="PrefabFile"/> of the bijection.</param>
        /// <param name="rightPrefabFile">
        ///     The right <see cref="PrefabFile"/> of the bijection.
        /// </param>
        private static void ComputeRootMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            PrefabFile leftPrefabFile,
            PrefabFile rightPrefabFile
        )
        {
            PrefabFileBijection.ComputeRootGameObjectMapping(
                prefabFileBijection,
                queue,
                leftPrefabFile.RootGameObjects,
                leftPrefabFile.RootTransforms,
                rightPrefabFile.RootGameObjects,
                rightPrefabFile.RootTransforms
            );

            PrefabFileBijection.ComputeRootPrefabInstanceMapping(
                prefabFileBijection,
                queue,
                leftPrefabFile.RootPrefabInstances,
                rightPrefabFile.RootPrefabInstances
            );
        }

        /// <summary>
        ///     Computes the mapping between the root <see cref="GameObject">GameObjects</see> and
        ///     <see cref="Transform">Transforms</see> and their descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <remarks>
        ///     Root <see cref="GameObject">GameObjects</see> and
        ///     <see cref="Transform">Transforms</see> are never those instantiated by a
        ///     <c>PrefabInstance</c>.
        /// </remarks>
        /// <param name="leftRootGameObjects">
        ///     The root <see cref="GameObject">GameObjects</see> of the left
        ///     <see cref="PrefabFile"/>.
        ///     <para/>
        ///     Must have the same number of elements as <paramref name="leftRootTransforms"/>.
        /// </param>
        /// <param name="leftRootTransforms">
        ///     The root <see cref="Transform">Transforms</see> of the left
        ///     <see cref="PrefabFile"/>.
        ///     <para/>
        ///     Must have the same number of elements as <paramref name="leftRootGameObjects"/>.
        /// </param>
        /// <param name="rightRootGameObjects">
        ///     The root <see cref="GameObject">GameObjects</see> of the right
        ///     <see cref="PrefabFile"/>.
        ///     <para/>
        ///     Must have the same number of elements as <paramref name="rightRootTransforms"/>.
        /// </param>
        /// <param name="rightRootTransforms">
        ///     The root <see cref="Transform">Transforms</see> of the right
        ///     <see cref="PrefabFile"/>.
        ///     <para/>
        ///     Must have the same number of elements as <paramref name="rightRootGameObjects"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputeRootGameObjectMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            IReadOnlyList<GameObject?> leftRootGameObjects,
            IReadOnlyList<Transform> leftRootTransforms,
            IReadOnlyList<GameObject?> rightRootGameObjects,
            IReadOnlyList<Transform> rightRootTransforms
        )
        {
            if (leftRootGameObjects.Count == 1 && rightRootGameObjects.Count == 1)
            {
                PrefabFileBijection.ComputeGameObjectTransformMapping(
                    prefabFileBijection,
                    queue,
                    leftRootGameObjects[0],
                    leftRootTransforms[0],
                    rightRootGameObjects[0],
                    rightRootTransforms[0]
                );
            }
            else
            {
                // Otherwise give up.
            }
        }

        /// <summary>
        ///     Computes the mapping between the root
        ///     <see cref="PrefabInstance">PrefabInstances</see> and their descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <param name="leftRootPrefabInstances">
        ///     The root <see cref="PrefabInstance">PrefabInstances</see> of the left
        ///     <see cref="PrefabFile"/>.
        /// </param>
        /// <param name="rightRootPrefabInstances">
        ///     The root <see cref="PrefabInstance">PrefabInstances</see> of the right
        ///     <see cref="PrefabFile"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputeRootPrefabInstanceMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            IReadOnlyList<PrefabInstance> leftRootPrefabInstances,
            IReadOnlyList<PrefabInstance> rightRootPrefabInstances
        )
        {
            Dictionary<(string, FileID), List<PrefabInstance>> leftRootPrefabInstancesBySourcePrefab =
                PrefabFileBijection.GroupBy(
                    leftRootPrefabInstances,
                    prefabInstance => prefabInstance.SourcePrefabGuidFileID
                );
            Dictionary<(string, FileID), List<PrefabInstance>> rightRootPrefabInstancesBySourcePrefab =
                PrefabFileBijection.GroupBy(
                    rightRootPrefabInstances,
                    prefabInstance => prefabInstance.SourcePrefabGuidFileID
                );

            HashSet<PrefabInstance> remainingLeftRootPrefabInstances =
                leftRootPrefabInstances.ToHashSet();
            HashSet<PrefabInstance> remainingRightRootPrefabInstances =
                rightRootPrefabInstances.ToHashSet();

            // Match `PrefabInstance`s with the same unique source prefab GUID.
            foreach (
                (string, FileID) sourcePrefab
                in leftRootPrefabInstancesBySourcePrefab.Keys
            )
            {
                if (!rightRootPrefabInstancesBySourcePrefab.ContainsKey(sourcePrefab)) { continue; }

                if (leftRootPrefabInstancesBySourcePrefab[sourcePrefab].Count  != 1) { continue; }
                if (rightRootPrefabInstancesBySourcePrefab[sourcePrefab].Count != 1) { continue; }

                PrefabInstance leftRootPrefabInstance =
                    leftRootPrefabInstancesBySourcePrefab[sourcePrefab][0];
                PrefabInstance rightRootPrefabInstance =
                    rightRootPrefabInstancesBySourcePrefab[sourcePrefab][0];

                remainingLeftRootPrefabInstances.Remove(leftRootPrefabInstance);
                remainingRightRootPrefabInstances.Remove(rightRootPrefabInstance);

                PrefabFileBijection.ComputePrefabInstanceMapping(
                    prefabFileBijection,
                    queue,
                    leftRootPrefabInstance,
                    rightRootPrefabInstance
                );
            }

            // If there is only one unmatched `PrefabInstance` on both sides, match them up.
            if (remainingLeftRootPrefabInstances.Count == 1 &&
                remainingRightRootPrefabInstances.Count == 1)
            {
                PrefabFileBijection.ComputePrefabInstanceMapping(
                    prefabFileBijection,
                    queue,
                    remainingLeftRootPrefabInstances.First(),
                    remainingRightRootPrefabInstances.First()
                );
            }

            // Otherwise give up.
        }

        /// <summary>
        ///     Computes the mapping between the given <see cref="GameObject">GameObjects</see>,
        ///     their <see cref="Transform">Transforms</see> and their descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <remarks>
        ///     The given <see cref="GameObject">GameObjects</see> and
        ///     <see cref="Transform">Transforms</see> must not have been instantiated by a
        ///     <c>PrefabInstance</c>.
        /// </remarks>
        /// <param name="leftGameObject">
        ///     The <see cref="GameObject"/> in the left <see cref="PrefabFile"/> to map onto
        ///     <paramref name="rightGameObject"/>.
        /// </param>
        /// <param name="leftTransform">
        ///     The <see cref="Transform"/> in the left <see cref="PrefabFile"/> to map onto
        ///     <paramref name="rightTransform"/>.
        /// </param>
        /// <param name="rightGameObject">
        ///     The <see cref="GameObject"/> in the right <see cref="PrefabFile"/> to map onto
        ///     <paramref name="leftGameObject"/>.
        /// </param>
        /// <param name="rightTransform">
        ///     The <see cref="Transform"/> in the right <see cref="PrefabFile"/> to map onto
        ///     <paramref name="leftTransform"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputeGameObjectTransformMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            GameObject? leftGameObject,
            Transform leftTransform,
            GameObject? rightGameObject,
            Transform rightTransform
        )
        {
            prefabFileBijection.Add(leftTransform, rightTransform);

            if (leftGameObject != null && rightGameObject != null)
            {
                prefabFileBijection.Add(leftGameObject, rightGameObject);

                // Handle `GameObject` components
                if (leftGameObject.Components  is IReadOnlyList<Component?> leftComponents &&
                    rightGameObject.Components is IReadOnlyList<Component?> rightComponents)
                {
                    PrefabFileBijection.ComputeGameObjectComponentMapping(
                        prefabFileBijection,
                        queue,
                        leftComponents,
                        rightComponents
                    );
                }
            }

            // Handle `Transform` child `Transform`s and `PrefabInstance`s.
            PrefabFileBijection.ComputeTransformChildrenMapping(
                prefabFileBijection,
                queue,
                leftTransform.KnownChildrenInRootOrder,
                rightTransform.KnownChildrenInRootOrder
            );
        }


        /// <summary>
        ///     Computes the mapping between the <see cref="Component">Components</see> of a
        ///     <c>GameObject</c> in two <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <remarks>
        ///     Does not compute the mapping between the <c>GameObject</c>s or children of any
        ///     <c>Transform</c> components.
        /// </remarks>
        /// <param name="leftComponents">
        ///     The ordered <see cref="Component">Components</see> in the left
        ///     <see cref="PrefabFile"/> to map onto <paramref name="rightComponents"/>.
        /// </param>
        /// <param name="rightComponents">
        ///     The ordered <see cref="Component">Components</see> in the right
        ///     <see cref="PrefabFile"/> to map onto <paramref name="leftComponents"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        private static void ComputeGameObjectComponentMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            IReadOnlyList<Component?> leftComponents,
            IReadOnlyList<Component?> rightComponents
        )
        {
            // Group the `Component`s by their type.
            Dictionary<(string, (string, FileID)?), List<Component?>> leftComponentsByType =
                PrefabFileBijection.GroupBy<(string, (string, FileID)?), Component?>(
                    leftComponents,
                    component => component?.Type is string type ?
                        (component.Type, (component as MonoBehaviour)?.ScriptGuidFileID) :
                        null
                );
            Dictionary<(string, (string, FileID)?), List<Component?>> rightComponentsByType =
                PrefabFileBijection.GroupBy<(string, (string, FileID)?), Component?>(
                    rightComponents,
                    component => component?.Type is string type ?
                        (component.Type, (component as MonoBehaviour)?.ScriptGuidFileID) :
                        null
                );

            foreach ((string, (string, FileID)?) type in leftComponentsByType.Keys)
            {
                if (!rightComponentsByType.ContainsKey(type)) { continue; }

                List<Component?> leftComponentsOfType  = leftComponentsByType[type];
                List<Component?> rightComponentsOfType = rightComponentsByType[type];

                // Map `Component`s of the same type to each other, assuming that their order is
                // unchanged.
                PrefabFileBijection.ComputeCommonMapping(
                    prefabFileBijection,
                    leftComponentsByType[type],
                    rightComponentsByType[type],
                    (leftComponent, rightComponent) =>
                    {
                        if (prefabFileBijection.ContainsLeft(leftComponent))   { return; }
                        if (prefabFileBijection.ContainsRight(rightComponent)) { return; }

                        prefabFileBijection.Add(leftComponent, rightComponent);
                    }
                );
            }
        }

        /// <summary>
        ///     Computes the mapping between the child <see cref="Transform">Transforms</see> and
        ///     <see cref="PrefabInstance">PrefabInstances</see> of a <c>Transform</c>,
        ///     their <c>GameObject</c>s and descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <param name="leftChildrenInRootOrder">
        ///     The ordered child <see cref="Transform">Transforms</see> in the left
        ///     <see cref="PrefabFile"/> to map onto <paramref name="rightTransforms"/>.
        /// </param>
        /// <param name="rightChildrenInRootOrder">
        ///     The ordered child <see cref="Transform">Transforms</see> in the right
        ///     <see cref="PrefabFile"/> to map onto <paramref name="leftTransforms"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputeTransformChildrenMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            IReadOnlyList<Either<Transform, PrefabInstance>> leftChildrenInRootOrder,
            IReadOnlyList<Either<Transform, PrefabInstance>> rightChildrenInRootOrder
        )
        {
            HashSet<Transform> leftMappedTransforms  = new();
            HashSet<Transform> rightMappedTransforms = new();
            HashSet<PrefabInstance> leftMappedPrefabInstances  = new();
            HashSet<PrefabInstance> rightMappedPrefabInstances = new();

            List<Transform?> leftTransformsInRootOrder =
                leftChildrenInRootOrder
                    .Select(transformOrPrefabInstance => transformOrPrefabInstance.LeftOrDefault)
                    .Where(transform => transform != null)
                    .ToList();
            List<Transform?> rightTransformsInRootOrder =
                rightChildrenInRootOrder
                    .Select(transformOrPrefabInstance => transformOrPrefabInstance.LeftOrDefault)
                    .Where(transform => transform != null)
                    .ToList();
            List<PrefabInstance?> leftPrefabInstancesInRootOrder =
                leftChildrenInRootOrder
                    .Select(transformOrPrefabInstance => transformOrPrefabInstance.RightOrDefault)
                    .Where(prefabInstance => prefabInstance != null)
                    .ToList();
            List<PrefabInstance?> rightPrefabInstancesInRootOrder =
                rightChildrenInRootOrder
                    .Select(transformOrPrefabInstance => transformOrPrefabInstance.RightOrDefault)
                    .Where(prefabInstance => prefabInstance != null)
                    .ToList();

            // Match up `Transforms` whose `GameObject`s have the same name.
            Dictionary<string, List<Transform?>> leftTransformsByName =
                PrefabFileBijection.GroupBy(
                    leftTransformsInRootOrder,
                    transform => transform?.GameObject?.Name
                );
            Dictionary<string, List<Transform?>> rightTransformsByName =
                PrefabFileBijection.GroupBy(
                    rightTransformsInRootOrder,
                    transform => transform?.GameObject?.Name
                );

            foreach (string name in leftTransformsByName.Keys)
            {
                if (!rightTransformsByName.ContainsKey(name)) { continue; }

                PrefabFileBijection.ComputeCommonMapping(
                    prefabFileBijection,
                    leftTransformsByName[name],
                    rightTransformsByName[name],
                    (leftTransform, rightTransform) =>
                    {
                        leftMappedTransforms.Add(leftTransform);
                        rightMappedTransforms.Add(rightTransform);

                        queue.Enqueue(
                            () => PrefabFileBijection.ComputeGameObjectTransformMapping(
                                prefabFileBijection,
                                queue,
                                leftTransform.GameObject,
                                leftTransform,
                                rightTransform.GameObject,
                                rightTransform
                            )
                        );
                    }
                );
            }

            // Match up the instantiated root `Transform`s of `PrefabInstance`s.
            Dictionary<(string, FileID), List<PrefabInstance?>> leftPrefabInstancesBySourcePrefab =
                PrefabFileBijection.GroupBy(
                    leftPrefabInstancesInRootOrder,
                    prefabInstance => prefabInstance?.SourcePrefabGuidFileID
                );
            Dictionary<(string, FileID), List<PrefabInstance?>> rightPrefabInstancesBySourcePrefab =
                PrefabFileBijection.GroupBy(
                    rightPrefabInstancesInRootOrder,
                    prefabInstance => prefabInstance?.SourcePrefabGuidFileID
                );

            foreach ((string, FileID) sourcePrefab in leftPrefabInstancesBySourcePrefab.Keys)
            {
                if (!rightPrefabInstancesBySourcePrefab.ContainsKey(sourcePrefab)) { continue; }

                PrefabFileBijection.ComputeCommonMapping(
                    prefabFileBijection,
                    leftPrefabInstancesBySourcePrefab[sourcePrefab],
                    rightPrefabInstancesBySourcePrefab[sourcePrefab],
                    (leftPrefabInstance, rightPrefabInstance) =>
                    {
                        leftMappedPrefabInstances.Add(leftPrefabInstance);
                        rightMappedPrefabInstances.Add(rightPrefabInstance);

                        queue.Enqueue(
                            () => PrefabFileBijection.ComputePrefabInstanceMapping(
                                prefabFileBijection,
                                queue,
                                leftPrefabInstance,
                                rightPrefabInstance
                            )
                        );
                    }
                );
            }

            // Match up any remaining `Transform`s in order.
            List<Transform?> leftRemainingTransforms =
                leftTransformsInRootOrder
                    .Where(
                        transform => transform == null || !leftMappedTransforms.Contains(transform)
                    )
                    .ToList();
            List<Transform?> rightRemainingTransforms =
                rightTransformsInRootOrder
                    .Where(
                        transform => transform == null || !rightMappedTransforms.Contains(transform)
                    )
                    .ToList();
            PrefabFileBijection.ComputeCommonMapping(
                prefabFileBijection,
                leftRemainingTransforms,
                rightRemainingTransforms,
                (leftTransform, rightTransform) => queue.Enqueue(
                    () => PrefabFileBijection.ComputeGameObjectTransformMapping(
                        prefabFileBijection,
                        queue,
                        leftTransform.GameObject,
                        leftTransform,
                        rightTransform.GameObject,
                        rightTransform
                    )
                )
            );

            // Match up any remaining `PrefabInstance`s in order.
            List<PrefabInstance?> leftRemainingPrefabInstances =
                leftPrefabInstancesInRootOrder
                    .Where(
                        prefabInstance => prefabInstance == null ||
                                          !leftMappedPrefabInstances.Contains(prefabInstance)
                    )
                    .ToList();
            List<PrefabInstance?> rightRemainingPrefabInstances =
                rightPrefabInstancesInRootOrder
                    .Where(
                        prefabInstance => prefabInstance == null ||
                                          !rightMappedPrefabInstances.Contains(prefabInstance)
                    )
                    .ToList();
            PrefabFileBijection.ComputeCommonMapping(
                prefabFileBijection,
                leftRemainingPrefabInstances,
                rightRemainingPrefabInstances,
                (leftPrefabInstance, rightPrefabInstance) => queue.Enqueue(
                    () => PrefabFileBijection.ComputePrefabInstanceMapping(
                        prefabFileBijection,
                        queue,
                        leftPrefabInstance,
                        rightPrefabInstance
                    )
                )
            );
        }

        /// <summary>
        ///     Computes the mapping between the given
        ///     <see cref="PrefabInstance">PrefabInstances</see> and their descendants in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <param name="leftPrefabInstance">
        ///     The <see cref="PrefabInstance"/> in the left <see cref="PrefabFile"/> to map onto
        ///     <paramref name="rightPrefabInstance"/>.
        /// </param>
        /// <param name="rightPrefabInstance">
        ///     The <see cref="PrefabInstance"/> in the right <see cref="PrefabFile"/> to map onto
        ///     <paramref name="leftPrefabInstance"/>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputePrefabInstanceMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            PrefabInstance leftPrefabInstance,
            PrefabInstance rightPrefabInstance
        )
        {
            prefabFileBijection.Add(leftPrefabInstance, rightPrefabInstance);

            queue.Enqueue(
                () => PrefabFileBijection.ComputePrefabInstanceDescendantMapping(
                    prefabFileBijection,
                    queue,
                    leftPrefabInstance,
                    rightPrefabInstance
                )
            );
        }

        /// <summary>
        ///     Computes the mapping between the descendants of the given
        ///     <see cref="PrefabInstance">PrefabInstances</see> in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <param name="leftPrefabInstance">
        ///     The <see cref="PrefabInstance"/> in the left <see cref="PrefabFile"/> whose
        ///     descendants are to be mapped onto <paramref name="rightPrefabInstance"/>'s
        ///     descendants.
        /// </param>
        /// <param name="rightPrefabInstance">
        ///     The <see cref="PrefabInstance"/> in the right <see cref="PrefabFile"/> whose
        ///     descendants are to be mapped onto <paramref name="leftPrefabInstance"/>'s
        ///     descendants.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputePrefabInstanceDescendantMapping(
            PrefabFileBijection prefabFileBijection,
            Queue<Action> queue,
            PrefabInstance leftPrefabInstance,
            PrefabInstance rightPrefabInstance
        )
        {
            Dictionary<FileID, List<Object>> leftInstantiatedDescendants = PrefabFileBijection.GroupBy(
                leftPrefabInstance.KnownInstantiatedDescendants,
                @object => @object.CorrespondingSourceObjectFileID
            );
            Dictionary<FileID, List<Object>> rightInstantiatedDescendants = PrefabFileBijection.GroupBy(
                rightPrefabInstance.KnownInstantiatedDescendants,
                @object => @object.CorrespondingSourceObjectFileID
            );

            foreach (FileID fileID in leftInstantiatedDescendants.Keys)
            {
                if (!rightInstantiatedDescendants.ContainsKey(fileID)) { continue; }

                if (leftInstantiatedDescendants[fileID].Count  != 1) { continue; }
                if (rightInstantiatedDescendants[fileID].Count != 1) { continue; }

                // There is only one object with the given `m_CorrespondingSourceObject` on each
                // side. Match them up.
                Object leftObject  = leftInstantiatedDescendants[fileID][0];
                Object rightObject = rightInstantiatedDescendants[fileID][0];

                // The root `Transform`s of the `PrefabInstance`s have already been mapped,
                // unless the `PrefabInstance`s are at the root of the `Prefab`s.
                if (!prefabFileBijection.ContainsLeft(leftObject) &&
                    !prefabFileBijection.ContainsRight(rightObject))
                {
                    prefabFileBijection.Add(leftObject, rightObject);
                }

                switch ((leftObject, rightObject))
                {
                    case (GameObject leftGameObject, GameObject rightGameObject):
                        // Handle `GameObject` components without recursing into `Transform`s.
                        // Any `Transform` children will result in an explicit instantiation of
                        // the `GameObject`'s `Transform`, which will be handled by the
                        // `Transform` case below.
                        PrefabFileBijection.ComputeGameObjectComponentMapping(
                            prefabFileBijection,
                            queue,
                            leftGameObject.KnownComponents,
                            rightGameObject.KnownComponents
                        );
                        break;
                    case (Transform leftTransform, Transform rightTransform):
                        // Handle `Transform` child `Transform`s and `PrefabInstance`s.
                        PrefabFileBijection.ComputeTransformChildrenMapping(
                            prefabFileBijection,
                            queue,
                            leftTransform.KnownChildrenInRootOrder,
                            rightTransform.KnownChildrenInRootOrder
                        );
                        break;
                }
            }
        }

        /// <summary>
        ///     Computes the mapping between the other Unity objects that are not
        ///     <c>GameObject</c>s, <c>Transform</c>s or <c>PrefabInstance</c>s in two
        ///     <see cref="PrefabFile">PrefabFiles</see>.
        /// </summary>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        private static void ComputeOtherMapping(
            PrefabFileBijection prefabFileBijection,
            PrefabFile leftPrefabFile,
            PrefabFile rightPrefabFile
        )
        {
            Dictionary<string, List<Object>> leftOtherObjects =
                PrefabFileBijection.GroupBy(leftPrefabFile.OtherObjects, @object => @object.Type);
            Dictionary<string, List<Object>> rightOtherObjects =
                PrefabFileBijection.GroupBy(rightPrefabFile.OtherObjects, @object => @object.Type);

            // If there is only one object with a given type on each side, match them up.
            foreach (string type in leftOtherObjects.Keys)
            {
                if (!rightOtherObjects.ContainsKey(type)) { continue; }

                if (leftOtherObjects[type].Count  != 1) { continue; }
                if (rightOtherObjects[type].Count != 1) { continue; }

                prefabFileBijection.Add(leftOtherObjects[type][0], rightOtherObjects[type][0]);
            }
        }


        /// <summary>
        ///     Maps two ordered lists of <typeparamref name="ObjectT">ObjectTs</typeparamref> to
        ///     each other.
        /// </summary>
        /// <remarks>Excess elements in either list are not mapped.</remarks>
        /// <typeparam name="ObjectT">The type of <see cref="Object"/> being mapped.</typeparam>
        /// <param name="leftObjects">
        ///     A list of <typeparamref name="ObjectT">ObjectTs</typeparamref> to map to
        ///     <paramref name="rightObjects"/>.
        /// </param>
        /// <param name="rightObjects">
        ///     A list of <typeparamref name="ObjectT">ObjectTs</typeparamref> to map to
        ///     <paramref name="leftObjects"/>.
        /// </param>
        /// <param name="addMapping">
        ///     A callback to add each pair of mapped
        ///     <typeparamref name="ObjectT">ObjectTs</typeparamref>.
        /// </param>
        /// <inheritdoc cref="PrefabFileBijection.ComputeRootMapping(
        ///     PrefabFileBijection,
        ///     Queue{Action},
        ///     PrefabFile,
        ///     PrefabFile
        /// )"/>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        private static void ComputeCommonMapping<ObjectT>(
            PrefabFileBijection prefabFileBijection,
            IReadOnlyList<ObjectT?> leftObjects,
            IReadOnlyList<ObjectT?> rightObjects,
            Action<ObjectT, ObjectT> addMapping
        )
            where ObjectT : Object
        {
            int commonCount = Math.Min(leftObjects.Count, rightObjects.Count);
            for (int i = 0; i < commonCount; i++)
            {
                ObjectT? leftObject  = leftObjects[i];
                ObjectT? rightObject = rightObjects[i];

                if (leftObject  == null) { continue; }
                if (rightObject == null) { continue; }

                addMapping(leftObject, rightObject);
            }
        }

        /// <summary>
        ///     Groups objects by a given key.
        /// </summary>
        /// <typeparam name="KeyT">The type of the grouping key.</typeparam>
        /// <typeparam name="ObjectT">The type of the objects.</typeparam>
        /// <param name="objects">The objects to group.</param>
        /// <param name="getKey">A function that returns the key for a given object.</param>
        /// <returns>
        ///     A dictionary with <typeparamref name="KeyT">KeyTs</typeparamref> as keys and lists
        ///     of <typeparamref name="ObjectT">ObjectTs</typeparamref> as values.
        ///     <para/>
        ///     <typeparamref name="ObjectT">ObjectTs</typeparamref> with <c>null</c> keys are not
        ///     included in the dictionary.
        /// </returns>
        private static Dictionary<KeyT, List<ObjectT>> GroupBy<KeyT, ObjectT>(
            IEnumerable<ObjectT> objects,
            Func<ObjectT, KeyT?> getKey
        )
            where KeyT : class
        {
            Dictionary<KeyT, List<ObjectT>> groups = new();

            foreach (ObjectT @object in objects)
            {
                if (getKey(@object) is not KeyT key) { continue; }

                if (!groups.TryGetValue(key, out List<ObjectT>? group))
                {
                    group = new List<ObjectT>();
                    groups[key] = group;
                }

                group.Add(@object);
            }

            return groups;
        }

        /// <inheritdoc cref="GroupBy{KeyT, ObjectT}(IEnumerable{ObjectT}, Func{ObjectT, KeyT})"/>
        private static Dictionary<KeyT, List<ObjectT>> GroupBy<KeyT, ObjectT>(
            IEnumerable<ObjectT> objects,
            Func<ObjectT, KeyT?> getKey
        )
            where KeyT : struct
        {
            Dictionary<KeyT, List<ObjectT>> groups = new();

            foreach (ObjectT @object in objects)
            {
                if (getKey(@object) is not KeyT key) { continue; }

                if (!groups.TryGetValue(key, out List<ObjectT>? group))
                {
                    group = new List<ObjectT>();
                    groups[key] = group;
                }

                group.Add(@object);
            }

            return groups;
        }
    }
}
