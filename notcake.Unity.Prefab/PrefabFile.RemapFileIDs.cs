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

            // Identify the object ordering.
            bool isOrderedByFileIDAscending = true;
            for (int i = 1; i < this.objects.Count; i++)
            {
                if (this.objects[i].FileID < this.objects[i - 1].FileID)
                {
                    isOrderedByFileIDAscending = false;
                    break;
                }
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
                Dictionary<FileID, int> oldPositions = new();
                for (int i = 0; i < this.objects.Count; i++)
                {
                    oldPositions[this.objects[i].FileID] = i;
                }

                if (newOrderingHint != null)
                {
                    // Override the ordering using `newOrderingHint`.
                    for (int i = 0; i < newOrderingHint.Count; i++)
                    {
                        oldPositions[newOrderingHint[i]] = i;
                    }
                }

                (uint Group, FileID? ParentFileID, long OldPosition) GetSortKey(Object @object)
                {
                    Component? objectAsComponent = @object as Component;
                    int oldPosition = oldPositions[@object.FileID];

                    if (this.otherObjectsSet.Contains(@object))
                    {
                        return (0, null, oldPosition);
                    }
                    else if (!@object.IsInstance && @object is GameObject)
                    {
                        // `GameObjects` ordered by `fileID`.
                        return (1, @object.FileID, -1);
                    }
                    else if (!@object.IsInstance &&
                             objectAsComponent != null &&
                             !(objectAsComponent.GameObject?.IsInstance ?? false))
                    {
                        // Per-`GameObject` components in `m_Component` order.
                        return (1, objectAsComponent.GameObject?.FileID, oldPosition);
                    }
                    else if (!@object.IsInstance &&
                             objectAsComponent != null &&
                             (objectAsComponent.GameObject?.IsInstance ?? false))
                    {
                        // `Component`s added to `GameObject`s that have been instantiated by
                        // `PrefabInstance`s, ordered in an unknown way.
                        return (2, null, oldPosition);
                    }
                    else if (!@object.IsInstance && @object is PrefabInstance)
                    {
                        // `PrefabInstance`s ordered by `fileID`, ascending.
                        return (3, @object.FileID, -1);
                    }
                    else if (@object.IsInstance)
                    {
                        // Per-`PrefabInstance` object instances ordered in an unknown way.
                        return (3, @object.PrefabInstance?.FileID, oldPosition);
                    }
                    else
                    {
                        return (2, null, oldPosition);
                    }
                }

                this.objects.Sort(
                    (a, b) =>
                    {
                        (uint, FileID?, long) aSortKey = GetSortKey(a);
                        (uint, FileID?, long) bSortKey = GetSortKey(b);
                        return aSortKey.CompareTo(bSortKey);
                    }
                );
            }

            // Reorder other lists
            this.UpdateAuxiliaryObjectListOrdering();
        }
    }
}
