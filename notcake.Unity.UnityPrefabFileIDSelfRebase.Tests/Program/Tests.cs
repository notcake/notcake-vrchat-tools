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
        [DataTestMethod]
        [DataRow("Resources/NestedPrefab3.prefab",  "Resources/NestedPrefab3a.prefab")]
        [DataRow("Resources/NestedPrefab3a.prefab", "Resources/NestedPrefab3.prefab" )]
        public void SelfRebase(string sourcePath, string destinationPath)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { sourcePath, destinationPath },
                testConsole
            );

            Assert.AreEqual(File.ReadAllText(sourcePath), testConsole.Out.ToString());
            Assert.AreEqual("", testConsole.Error.ToString());
            Assert.AreEqual(0, exitCode);
        }
    }
}
