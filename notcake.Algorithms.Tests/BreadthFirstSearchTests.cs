using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Algorithms.Tests
{
    /// <summary>
    ///     Tests for the <see cref="BreadthFirstSearch"/> class.
    /// </summary>
    [TestClass]
    public class BreadthFirstSearchTests
    {
        /// <summary>
        ///     Tests the breadth-first traversal of a cyclic graph with a single node.
        /// </summary>
        [TestMethod]
        public void Cycle()
        {
            IReadOnlyList<IReadOnlyList<int>> edgeLists = Graphs.Cycle;

            List<int> nodes = new();
            List<(int, int)> edges = new();
            BreadthFirstSearch.Enumerate(
                0,
                node => edgeLists[node],
                nodes.Add,
                (a, b) => edges.Add((a, b))
            );

            Assert.IsTrue(new int[] { 0 }.SequenceEqual(nodes));
            Assert.IsTrue(
                new (int, int)[]
                {
                    (0, 0),
                }.SequenceEqual(edges)
            );
        }

        /// <summary>
        ///     Tests the breadth-first traversal of a binary tree graph with 7 nodes.
        /// </summary>
        [TestMethod]
        public void Tree()
        {
            //       0
            //      / \
            //     /   \
            //    /     \
            //   1       2
            //  / \     / \
            // 3   4   5   6

            IReadOnlyList<IReadOnlyList<int>> edgeLists = Graphs.Tree;

            List<int> nodes = new();
            List<(int, int)> edges = new();
            BreadthFirstSearch.Enumerate(
                0,
                node => edgeLists[node],
                nodes.Add,
                (a, b) => edges.Add((a, b))
            );

            Assert.IsTrue(new int[] { 0, 1, 2, 3, 4, 5, 6 }.SequenceEqual(nodes));
            Assert.IsTrue(
                new (int, int)[]
                {
                    (0, 1),
                    (0, 2),
                    (1, 3),
                    (1, 4),
                    (2, 5),
                    (2, 6),
                }.SequenceEqual(edges)
            );
        }
    }
}
