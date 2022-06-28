using System;
using System.Collections.Generic;

namespace notcake.Algorithms
{
    /// <summary>
    ///     Provides methods to traverse graphs in depth-first order.
    /// </summary>
    public static class DepthFirstSearch
    {
        /// <inheritdoc cref="Enumerate{NodeT}(
        ///     NodeT,
        ///     Func{NodeT, IEnumerable{NodeT}},
        ///     Action{NodeT}?,
        ///     Action{NodeT}?,
        ///     Action{NodeT, NodeT}?,
        ///     HashSet{NodeT}?
        /// )"/>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT>? preCallback,
            Action<NodeT>? postCallback
        )
        {
            DepthFirstSearch.Enumerate(
                startNode,
                getEdgeEnumerator,
                preCallback,
                postCallback,
                null,
                null
            );
        }

        /// <inheritdoc cref="Enumerate{NodeT}(
        ///     NodeT,
        ///     Func{NodeT, IEnumerable{NodeT}},
        ///     Action{NodeT}?,
        ///     Action{NodeT}?,
        ///     Action{NodeT, NodeT}?,
        ///     HashSet{NodeT}?
        /// )"/>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT>? preCallback,
            Action<NodeT>? postCallback,
            Action<NodeT, NodeT>? edgeCallback
        )
        {
            DepthFirstSearch.Enumerate(
                startNode,
                getEdgeEnumerator,
                preCallback,
                postCallback,
                edgeCallback,
                null
            );
        }

        /// <summary>
        ///     Traverses a graph in depth-first order.
        /// </summary>
        /// <typeparam name="NodeT">The node type.</typeparam>
        /// <param name="startNode">The starting node.</param>
        /// <param name="getEdgeEnumerator">
        ///     A function that returns an <see cref="IEnumerable{T}"/> of outgoing edges of a given
        ///     node.
        /// </param>
        /// <param name="preCallback">
        ///     An optional callback to be called when starting the depth-first traversal of a node.
        /// </param>
        /// <param name="postCallback">
        ///     An optional callback to be called when finishing the depth-first traversal of a
        ///     node.
        /// </param>
        /// <param name="edgeCallback">
        ///     An optional callback to be called for every edge seen.
        /// </param>
        /// <param name="visitedNodes">
        ///     An optional <see cref="HashSet{T}"/> to be used to record nodes that have been
        ///     visited.
        /// </param>
        public static void Enumerate<NodeT>(
            NodeT startNode,
            Func<NodeT, IEnumerable<NodeT>> getEdgeEnumerator,
            Action<NodeT>? preCallback,
            Action<NodeT>? postCallback,
            Action<NodeT, NodeT>? edgeCallback,
            HashSet<NodeT>? visitedNodes
        )
        {
            visitedNodes ??= new HashSet<NodeT>();
            List<NodeT> stack = new();
            List<IEnumerator<NodeT>> enumeratorStack = new();

            visitedNodes.Add(startNode);
            stack.Add(startNode);
            enumeratorStack.Add(getEdgeEnumerator(startNode).GetEnumerator());

            preCallback?.Invoke(startNode);

            while (enumeratorStack.Count > 0)
            {
                if (enumeratorStack[^1].MoveNext())
                {
                    NodeT node = enumeratorStack[^1].Current;

                    edgeCallback?.Invoke(stack[^1], node);

                    if (!visitedNodes.Contains(node))
                    {
                        visitedNodes.Add(node);
                        stack.Add(node);
                        enumeratorStack.Add(getEdgeEnumerator(node).GetEnumerator());

                        preCallback?.Invoke(node);
                    }
                }
                else
                {
                    postCallback?.Invoke(stack[^1]);

                    enumeratorStack.RemoveAt(enumeratorStack.Count - 1);
                    stack.RemoveAt(stack.Count - 1);
                }
            }
        }
    }
}
