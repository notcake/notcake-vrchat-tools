using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Prefab.Tests.Resources
{
    /// <summary>
    ///     Provides inaccessible prefab file paths for tests.
    /// </summary>
    [TestClass]
    public class InaccessibleFilePathTests
    {
        private static int initializationCount = 0;

        /// <summary>
        ///     The path to a directory.
        /// </summary>
        protected const string DirectoryPrefabPath = "Resources";

        /// <summary>
        ///     The path to a prefab file within a non-existent firectory.
        /// </summary>
        protected const string NonExistentDirectoryPrefabPath =
            "Resources/NonExistentDirectory/NonExistentPrefab.prefab";

        /// <summary>
        ///     The path to a prefab file which is already open with exclusive access.
        /// </summary>
        protected const string ExclusiveAccessPrefabPath = "Resources/ExclusiveAccessPrefab.prefab";
        private static FileStream? exclusiveAccessFileStream = null;

        [ClassInitialize]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static void ClassInitialize(TestContext testContext)
        {
            int initializationCount =
                Interlocked.Increment(ref InaccessibleFilePathTests.initializationCount);
            if (initializationCount != 1) { return; }

            Assert.IsNull(InaccessibleFilePathTests.exclusiveAccessFileStream);
            InaccessibleFilePathTests.exclusiveAccessFileStream = File.Open(
                InaccessibleFilePathTests.ExclusiveAccessPrefabPath,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None
            );
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            int initializationCount =
                Interlocked.Decrement(ref InaccessibleFilePathTests.initializationCount);
            if (initializationCount != 0) { return; }

            InaccessibleFilePathTests.exclusiveAccessFileStream?.Close();
            InaccessibleFilePathTests.exclusiveAccessFileStream?.Dispose();
            InaccessibleFilePathTests.exclusiveAccessFileStream = null;
            File.Delete(InaccessibleFilePathTests.ExclusiveAccessPrefabPath);
        }
    }
}
