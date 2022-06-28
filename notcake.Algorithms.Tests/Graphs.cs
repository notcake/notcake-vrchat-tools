using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace notcake.Algorithms.Tests
{
    /// <summary>
    ///     Provides graphs for testing.
    /// </summary>
    public static class Graphs
    {
        /// <summary>
        ///     Gets a cyclic graph with a single node.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<int>> Cycle { get; } = new[]
        {
            /* 0 -> */ new int[] { 0 },
        };

        /// <summary>
        ///     Gets a binary tree graph with 7 nodes.
        /// </summary>
        [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
        public static IReadOnlyList<IReadOnlyList<int>> Tree { get; } = new[]
        {
            //       0
            //      / \
            //     /   \
            //    /     \
            //   1       2
            //  / \     / \
            // 3   4   5   6

            /* 0 -> */ new int[] { 1, 2 },
            /* 1 -> */ new int[] { 3, 4 },
            /* 2 -> */ new int[] { 5, 6 },
            /* 3 -> */ new int[] { },
            /* 4 -> */ new int[] { },
            /* 5 -> */ new int[] { },
            /* 6 -> */ new int[] { },
        };
    }
}
