using System;
using System.CommandLine;
using System.CommandLine.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.UnityPrefabFileIDSelfRebase.Tests.Program
{
    using Program = notcake.Unity.UnityPrefabFileIDSelfRebase.Program;

    /// <summary>
    ///     Tests for <see cref="notcake.Unity.UnityPrefabFileIDSelfRebase"/>'s handling of
    ///     unwritable file paths.
    /// </summary>
    [TestClass]
    public class UnwritableFilePathTests :
        notcake.Unity.Prefab.Tests.Resources.UnwritableFilePathTests
    {
        [ClassInitialize]
        public new static void ClassInitialize(TestContext testContext)
        {
            notcake.Unity.Prefab.Tests.Resources.UnwritableFilePathTests.ClassInitialize(testContext);
        }

        [ClassCleanup]
        public new static void ClassCleanup()
        {
            notcake.Unity.Prefab.Tests.Resources.UnwritableFilePathTests.ClassCleanup();
        }

        /// <summary>
        ///     Tests the program with an unwritable output prefab file path.
        /// </summary>
        /// <param name="path">The unopenable left prefab file path.</param>
        /// <param name="error">
        ///     The expected error message, excluding the trailing line break.
        /// </param>
        [DataTestMethod]
        [DataRow(
            UnwritableFilePathTests.DirectoryPrefabPath,
            $"Expected \"{UnwritableFilePathTests.DirectoryPrefabPath}\" to be a prefab file, " +
            "but found a directory."
        )]
        [DataRow(
            UnwritableFilePathTests.NonExistentDirectoryPrefabPath,
            "The directory containing " +
            $"\"{UnwritableFilePathTests.NonExistentDirectoryPrefabPath}\" does not exist."
        )]
        [DataRow(
            UnwritableFilePathTests.ExclusiveAccessPrefabPath,
            $"Cannot write \"{UnwritableFilePathTests.ExclusiveAccessPrefabPath}\" because it is " +
            "locked by another process."
        )]
        [DataRow(
            UnwritableFilePathTests.NoWritePermissionPrefabPath,
            $"Cannot write \"{UnwritableFilePathTests.NoWritePermissionPrefabPath}\" because of " +
            "filesystem permissions."
        )]
        public void UnwritableOutputPrefabFilePath(string path, string error)
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[]
                {
                    "Resources/NestedPrefab3.prefab",
                    "Resources/NestedPrefab3.prefab",
                    "--output", path,
                },
                testConsole
            );

            Assert.AreEqual("", testConsole.Out.ToString());
            Assert.AreEqual(error + Environment.NewLine, testConsole.Error.ToString());
            Assert.AreEqual(1, exitCode);
        }
    }
}
