using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.UnityPrefabFileIDSelfRebase.Tests.Program
{
    using Program = notcake.Unity.UnityPrefabFileIDSelfRebase.Program;

    /// <summary>
    ///     Tests for <see cref="notcake.Unity.UnityPrefabFileIDSelfRebase"/>.
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
    public class Tests
    {
        /// <summary>
        ///     Tests that a prefab file's <c>fileID</c>s can be remapped to themselves losslessly.
        /// </summary>
        /// <param name="path">The path to the prefab file.</param>
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
        public void SelfSelfRebase(string path)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { path, path },
                testConsole
            );

            Assert.AreEqual(File.ReadAllText(path), testConsole.Out.ToString());
            Assert.AreEqual("", testConsole.Error.ToString());
            Assert.AreEqual(0, exitCode);
        }

        /// <summary>
        ///     Tests that a prefab file's <c>fileID</c>s can be remapped correctly.
        /// </summary>
        /// <param name="sourcePath">The path to the source prefab file.</param>
        /// <param name="destinationPath">The path to the destination prefab file.</param>
        /// <param name="expectedOutputPath">
        ///     The path to the <c>.prefab</c> file that contains the expected output.
        /// </param>
        [DataTestMethod]
        [DataRow(
            "Resources/NestedPrefab3.prefab",
            "Resources/NestedPrefab3a.prefab",
            "Resources/NestedPrefab3.prefab"
        )]
        [DataRow(
            "Resources/NestedPrefab3a.prefab",
            "Resources/NestedPrefab3.prefab",
            "Resources/NestedPrefab3a.prefab"
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab"
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab"
        )]
        [DataRow(
            "Resources/ReorderedComponentsA.prefab",
            "Resources/ReorderedComponentsB.prefab",
            "Resources/ReorderedComponentsAReordered.prefab"
        )]
        [DataRow(
            "Resources/ReorderedComponentsB.prefab",
            "Resources/ReorderedComponentsA.prefab",
            "Resources/ReorderedComponentsBReordered.prefab"
        )]
        public void SelfRebase(string sourcePath, string destinationPath, string expectedOutputPath)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { sourcePath, destinationPath },
                testConsole
            );

            Assert.AreEqual(File.ReadAllText(expectedOutputPath), testConsole.Out.ToString());
            Assert.AreEqual("", testConsole.Error.ToString());
            Assert.AreEqual(0, exitCode);
        }
    }
}
