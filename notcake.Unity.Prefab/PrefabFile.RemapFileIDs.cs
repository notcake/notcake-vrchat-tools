using System;
using System.Collections.Generic;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    public partial class PrefabFile
    {
        /// <summary>
        ///     Remaps the <c>fileID</c>s in a <see cref="PrefabFile"/>.
        /// </summary>
        /// <param name="fileIDMapping">
        ///     A dictionary mapping old <see cref="FileID"/>s to new <see cref="FileID"/>s.
        ///     <para/>
        ///     Unity objects whose <see cref="FileID"/>s which are omitted from the dictionary are
        ///     left unchanged, unless they have been instantiated by a
        ///     <see cref="PrefabInstance"/>.
        /// </param>
        /// <param name="newOrderingHint">
        ///     An ordered list of new <see cref="FileID"/>s, to use as a hint for reordering the
        ///     Unity objects.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when multiple Unity objects would end up with the same <c>fileID</c>.
        /// </exception>
        public void RemapFileIDs(
            IReadOnlyDictionary<FileID, FileID> fileIDMapping,
            IReadOnlyList<FileID>? newOrderingHint = null
        )
        {
            // Validate the remap operation.
            Dictionary<FileID, FileID> reverseFileIDMapping = new();
            foreach ((FileID oldFileID, FileID newFileID) in fileIDMapping)
            {
                if (this.objectsByFileID.ContainsKey(oldFileID))
                {
                    if (reverseFileIDMapping.TryGetValue(newFileID, out FileID otherOldFileID))
                    {
                        throw new InvalidOperationException(
                            $"Cannot remap both &{oldFileID} and &{otherOldFileID} to &{newFileID}."
                        );
                    }

                    reverseFileIDMapping.Add(newFileID, oldFileID);
                }

                if (this.objectsByFileID.ContainsKey(newFileID) &&
                    !fileIDMapping.ContainsKey(newFileID))
                {
                    throw new InvalidOperationException(
                        $"Cannot remap &{oldFileID} to &{newFileID} because an object with that " +
                        "fileID already exists."
                    );
                }
            }

            Dictionary<FileID, FileID> fullFileIDMapping = new(fileIDMapping);

            // Identify the object ordering before remapping `fileID`s.
            bool isOrderedByFileIDAscending = true;
            for (int i = 1; i < this.objects.Count; i++)
            {
                if (this.objects[i].FileID < this.objects[i - 1].FileID)
                {
                    isOrderedByFileIDAscending = false;
                    break;
                }
            }

            // Determine whether `Component`s are grouped by their `GameObject` `fileID`s in
            // ascending order.
            FileID? lastComponentGameObjectFileID = null;
            bool componentsOrderedByGameObjectFileIDAscending = true;
            for (int i = 0; i < this.objects.Count; i++)
            {
                if (this.objects[i].IsInstance ||
                    this.objects[i] is not Component component)
                {
                    continue;
                }

                FileID? componentGameObjectFileID = component.GameObjectFileID;
                if (componentGameObjectFileID == null) { continue; }

                lastComponentGameObjectFileID ??= componentGameObjectFileID;
                if (componentGameObjectFileID < lastComponentGameObjectFileID)
                {
                    // `GameObject` `fileID`s went backwards.
                    // Assume that `Component`s are grouped by their `GameObject`s or
                    // `PrefabInstance`s instead of their `GameObject`s only.
                    componentsOrderedByGameObjectFileIDAscending = false;
                    break;
                }

                lastComponentGameObjectFileID = componentGameObjectFileID;
            }

            // When the `fileID` of a `PrefabInstance` changes, the `fileID`s of all its
            // instantiated descendants also changes. Compute those changes and add them to the
            // mapping.
            foreach (PrefabInstance prefabInstance in this.prefabInstances)
            {
                if (!fileIDMapping.ContainsKey(prefabInstance.FileID)) { continue; }

                FileID oldPrefabInstanceFileID = prefabInstance.FileID;
                FileID newPrefabInstanceFileID = fileIDMapping[prefabInstance.FileID];

                foreach (Object @object in prefabInstance.KnownInstantiatedDescendants)
                {
                    if (fileIDMapping.ContainsKey(@object.FileID)) { continue; }

                    // Leave instantiated descendants with `fileID`s that do not check out alone.
                    FileID? expectedFileID = @object.CorrespondingSourceObjectFileID?.Instantiate(
                        oldPrefabInstanceFileID
                    );
                    if (@object.FileID != expectedFileID) { continue; }

                    fullFileIDMapping[@object.FileID] = @object.FileID.Reinstantiate(
                        oldPrefabInstanceFileID,
                        newPrefabInstanceFileID
                    );
                }
            }

            // Remap `objectsByFileID` and `objectFileIDs`.
            // These cannot be updated in place, otherwise `fileID` swaps will not be handled
            // correctly.
            Dictionary<FileID, Object> newObjectsByFileID = new();
            Dictionary<Object, FileID> newObjectFileIDs = new();
            foreach (Object @object in this.objects)
            {
                FileID oldFileID = @object.FileID;
                if (!fullFileIDMapping.TryGetValue(oldFileID, out FileID newFileID))
                {
                    newFileID = oldFileID;
                }

                newObjectsByFileID[newFileID] = @object;
                newObjectFileIDs[@object] = newFileID;
            }

            this.objectsByFileID = newObjectsByFileID;
            this.objectFileIDs = newObjectFileIDs;

            // Remap references in YAML.
            Queue<YamlNode> queue = new();
            foreach (Object @object in this.objects)
            {
                queue.Enqueue(@object.Document.RootNode);
            }

            while (queue.Count > 0)
            {
                YamlNode yamlNode = queue.Dequeue();

                switch (yamlNode)
                {
                    case YamlSequence yamlSequence:
                        foreach (YamlNode yamlChildNode in yamlSequence)
                        {
                            queue.Enqueue(yamlChildNode);
                        }
                        break;
                    case YamlMapping yamlMapping:
                        foreach ((YamlNode yamlKeyNode, YamlNode yamlValueNode) in yamlMapping)
                        {
                            queue.Enqueue(yamlKeyNode);
                            queue.Enqueue(yamlValueNode);
                        }

                        if (!yamlMapping.ContainsKey("guid") &&
                            yamlMapping.ToFileID() is FileID fileID &&
                            fullFileIDMapping.ContainsKey(fileID))
                        {
                            yamlMapping["fileID"] =
                                new YamlInteger(fullFileIDMapping[fileID].Value);
                        }
                        break;
                }
            }

            // Reorder objects.
            if (isOrderedByFileIDAscending)
            {
                this.objects.Sort((a, b) => a.FileID.CompareTo(b.FileID));
            }
            else
            {
                // Used to order other objects and `PrefabInstance`-instantiated Unity objects.
                Dictionary<FileID, int> oldPositions = new();
                // Used to order `Component`s within `GameObject`s.
                Dictionary<FileID, int> componentPositions = new();
                for (int i = 0; i < this.objects.Count; i++)
                {
                    oldPositions[this.objects[i].FileID] = i;
                    componentPositions[this.objects[i].FileID] = i;
                }

                if (newOrderingHint != null)
                {
                    // Override the ordering using `newOrderingHint`.
                    for (int i = 0; i < newOrderingHint.Count; i++)
                    {
                        oldPositions[newOrderingHint[i]] = i;
                    }
                }

                // Used to order `Component`s in `PrefabInstance`-instantiated `GameObject`s.
                // Contains the position of the first `Component` in each `GameObject`, taking
                // `newOrderingHint` into account.
                Dictionary<FileID, int> gameObjectComponentPositions = new();
                foreach (Object @object in this.objects)
                {
                    if (@object is not Component component) { continue; }
                    if (component.GameObject?.FileID is not FileID gameObjectFileID) { continue; }

                    if (gameObjectComponentPositions.ContainsKey(gameObjectFileID))
                    {
                        gameObjectComponentPositions[gameObjectFileID] = Math.Min(
                            gameObjectComponentPositions[gameObjectFileID],
                            oldPositions[component.FileID]
                        );
                    }
                    else
                    {
                        gameObjectComponentPositions[gameObjectFileID] =
                            oldPositions[component.FileID];
                    }
                }

                (
                    uint Group,
                    FileID? ParentFileID,
                    long GroupPosition,
                    long OldPosition
                ) GetSortKey(Object @object)
                {
                    Component? objectAsComponent = @object as Component;
                    int oldPosition = oldPositions[@object.FileID];

                    if (this.otherObjectsSet.Contains(@object))
                    {
                        return (0, null, -1, oldPosition);
                    }
                    // Groups for all `GameObject`s, including those instantiated by
                    // `PrefabInstance`s, ordered by `fileID`, ascending.
                    // Groups for non-`PrefabInstance` instantiated `GameObject`s and
                    // `PrefabInstance`s, ordered by `fileID`, ascending.
                    else if (!@object.IsInstance && @object is GameObject)
                    {
                        // The `GameObject`, if not instantiated by a `PrefabInstance` and the group
                        // is for a `GameObject`.
                        return (1, @object.FileID, -1, -1);
                    }
                    else if (!@object.IsInstance &&
                             objectAsComponent != null &&
                             !(objectAsComponent.GameObject?.IsInstance ?? false))
                    {
                        // The `GameObject`'s components in `m_Component` order.
                        int componentPosition = componentPositions[@object.FileID];
                        return (1, objectAsComponent.GameObject?.FileID, -1, componentPosition);
                    }
                    else if (!@object.IsInstance &&
                             objectAsComponent != null &&
                             (objectAsComponent.GameObject?.IsInstance ?? false))
                    {
                        // This `Component` has been added to a `PrefabInstance`-instantiated
                        // `GameObject`.
                        // `m_Component` order is determined by `Component` `fileID`s, which
                        // have changed, so use `newOrderingHint` through `oldPosition`.
                        if (componentsOrderedByGameObjectFileIDAscending)
                        {
                            // The `GameObject`'s components in `m_Component` order.
                            return (1, objectAsComponent.GameObject?.FileID, -1, oldPosition);
                        }
                        else
                        {
                            // Groups for the `PrefabInstance`'s `GameObject` instances, ordered in
                            // an unknown way, if the group is for a `PrefabInstance`.
                            // The `GameObject`'s components in `m_Component` order.
                            int gameObjectComponentPosition =
                                gameObjectComponentPositions[objectAsComponent.GameObject.FileID];
                            return (
                                1,
                                objectAsComponent.GameObject?.PrefabInstance?.FileID,
                                gameObjectComponentPosition,
                                oldPosition
                            );
                        }
                    }
                    // Groups for `PrefabInstance`s, ordered by `fileID`, ascending.
                    else if (!@object.IsInstance && @object is PrefabInstance)
                    {
                        // The `PrefabInstance`.
                        return (2, @object.FileID, -1, -1);
                    }
                    else if (@object.IsInstance)
                    {
                        // The `PrefabInstance`'s instantiated objects, ordered in an unknown way.
                        return (2, @object.PrefabInstance?.FileID, -1, oldPosition);
                    }
                    else
                    {
                        return (1, null, -1, oldPosition);
                    }
                }

                this.objects.Sort(
                    (a, b) =>
                    {
                        (uint, FileID?, long, long) aSortKey = GetSortKey(a);
                        (uint, FileID?, long, long) bSortKey = GetSortKey(b);
                        return aSortKey.CompareTo(bSortKey);
                    }
                );
            }

            // Reorder other lists
            this.UpdateAuxiliaryObjectListOrdering();
        }
    }
}
