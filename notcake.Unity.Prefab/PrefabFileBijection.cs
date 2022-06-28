using System;
using System.Collections.Generic;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     A one-to-one mapping between the Unity objects in two
    ///     <see cref="PrefabFile">PrefabFiles</see>.
    /// </summary>
    public partial class PrefabFileBijection
    {
        /// <summary>
        ///     Gets the left <see cref="PrefabFile"/> of the <see cref="PrefabFileBijection"/>.
        /// </summary>
        public PrefabFile LeftPrefabFile { get; }

        /// <summary>
        ///     Gets the right <see cref="PrefabFile"/> of the <see cref="PrefabFileBijection"/>.
        /// </summary>
        public PrefabFile RightPrefabFile { get; }

        private readonly Dictionary<Object, Object> leftToRight = new();
        private readonly Dictionary<Object, Object> rightToLeft = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrefabFileBijection"/> class.
        /// </summary>
        /// <param name="leftPrefabFile">The left <see cref="PrefabFile"/> of the bijection.</param>
        /// <param name="rightPrefabFile">
        ///     The right <see cref="PrefabFile"/> of the bijection.
        /// </param>
        public PrefabFileBijection(PrefabFile leftPrefabFile, PrefabFile rightPrefabFile)
        {
            this.LeftPrefabFile  = leftPrefabFile;
            this.RightPrefabFile = rightPrefabFile;
        }

        /// <summary>
        ///     Gets the number of Unity object mappings in the bijection.
        /// </summary>
        public int Count => this.leftToRight.Count;

        /// <summary>
        ///     Adds a new mapping to the bijection.
        /// </summary>
        /// <param name="leftObject">
        ///     The Unity object to map to <paramref name="rightObject"/>.
        /// </param>
        /// <param name="rightObject">
        ///     The Unity object to map to <paramref name="leftObject"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="leftObject"/> or <paramref name="rightObject"/> are not
        ///     contained in <see cref="LeftPrefabFile"/> or <see cref="RightPrefabFile"/>
        ///     respectively.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <paramref name="leftObject"/> or <paramref name="rightObject"/> already
        ///     have an existing mapping.
        /// </exception>
        public void Add(Object leftObject, Object rightObject)
        {
            if (!this.LeftPrefabFile.Contains(leftObject))
            {
                throw new ArgumentOutOfRangeException(nameof(leftObject));
            }

            if (!this.RightPrefabFile.Contains(rightObject))
            {
                throw new ArgumentOutOfRangeException(nameof(rightObject));
            }

            if (this.leftToRight.ContainsKey(leftObject) ||
                this.rightToLeft.ContainsKey(rightObject))
            {
                throw new InvalidOperationException();
            }

            this.leftToRight[leftObject] = rightObject;
            this.rightToLeft[rightObject] = leftObject;
        }

        /// <summary>
        ///     Determines whether the given Unity object from the left <see cref="PrefabFile"/> has
        ///     a corresponding Unity object in the right <see cref="PrefabFile"/>.
        /// </summary>
        /// <param name="leftObject">The Unity object in the left <see cref="PrefabFile"/>.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="leftObject"/> has a corresponding Unity object in
        ///     the right <see cref="PrefabFile"/>;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool ContainsLeft(Object leftObject)
        {
            return this.leftToRight.ContainsKey(leftObject);
        }

        /// <summary>
        ///     Determines whether the given Unity object from the right <see cref="PrefabFile"/>
        ///     has a corresponding Unity object in the left <see cref="PrefabFile"/>.
        /// </summary>
        /// <param name="rightObject">
        ///     The Unity object in the right <see cref="PrefabFile"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="rightObject"/> has a corresponding Unity object in
        ///     the left <see cref="PrefabFile"/>;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool ContainsRight(Object rightObject)
        {
            return this.rightToLeft.ContainsKey(rightObject);
        }

        /// <summary>
        ///     Maps a Unity object from the left <see cref="PrefabFile"/> to the corresponding
        ///     Unity object in the right <see cref="PrefabFile"/>.
        /// </summary>
        /// <param name="leftObject">
        ///     The Unity object in the left <see cref="PrefabFile"/> to map to the right
        ///     <see cref="PrefabFile"/>.
        /// </param>
        /// <returns>
        ///     The corresponding Unity object in the right <see cref="PrefabFile"/>, if it
        ///     exists;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public Object? MapLeftToRight(Object leftObject)
        {
            return this.leftToRight.TryGetValue(leftObject, out Object? rightObject) ?
                rightObject :
                null;
        }

        /// <summary>
        ///     Maps a Unity object from the right <see cref="PrefabFile"/> to the corresponding
        ///     Unity object in the left <see cref="PrefabFile"/>.
        /// </summary>
        /// <param name="rightObject">
        ///     The Unity object in the right <see cref="PrefabFile"/> to map to the left
        ///     <see cref="PrefabFile"/>.
        /// </param>
        /// <returns>
        ///     The corresponding Unity object in the left <see cref="PrefabFile"/>, if it
        ///     exists;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public Object? MapRightToLeft(Object rightObject)
        {
            return this.rightToLeft.TryGetValue(rightObject, out Object? leftObject) ?
                leftObject :
                null;
        }

        /// <summary>
        ///     Converts the <see cref="PrefabFileBijection"/> to a dictionary mapping
        ///     <see cref="FileID">FileIDs</see> from the left <see cref="PrefabFile"/> to
        ///     <see cref="FileID">FileIDs</see> from the right <see cref="PrefabFile"/>.
        /// </summary>
        /// <returns>
        ///     A dictionary mapping <see cref="FileID">FileIDs</see> from the left
        ///     <see cref="PrefabFile"/> to <see cref="FileID">FileIDs</see> from the right
        ///     <see cref="PrefabFile"/>.
        /// </returns>
        public Dictionary<FileID, FileID> ToLeftToRightFileIDMapping()
        {
            Dictionary<FileID, FileID> fileIDMapping = new();

            foreach ((Object leftObject, Object rightObject) in this.leftToRight)
            {
                fileIDMapping[leftObject.FileID] = rightObject.FileID;
            }

            return fileIDMapping;
        }

        /// <summary>
        ///     Converts the <see cref="PrefabFileBijection"/> to a dictionary mapping
        ///     <see cref="FileID">FileIDs</see> from the right <see cref="PrefabFile"/> to
        ///     <see cref="FileID">FileIDs</see> from the left <see cref="PrefabFile"/>.
        /// </summary>
        /// <returns>
        ///     A dictionary mapping <see cref="FileID">FileIDs</see> from the right
        ///     <see cref="PrefabFile"/> to <see cref="FileID">FileIDs</see> from the left
        ///     <see cref="PrefabFile"/>.
        /// </returns>
        public Dictionary<FileID, FileID> ToRightToLeftFileIDMapping()
        {
            Dictionary<FileID, FileID> fileIDMapping = new();

            foreach ((Object rightObject, Object leftObject) in this.rightToLeft)
            {
                fileIDMapping[rightObject.FileID] = leftObject.FileID;
            }

            return fileIDMapping;
        }
    }
}
