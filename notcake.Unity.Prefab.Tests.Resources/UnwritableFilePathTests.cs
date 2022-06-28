using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Prefab.Tests.Resources
{
    /// <summary>
    ///     Provides unwritable prefab file paths for tests.
    /// </summary>
    [TestClass]
    public class UnwritableFilePathTests : InaccessibleFilePathTests
    {
        private static int initializationCount = 0;

        /// <summary>
        ///     The path to a prefab file whose permissions do not allow writing.
        /// </summary>
        protected const string NoWritePermissionPrefabPath = "Resources/UnwritablePrefab.prefab";

        [ClassInitialize]
        public new static void ClassInitialize(TestContext testContext)
        {
            int initializationCount =
                Interlocked.Increment(ref UnwritableFilePathTests.initializationCount);
            if (initializationCount != 1) { return; }

            InaccessibleFilePathTests.ClassInitialize(testContext);

            FileInfo fileInfo = new(UnwritableFilePathTests.NoWritePermissionPrefabPath);
            FileStream fileStream = File.Open(
                UnwritableFilePathTests.NoWritePermissionPrefabPath,
                FileMode.Create,
                FileAccess.Write
            );
            fileStream.Close();
            fileInfo.IsReadOnly = true;
        }

        [ClassCleanup]
        public new static void ClassCleanup()
        {
            int initializationCount =
                Interlocked.Decrement(ref UnwritableFilePathTests.initializationCount);
            if (initializationCount != 0) { return; }

            FileInfo fileInfo = new(UnwritableFilePathTests.NoWritePermissionPrefabPath);
            fileInfo.IsReadOnly = false;
            File.Delete(UnwritableFilePathTests.NoWritePermissionPrefabPath);

            InaccessibleFilePathTests.ClassCleanup();
        }
    }
}
