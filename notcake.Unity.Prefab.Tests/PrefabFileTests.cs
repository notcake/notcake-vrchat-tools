using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Prefab.Tests
{
    /// <summary>
    ///     Tests for the <see cref="PrefabFile"/> class.
    /// </summary>
    [TestClass]
    [DeploymentItem("Resources/NestedPrefab1.prefab")]
    [DeploymentItem("Resources/NestedPrefab2.prefab")]
    [DeploymentItem("Resources/NestedPrefab3.prefab")]
    [DeploymentItem("Resources/NestedPrefab3a.prefab")]
    [DeploymentItem("Resources/PrefabVariant1.prefab")]
    [DeploymentItem("Resources/PrefabVariant2.prefab")]
    [DeploymentItem("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
    [DeploymentItem("Resources/Avatars/Toastacuga.prefab")]
    public class PrefabFileTests
    {
        /// <summary>
        ///     Tests that <c>.prefab</c> files can be round tripped.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab")]
        [DataRow("Resources/NestedPrefab2.prefab")]
        [DataRow("Resources/NestedPrefab3.prefab")]
        [DataRow("Resources/NestedPrefab3a.prefab")]
        [DataRow("Resources/PrefabVariant1.prefab")]
        [DataRow("Resources/PrefabVariant2.prefab")]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
        [DataRow("Resources/Avatars/Toastacuga.prefab")]
        public void RoundTrip(string path)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            Assert.AreEqual(prefabContents, prefabFile.Serialize());
        }

        /// <summary>
        ///     Tests that <see cref="PrefabFile.OtherObjects"/> in the prefab file are correctly
        ///     identified.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        /// <param name="otherObjects">
        ///     The expected <c>fileID</c>s of <see cref="PrefabFile.OtherObjects"/>.
        /// </param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab",                       new long[] {           })]
        [DataRow("Resources/NestedPrefab2.prefab",                       new long[] {           })]
        [DataRow("Resources/NestedPrefab3.prefab",                       new long[] {           })]
        [DataRow("Resources/NestedPrefab3a.prefab",                      new long[] {           })]
        [DataRow("Resources/PrefabVariant1.prefab",                      new long[] {           })]
        [DataRow("Resources/PrefabVariant2.prefab",                      new long[] {           })]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab", new long[] {           })]
        [DataRow("Resources/Avatars/Toastacuga.prefab",                  new long[] { 100100000 })]
        public void OtherObjects(string path, long[] otherObjects)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            CollectionAssert.AreEqual(
                otherObjects,
                prefabFile.OtherObjects
                    .Select(@object => @object.FileID.Value)
                    .ToArray()
            );
        }

        /// <summary>
        ///     Tests that prefab file roots are correctly identified.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        /// <param name="rootGameObject">
        ///     The expected <c>fileID</c> of the root <c>GameObject</c>.
        /// </param>
        /// <param name="rootTransform">
        ///     The expected <c>fileID</c> of the root <c>GameObject</c>'s <c>Transform</c>.
        /// </param>
        /// <param name="rootPrefabInstance">
        ///     The expected <c>fileID</c> of the root <c>PrefabInstance</c>.
        /// </param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab",                       4989725120814738964, 8820928727568499859,                null)]
        [DataRow("Resources/NestedPrefab2.prefab",                       4989725120814738964, 8820928727568499859,                null)]
        [DataRow("Resources/NestedPrefab3.prefab",                       4989725120814738964, 8820928727568499859,                null)]
        [DataRow("Resources/NestedPrefab3a.prefab",                      6293954053335516682, 7497849768378543245,                null)]
        [DataRow("Resources/PrefabVariant1.prefab",                      5413307586876789807, 5467125436267295398,                null)]
        [DataRow("Resources/PrefabVariant2.prefab",                                     null,                null, 6612481006152796884)]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab",    1485637429502818,    4090137802650142,                null)]
        [DataRow("Resources/Avatars/Toastacuga.prefab",                     1445245993874940,    4851560997859476,                null)]
        public void Roots(
            string path,
            long? rootGameObject,
            long? rootTransform,
            long? rootPrefabInstance
        )
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            Assert.AreEqual(rootGameObject,     prefabFile.RootGameObject?.FileID.Value);
            Assert.AreEqual(rootTransform,      prefabFile.RootTransform?.FileID.Value);
            Assert.AreEqual(rootPrefabInstance, prefabFile.RootPrefabInstance?.FileID.Value);
        }

        /// <summary>
        ///     Tests that <see cref="PrefabFile.OrphanObjects"/> in the prefab file are correctly
        ///     identified.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        /// <param name="orphanObjects">
        ///     The expected <c>fileID</c>s of <see cref="PrefabFile.OrphanObjects"/>.
        /// </param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab",                       new long[] { })]
        [DataRow("Resources/NestedPrefab2.prefab",                       new long[] { })]
        [DataRow("Resources/NestedPrefab3.prefab",                       new long[] { })]
        [DataRow("Resources/NestedPrefab3a.prefab",                      new long[] { })]
        [DataRow("Resources/PrefabVariant1.prefab",                      new long[] { })]
        [DataRow("Resources/PrefabVariant2.prefab",                      new long[] { })]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab", new long[] { })]
        [DataRow("Resources/Avatars/Toastacuga.prefab",                  new long[] { })]
        public void OrphanObjects(string path, long[] orphanObjects)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            CollectionAssert.AreEqual(
                orphanObjects,
                prefabFile.OrphanObjects
                    .Select(@object => @object.FileID.Value)
                    .ToArray()
            );
        }

        /// <summary>
        ///     Tests that a prefab file's <c>fileID</c>s can be remapped to themselves losslessly.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab")]
        [DataRow("Resources/NestedPrefab2.prefab")]
        [DataRow("Resources/NestedPrefab3.prefab")]
        [DataRow("Resources/NestedPrefab3a.prefab")]
        [DataRow("Resources/PrefabVariant1.prefab")]
        [DataRow("Resources/PrefabVariant2.prefab")]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
        [DataRow("Resources/Avatars/Toastacuga.prefab")]
        public void SelfRemapFileIDs(string path)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(prefabFile, prefabFile);

            prefabFile.RemapFileIDs(
                prefabFileBijection.ToLeftToRightFileIDMapping(),
                prefabFile.Objects.Select(@object => @object.FileID).ToList()
            );
            Assert.AreEqual(prefabContents, prefabFile.Serialize());
        }

        /// <summary>
        ///     Tests that a prefab file's <c>fileID</c>s can be remapped correctly.
        /// </summary>
        [TestMethod]
        public void RemapFileIDs()
        {
            string leftPrefabContents  = File.ReadAllText("Resources/NestedPrefab3.prefab");
            string rightPrefabContents = File.ReadAllText("Resources/NestedPrefab3a.prefab");
            PrefabFile leftPrefabFile  = PrefabFile.Deserialize(leftPrefabContents);
            PrefabFile rightPrefabFile = PrefabFile.Deserialize(rightPrefabContents);
            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(leftPrefabFile, rightPrefabFile);

            Dictionary<FileID, FileID> leftToRightFileIDMapping =
                prefabFileBijection.ToLeftToRightFileIDMapping();
            Dictionary<FileID, FileID> rightToLeftFileIDMapping =
                prefabFileBijection.ToRightToLeftFileIDMapping();
            List<FileID> leftPrefabOrdering =
                leftPrefabFile.Objects.Select(@object => @object.FileID).ToList();
            List<FileID> rightPrefabOrdering =
                rightPrefabFile.Objects.Select(@object => @object.FileID).ToList();

            leftPrefabFile.RemapFileIDs(leftToRightFileIDMapping, rightPrefabOrdering);
            Assert.AreEqual(rightPrefabContents, leftPrefabFile.Serialize());

            rightPrefabFile.RemapFileIDs(rightToLeftFileIDMapping, leftPrefabOrdering);
            Assert.AreEqual(leftPrefabContents, rightPrefabFile.Serialize());
        }
    }
}
