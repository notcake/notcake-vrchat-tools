namespace notcake.Functional
{
    /// <inheritdoc cref="Either{L, R}"/>
    public static class Either
    {
        /// <summary>
        ///     Represents the possibility which an <see cref="Either{L, R}"/> contains.
        /// </summary>
        public enum Side : byte
        {
            /// <summary>
            ///     Indicates that the <see cref="Either{L, R}"/> contains the left possibility.
            /// </summary>
            Left,
            /// <summary>
            ///     Indicates that the <see cref="Either{L, R}"/> contains the right possibility.
            /// </summary>
            Right,
        }
    }
}
