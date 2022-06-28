using System;
using System.Diagnostics.CodeAnalysis;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Represents a Unity <c>fileID</c>.
    /// </summary>
    public struct FileID : IEquatable<FileID>, IComparable<FileID>
    {
        /// <summary>
        ///     Gets the <c>fileID</c> as a signed 64-bit integer.
        /// </summary>
        public long Value { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileID"/> struct.
        /// </summary>
        /// <param name="fileID">The <c>fileID</c> as a signed 64-bit integer.</param>
        public FileID(long fileID)
        {
            this.Value = fileID;
        }

        #region object
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not FileID fileID) { return false; }

            return this.Equals(fileID);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
        #endregion

        #region IEquatable<FileID>
        public bool Equals(FileID other)
        {
            return this.Value == other.Value;
        }
        #endregion

        #region IComparable<FileID>
        public int CompareTo(FileID other)
        {
            return this.Value.CompareTo(other.Value);
        }
        #endregion

        #region FileID
        /// <summary>
        ///     Computes the <see cref="FileID"/> of the Unity object when instantiated by a given
        ///     <c>PrefabInstance</c>.
        /// </summary>
        /// <param name="prefabInstanceFileID">
        ///     The <see cref="FileID"/> of the <c>PrefabInstance</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="FileID"/> of the Unity object instance.
        /// </returns>
        public FileID Instantiate(FileID prefabInstanceFileID)
        {
            long fileID = this.Value;
            fileID ^= prefabInstanceFileID.Value;
            fileID &= 0x7FFFFFFF_FFFFFFFF;
            return new FileID(fileID);
        }

        /// <summary>
        ///     Computes the possible <see cref="FileID">FileIDs</see> of the Unity object's
        ///     template from an object instantiated by a given <c>PrefabInstance</c>.
        /// </summary>
        /// <param name="prefabInstanceFileID">
        ///     The <see cref="FileID"/> of the <c>PrefabInstance</c>.
        /// </param>
        /// <returns>
        ///     A tuple containing the possible positive <see cref="FileID"/> of the Unity object's
        ///     template, followed by the possible negative <see cref="FileID"/>.
        ///     <para/>
        ///     If the Unity object's template is another object instance, its <see cref="FileID"/>
        ///     must be the positive one.
        /// </returns>
        public (FileID, FileID) Uninstantiate(FileID prefabInstanceFileID)
        {
            long fileID = this.Value;
            fileID ^= prefabInstanceFileID.Value;
            fileID &= 0x7FFFFFFF_FFFFFFFF;
            return (new FileID(fileID), new FileID(fileID | -0x80000000_00000000L));
        }

        /// <summary>
        ///     Computes the <see cref="FileID"/> of the Unity object when instantiated by a
        ///     <c>PrefabInstance</c> with a different <see cref="FileID"/>.
        /// </summary>
        /// <param name="oldPrefabInstanceFileID">
        ///     The <see cref="FileID"/> of the old <c>PrefabInstance</c>.
        /// </param>
        /// <param name="newPrefabInstanceFileID">
        ///     The <see cref="FileID"/> of the new <c>PrefabInstance</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="FileID"/> of the Unity object instance when instantiated under the
        ///     <see cref="FileID"/> of the new <c>PrefabInstance</c>.
        /// </returns>
        public FileID Reinstantiate(FileID oldPrefabInstanceFileID, FileID newPrefabInstanceFileID)
        {
            long fileID = this.Value;
            fileID ^= oldPrefabInstanceFileID.Value;
            fileID ^= newPrefabInstanceFileID.Value;
            fileID &= 0x7FFFFFFF_FFFFFFFF;
            return new FileID(fileID);
        }
        #endregion

        public static bool operator ==(FileID left, FileID right) { return left.Value == right.Value; }
        public static bool operator !=(FileID left, FileID right) { return left.Value != right.Value; }

        public static bool operator < (FileID left, FileID right) { return left.Value <  right.Value; }
        public static bool operator <=(FileID left, FileID right) { return left.Value <= right.Value; }
        public static bool operator > (FileID left, FileID right) { return left.Value >  right.Value; }
        public static bool operator >=(FileID left, FileID right) { return left.Value >= right.Value; }

        /// <summary>
        ///     Gets the <c>0</c> <see cref="FileID"/>.
        /// </summary>
        public static FileID Zero => new(0);
    }
}
