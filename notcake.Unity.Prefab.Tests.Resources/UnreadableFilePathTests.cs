using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Unix;

namespace notcake.Unity.Prefab.Tests.Resources
{
    /// <summary>
    ///     Provides unreadable prefab file paths for tests.
    /// </summary>
    [TestClass]
    [DeploymentItem(UnreadableFilePathTests.InvalidTagPrefabPath)]
    [DeploymentItem(UnreadableFilePathTests.InvalidYamlPrefabPath)]
    public class UnreadableFilePathTests : InaccessibleFilePathTests
    {
        private static int initializationCount = 0;

        /// <summary>
        ///     The path to a non-existent prefab file.
        /// </summary>
        protected const string NonExistentPrefabPath = "Resources/NonExistentPrefab.prefab";

        /// <summary>
        ///     The path to a prefab file whose permissions do not allow reading.
        /// </summary>
        protected const string NoReadPermissionPrefabPath = "Resources/UnreadablePrefab.prefab";

        /// <summary>
        ///     The path to a prefab file containing an invalid tag.
        /// </summary>
        protected const string InvalidTagPrefabPath = "Resources/InvalidTag.prefab";

        /// <summary>
        ///     The path to a prefab file containing invalid YAML.
        /// </summary>
        protected const string InvalidYamlPrefabPath = "Resources/InvalidYAML.prefab";

        [ClassInitialize]
        public new static void ClassInitialize(TestContext testContext)
        {
            int initializationCount =
                Interlocked.Increment(ref UnreadableFilePathTests.initializationCount);
            if (initializationCount != 1) { return; }

            InaccessibleFilePathTests.ClassInitialize(testContext);

            FileInfo fileInfo = new(UnreadableFilePathTests.NoReadPermissionPrefabPath);
            FileStream fileStream = File.Open(
                UnreadableFilePathTests.NoReadPermissionPrefabPath,
                FileMode.Create,
                FileAccess.Write
            );
            fileStream.Close();

            if (OperatingSystem.IsWindows())
            {
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                SecurityIdentifier everyone = new(WellKnownSidType.WorldSid, null);
                fileSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        everyone,
                        FileSystemRights.Read,
                        AccessControlType.Deny
                    )
                );
                fileInfo.SetAccessControl(fileSecurity);
            }
            else
            {
                UnixFileInfo unixFileInfo = new(UnreadableFilePathTests.NoReadPermissionPrefabPath);
                unixFileInfo.FileAccessPermissions &= ~(
                    FileAccessPermissions.UserRead |
                    FileAccessPermissions.GroupRead |
                    FileAccessPermissions.OtherRead
                );
            }
        }

        [ClassCleanup]
        public new static void ClassCleanup()
        {
            int initializationCount =
                Interlocked.Decrement(ref UnreadableFilePathTests.initializationCount);
            if (initializationCount != 0) { return; }

            File.Delete(UnreadableFilePathTests.NoReadPermissionPrefabPath);

            InaccessibleFilePathTests.ClassCleanup();
        }
    }
}
