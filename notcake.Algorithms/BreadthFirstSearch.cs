using System;
using System.Collections.Generic;

namespace notcake.Algorithms
{
    /// <summary>
    ///     Provides methods to traverse graphs in breadth-first order.
    /// </summary>
    public static class BreadthFirstSearch
    {
        /// <summary>
        ///     Gets an <see cref="IEnumerable{T}"/> containing a graph's nodes in breadth-first
        ///     order.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{T}"/> containing the graph's nodes in breadth-first order.
        /// </returns>
        /// <inheritdoc cref="Enumerate{NodeT}(
        ///     NodeT,
        ///     Func{NodeT, IEnumerable{NodeT}},
        ///     Action{NodeT},
        ///     Action{NodeT, NodeT}?,
        ///     HashSet{NodeT}?
        /// )"/>
        public static IEnumerable<NodeT> Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator
        )
        {
            List<NodeT> nodes = new();
            BreadthFirstSearch.Enumerate(startNode, getEdgeEnumerator, nodes.Add, null, null);
            return nodes;
        }

        /// <inheritdoc cref="Enumerate{NodeT}(
        ///     NodeT,
        ///     Func{NodeT, IEnumerable{NodeT}},
        ///     Action{NodeT},
        ///     Action{NodeT, NodeT}?,
        ///     HashSet{NodeT}?
        /// )"/>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT> nodeCallback
        )
        {
            BreadthFirstSearch.Enumerate(startNode, getEdgeEnumerator, nodeCallback, null, null);
        }

        /// <inheritdoc cref="Enumerate{NodeT}(
        ///     NodeT,
        ///     Func{NodeT, IEnumerable{NodeT}},
        ///     Action{NodeT},
        ///     Action{NodeT, NodeT}?,
        ///     HashSet{NodeT}?
        /// )"/>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT> nodeCallback,
            Action<NodeT, NodeT>? edgeCallback
        )
        {
            BreadthFirstSearch.Enumerate(
                startNode,
                getEdgeEnumerator,
                nodeCallback,
                edgeCallback,
                null
            );
        }

        /// <summary>
        ///     Traverses a graph in breadth-first order.
        /// </summary>
        /// <typeparam name="NodeT">The node type.</typeparam>
        /// <param name="startNode">The starting node.</param>
        /// <param name="getEdgeEnumerator">
        ///     A function that returns an <see cref="IEnumerable{T}"/> of outgoing edges of a given
        ///     node.
        /// </param>
        /// <param name="nodeCallback">A callback to be called for every node visited.</param>
        /// <param name="edgeCallback">
        ///     An optional callback to be called for every edge seen.
        /// </param>
        /// <param name="queuedNodes">
        ///     An optional <see cref="HashSet{T}"/> to be used to record nodes that have been seen.
        /// </param>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT> nodeCallback,
            Action<NodeT, NodeT>? edgeCallback,
            HashSet<NodeT>? queuedNodes
        )
        {
            queuedNodes ??= new HashSet<NodeT>();
            Queue<NodeT> queue = new();

            queuedNodes.Add(startNode);
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                NodeT node = queue.Dequeue();

                nodeCallback(node);

                foreach (NodeT nextNode in getEdgeEnumerator(node))
                {
                    edgeCallback?.Invoke(node, nextNode);

                    if (!queuedNodes.Contains(nextNode))
                    {
                        queuedNodes.Add(nextNode);
                        queue.Enqueue(nextNode);
                    }
                }
            }
        }
    }
}
