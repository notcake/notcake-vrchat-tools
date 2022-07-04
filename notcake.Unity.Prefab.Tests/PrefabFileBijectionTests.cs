using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Prefab.Tests
{
    /// <summary>
    ///     Tests for the <see cref="PrefabFileBijection"/> class.
    /// </summary>
    [TestClass]
    [DeploymentItem("Resources/NestedPrefab1.prefab")]
    [DeploymentItem("Resources/NestedPrefab2.prefab")]
    [DeploymentItem("Resources/NestedPrefab3.prefab")]
    [DeploymentItem("Resources/NestedPrefab3a.prefab")]
    [DeploymentItem("Resources/GameObject.prefab")]
    [DeploymentItem("Resources/PrefabVariant.prefab")]
    [DeploymentItem("Resources/GameObjectAndPrefabInstanceComponentsA.prefab")]
    [DeploymentItem("Resources/GameObjectAndPrefabInstanceComponentsB.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsA.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsAReordered.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsB.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsBReordered.prefab")]
    [DeploymentItem("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
    [DeploymentItem("Resources/Avatars/Toastacuga.prefab")]
    public class PrefabFileBijectionTests
    {
        /// <summary>
        ///     Tests that the Unity object mapping between a prefab file and itself is correctly
        ///     computed.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab")]
        [DataRow("Resources/NestedPrefab2.prefab")]
        [DataRow("Resources/NestedPrefab3.prefab")]
        [DataRow("Resources/NestedPrefab3a.prefab")]
        [DataRow("Resources/GameObject.prefab")]
        [DataRow("Resources/PrefabVariant.prefab")]
        [DataRow("Resources/GameObjectAndPrefabInstanceComponentsA.prefab")]
        [DataRow("Resources/GameObjectAndPrefabInstanceComponentsB.prefab")]
        [DataRow("Resources/ReorderedComponentsA.prefab")]
        [DataRow("Resources/ReorderedComponentsAReordered.prefab")]
        [DataRow("Resources/ReorderedComponentsB.prefab")]
        [DataRow("Resources/ReorderedComponentsBReordered.prefab")]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
        [DataRow("Resources/Avatars/Toastacuga.prefab")]
        public void SelfBijection(string path)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile prefabFile = PrefabFile.Deserialize(prefabContents);
            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(prefabFile, prefabFile);

            // The `PrefabFileBijection` must be exhaustive.
            Assert.AreEqual(prefabFile.Objects.Count, prefabFileBijection.Count);

            foreach (Object @object in prefabFile.Objects)
            {
                Assert.AreEqual(@object, prefabFileBijection.MapLeftToRight(@object));
                Assert.AreEqual(@object, prefabFileBijection.MapRightToLeft(@object));
            }
        }

        /// <summary>
        ///     Tests that the Unity object mapping between a prefab file and a clone of itself is
        ///     correctly computed.
        /// </summary>
        /// <param name="path">The path to the <c>.prefab</c> file to load as input.</param>
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab1.prefab")]
        [DataRow("Resources/NestedPrefab2.prefab")]
        [DataRow("Resources/NestedPrefab3.prefab")]
        [DataRow("Resources/NestedPrefab3a.prefab")]
        [DataRow("Resources/GameObject.prefab")]
        [DataRow("Resources/PrefabVariant.prefab")]
        [DataRow("Resources/GameObjectAndPrefabInstanceComponentsA.prefab")]
        [DataRow("Resources/GameObjectAndPrefabInstanceComponentsB.prefab")]
        [DataRow("Resources/ReorderedComponentsA.prefab")]
        [DataRow("Resources/ReorderedComponentsAReordered.prefab")]
        [DataRow("Resources/ReorderedComponentsB.prefab")]
        [DataRow("Resources/ReorderedComponentsBReordered.prefab")]
        [DataRow("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
        [DataRow("Resources/Avatars/Toastacuga.prefab")]
        public void CloneBijection(string path)
        {
            string prefabContents = File.ReadAllText(path);
            PrefabFile leftPrefabFile  = PrefabFile.Deserialize(prefabContents);
            PrefabFile rightPrefabFile = PrefabFile.Deserialize(prefabContents);
            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(leftPrefabFile, rightPrefabFile);

            // The `PrefabFileBijection` must be exhaustive.
            Assert.AreEqual(leftPrefabFile.Objects.Count,  prefabFileBijection.Count);
            Assert.AreEqual(rightPrefabFile.Objects.Count, prefabFileBijection.Count);

            foreach (Object leftObject in leftPrefabFile.Objects)
            {
                Assert.AreEqual(
                    rightPrefabFile.GetObjectByFileID(leftObject.FileID),
                    prefabFileBijection.MapLeftToRight(leftObject)
                );
            }

            foreach (Object rightObject in rightPrefabFile.Objects)
            {
                Assert.AreEqual(
                    leftPrefabFile.GetObjectByFileID(rightObject.FileID),
                    prefabFileBijection.MapRightToLeft(rightObject)
                );
            }
        }

        /// <summary>
        ///     Tests that the Unity object mapping between two prefab files is correctly computed.
        /// </summary>
        /// <param name="leftPath">
        ///     The path to the left <c>.prefab</c> file to load as input.
        /// </param>
        /// <param name="rightPath">
        ///     The path to the right <c>.prefab</c> file to load as input.
        /// </param>
        /// <param name="fileIDMapping">
        ///     The expected mappings between <c>fileID</c>s in the left prefab file and
        ///     <c>fileID</c>s in the right prefab file, as a flattened array of <c>fileID</c>
        ///     pairs.
        /// </param>
        [DataTestMethod]
        [DataRow(
            "Resources/NestedPrefab2.prefab",
            "Resources/NestedPrefab3.prefab",
            new object?[]
            {
                4989725120814738964, 4989725120814738964, // GameObject: Nested Prefab 2 / 3
                8820928727568499859, 8820928727568499859, // Transform
                4156974465077255250,  384786867856306869, // PrefabInstance: Child 1
                4889415719643437249, 9168539870313657894, // Transform
                4431041678912474380,  476074792185059992, // PrefabInstance: Child 2
                5121722109705652639, 9003083168696704523, // Transform
                               null, 2786369713623583212, // Transform
                               null, 5073144092867333913, // GameObject: Nested Inner Child
                               null, 1912756242714561364, // Transform
            }
        )]
        [DataRow(
            "Resources/NestedPrefab3.prefab",
            "Resources/NestedPrefab3a.prefab",
            new object?[]
            {
                4989725120814738964, 6293954053335516682, // GameObject: Nested Prefab 3 / 3a
                8820928727568499859, 7497849768378543245, // Transform
                 384786867856306869, 1670981476826454699, // PrefabInstance: Child 1
                9168539870313657894, 7879820769096490552, // Transform
                 476074792185059992, 1512319774972293766, // PrefabInstance: Child 2
                9003083168696704523, 7968817328979179029, // Transform
                2786369713623583212, 3804593009358545394, // Transform
                5073144092867333913, 6053085283548292871, // GameObject: Nested Inner Child
                1912756242714561364,  643158201123562314, // Transform
            }
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab",
            new object?[]
            {
                1295435386291451790, 3703353320363238006, // GameObject: GameObjectAndPrefabInstanceComponents
                6900024346194483700, 9034067136953502732, // Transform
                3246267058468154075, 1122439843563388707, // GameObject: Child 1
                 446762813002330973, 2642913480352949925, // Transform
                2445294676174449731,  248213834871142843, // Event System
                8101453579712206767, 5976451239843250775, // Event System
                1130121813463608664, 3256177755652880544, // GameObject: Child 2
                9040806159793680495, 6911300203454047639, // Transform
                1527149206699838427, 4012493667704102435, // Event System
                3122549251109195932,  705726316260509028, // Event System
                3053240162318261203,  631051950086114859, // PrefabInstance: PrefabInstance 1
                7007784988793393148, 4890378088392989188, // GameObject: PrefabInstance 1
                7025730173712034165, 4836536120345214093, // Transform
                7167184517858377455, 4749133767824403223, // Event System
                9148366837940496496, 6659483049433300360, // Event System
                6440009496554267807, 8917471562729173351, // PrefabInstance: PrefabInstance 2
                1315169930923323568, 3521489118299776328, // GameObject: PrefabInstance 2
                1333244823837382201, 3467235967246025665, // Transform
                6864714582965902295, 9068813882046058031, // Event System
                4575199936328865583, 2152962228995310295, // Event System
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsA.prefab",
            "Resources/ReorderedComponentsB.prefab",
            new object?[]
            {
                2853174732634746890, 7097020843408076707, // GameObject: ReorderedComponents
                6356941340319174409, 2151957425077981344, // Transform
                6968487965431459191, 2688692364277560030, // Event System
                4797569303984820519,  536295912774078094, // Event System
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsAReordered.prefab",
            "Resources/ReorderedComponentsBReordered.prefab",
            new object?[]
            {
                2853174732634746890, 7097020843408076707, // GameObject: ReorderedComponents
                6356941340319174409, 2151957425077981344, // Transform
                6968487965431459191, 2688692364277560030, // Event System
                4797569303984820519,  536295912774078094, // Event System
            }
        )]
        public void Bijection(string leftPath, string rightPath, object?[] fileIDMapping)
        {
            string leftPrefabContents  = File.ReadAllText(leftPath);
            string rightPrefabContents = File.ReadAllText(rightPath);
            PrefabFile leftPrefabFile  = PrefabFile.Deserialize(leftPrefabContents);
            PrefabFile rightPrefabFile = PrefabFile.Deserialize(rightPrefabContents);
            PrefabFileBijection prefabFileBijection =
                PrefabFileBijection.Compute(leftPrefabFile, rightPrefabFile);

            for (int i = 0; i < fileIDMapping.Length; i += 2)
            {
                Object? leftObject = fileIDMapping[i] is long leftFileID ?
                    leftPrefabFile.GetObjectByFileID(new FileID(leftFileID)) :
                    null;
                Object? rightObject = fileIDMapping[i + 1] is long rightFileID ?
                    rightPrefabFile.GetObjectByFileID(new FileID(rightFileID)) :
                    null;

                if (leftObject != null)
                {
                    Assert.AreEqual(rightObject, prefabFileBijection.MapLeftToRight(leftObject));
                }

                if (rightObject != null)
                {
                    Assert.AreEqual(leftObject, prefabFileBijection.MapRightToLeft(rightObject));
                }
            }
        }
    }
}
