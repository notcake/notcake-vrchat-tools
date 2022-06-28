using System;
using System.CommandLine;
using System.CommandLine.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.UnityPrefabFileIDSelfRebase.Tests.Program
{
    using Program = notcake.Unity.UnityPrefabFileIDSelfRebase.Program;

    /// <summary>
    ///     Tests for <see cref="notcake.Unity.UnityPrefabFileIDSelfRebase"/>'s handling of
    ///     unreadable file paths.
    /// </summary>
    [TestClass]
    [DeploymentItem("Resources/NestedPrefab3.prefab")]
    public class UnreadableFilePathTests :
        notcake.Unity.Prefab.Tests.Resources.UnreadableFilePathTests
    {
        [ClassInitialize]
        public new static void ClassInitialize(TestContext testContext)
        {
            notcake.Unity.Prefab.Tests.Resources.UnreadableFilePathTests.ClassInitialize(testContext);
        }

        [ClassCleanup]
        public new static void ClassCleanup()
        {
            notcake.Unity.Prefab.Tests.Resources.UnreadableFilePathTests.ClassCleanup();
        }

        /// <summary>
        ///     Tests the program with an unreadable left prefab file path.
        /// </summary>
        /// <param name="path">The unreadable left prefab file path.</param>
        /// <param name="error">
        ///     The expected error message, excluding the trailing line break.
        /// </param>
        [DataTestMethod]
        [DataRow(
            UnreadableFilePathTests.NonExistentPrefabPath,
            $"Prefab file \"{UnreadableFilePathTests.NonExistentPrefabPath}\" does not exist."
        )]
        [DataRow(
            UnreadableFilePathTests.DirectoryPrefabPath,
            $"Expected \"{UnreadableFilePathTests.DirectoryPrefabPath}\" to be a prefab file, " +
            "but found a directory."
        )]
        [DataRow(
            UnreadableFilePathTests.NonExistentDirectoryPrefabPath,
            "The directory containing " +
            $"\"{UnreadableFilePathTests.NonExistentDirectoryPrefabPath}\" does not exist."
        )]
        [DataRow(
            UnreadableFilePathTests.ExclusiveAccessPrefabPath,
            $"Cannot read \"{UnreadableFilePathTests.ExclusiveAccessPrefabPath}\" because it is " +
            "locked by another process."
        )]
        [DataRow(
            UnreadableFilePathTests.NoReadPermissionPrefabPath,
            $"Cannot read \"{UnreadableFilePathTests.NoReadPermissionPrefabPath}\" because of " +
            "filesystem permissions."
        )]
        [DataRow(
            UnreadableFilePathTests.InvalidTagPrefabPath,
            $"Could not parse \"{UnreadableFilePathTests.InvalidTagPrefabPath}\" because it is " +
            "malformed: Invalid tag: "
        )]
        [DataRow(
            UnreadableFilePathTests.InvalidYamlPrefabPath,
            $"Could not parse \"{UnreadableFilePathTests.InvalidYamlPrefabPath}\" because it is " +
            "malformed: (Line: 2, Col: 1, Idx: 14) - (Line: 2, Col: 1, Idx: 14): " +
            "While parsing a node, did not find expected node content."
        )]
        public void UnreadableSourcePrefabFilePath(string path, string error)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { path, "Resources/NestedPrefab3.prefab" },
                testConsole
            );

            Assert.AreEqual("", testConsole.Out.ToString());
            Assert.AreEqual(error + Environment.NewLine, testConsole.Error.ToString());
            Assert.AreEqual(1, exitCode);
        }

        /// <summary>
        ///     Tests the program with an unreadable right prefab file path.
        /// </summary>
        /// <param name="path">The unreadable right prefab file path.</param>
        /// <param name="error">
        ///     The expected error message, excluding the trailing line break.
        /// </param>
        [DataTestMethod]
        [DataRow(
            UnreadableFilePathTests.NonExistentPrefabPath,
            $"Prefab file \"{UnreadableFilePathTests.NonExistentPrefabPath}\" does not exist."
        )]
        [DataRow(
            UnreadableFilePathTests.DirectoryPrefabPath,
            $"Expected \"{UnreadableFilePathTests.DirectoryPrefabPath}\" to be a prefab file, " +
            "but found a directory."
        )]
        [DataRow(
            UnreadableFilePathTests.NonExistentDirectoryPrefabPath,
            "The directory containing " +
            $"\"{UnreadableFilePathTests.NonExistentDirectoryPrefabPath}\" does not exist."
        )]
        [DataRow(
            UnreadableFilePathTests.ExclusiveAccessPrefabPath,
            $"Cannot read \"{UnreadableFilePathTests.ExclusiveAccessPrefabPath}\" because it is " +
            "locked by another process."
        )]
        [DataRow(
            UnreadableFilePathTests.NoReadPermissionPrefabPath,
            $"Cannot read \"{UnreadableFilePathTests.NoReadPermissionPrefabPath}\" because of " +
            "filesystem permissions."
        )]
        [DataRow(
            UnreadableFilePathTests.InvalidTagPrefabPath,
            $"Could not parse \"{UnreadableFilePathTests.InvalidTagPrefabPath}\" because it is " +
            "malformed: Invalid tag: "
        )]
        [DataRow(
            UnreadableFilePathTests.InvalidYamlPrefabPath,
            $"Could not parse \"{UnreadableFilePathTests.InvalidYamlPrefabPath}\" because it is " +
            "malformed: (Line: 2, Col: 1, Idx: 14) - (Line: 2, Col: 1, Idx: 14): " +
            "While parsing a node, did not find expected node content."
        )]
        public void UnreadableDestinationPrefabFilePath(string path, string error)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { "Resources/NestedPrefab3.prefab", path },
                testConsole
            );

            Assert.AreEqual("", testConsole.Out.ToString());
            Assert.AreEqual(error + Environment.NewLine, testConsole.Error.ToString());
            Assert.AreEqual(1, exitCode);
        }
    }
}
