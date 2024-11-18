using MunicipalServices.Models;
using MunicipalServices.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a Red-Black tree implementation for storing and managing service requests.
    /// Provides O(log n) time complexity for insertions, deletions and searches through self-balancing.
    /// </summary>
    public class RedBlackTree : ServiceRequestTree
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The root node of the Red-Black tree.
        /// </summary>
        private ServiceRequestNode root;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The total number of nodes in the tree.
        /// </summary>
        private int nodeCount;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the RedBlackTree class.
        /// </summary>
        public RedBlackTree()
        {
            root = null;
            nodeCount = 0;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the total number of service requests in the tree.
        /// </summary>
        public override int Count => nodeCount;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an in-order traversal of all service requests in the tree.
        /// </summary>
        /// <returns>An enumerable collection of service requests in sorted order.</returns>
        public IEnumerable<ServiceRequest> GetInOrderTraversal()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversalHelper(root, result);
            return result;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Helper method for performing an in-order traversal.
        /// </summary>
        /// <param name="node">The current node being traversed.</param>
        /// <param name="result">The list to store traversal results.</param>
        private void InOrderTraversalHelper(ServiceRequestNode node, List<ServiceRequest> result)
        {
            if (node == null) return;
            InOrderTraversalHelper(node.Left, result);
            result.Add(node.Data);
            InOrderTraversalHelper(node.Right, result);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new service request into the tree.
        /// </summary>
        /// <param name="request">The service request to insert.</param>
        public override void Insert(ServiceRequest request)
        {
            root = InsertRec(root, request);
            root.IsRed = false; // root must always be black
            nodeCount++;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Recursive helper method for inserting a node and maintaining Red-Black tree properties.
        /// </summary>
        /// <param name="node">The current node in the recursion.</param>
        /// <param name="request">The service request to insert.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode InsertRec(ServiceRequestNode node, ServiceRequest request)
        {
            if (node == null)
                return new ServiceRequestNode(request);

            int comparison = string.Compare(request.RequestId, node.Data.RequestId);

            if (comparison < 0)
                node.Left = InsertRec(node.Left, request);
            else if (comparison > 0)
                node.Right = InsertRec(node.Right, request);

            // Fix Red-Black Tree violations
            if (IsRed(node.Right) && !IsRed(node.Left))
                node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left))
                node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right))
                FlipColors(node);

            return node;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if a node is red.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node is red, false otherwise.</returns>
        private bool IsRed(ServiceRequestNode node)
        {
            return node != null && node.IsRed;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a left rotation on the given node.
        /// </summary>
        /// <param name="h">The node to rotate.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode RotateLeft(ServiceRequestNode h)
        {
            ServiceRequestNode x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a right rotation on the given node.
        /// </summary>
        /// <param name="h">The node to rotate.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode RotateRight(ServiceRequestNode h)
        {
            ServiceRequestNode x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Flips the colors of a node and its children.
        /// </summary>
        /// <param name="h">The node whose colors to flip.</param>
        private void FlipColors(ServiceRequestNode h)
        {
            h.IsRed = true;
            h.Left.IsRed = false;
            h.Right.IsRed = false;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds a service request by its ID.
        /// </summary>
        /// <param name="requestId">The ID of the request to find.</param>
        /// <returns>The found service request, or null if not found.</returns>
        public override ServiceRequest Find(string requestId)
        {
            ServiceRequest result = null;
            result = FindRec(root, requestId)?.Data;
            return result;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Recursive helper method for finding a node.
        /// </summary>
        /// <param name="node">The current node in the recursion.</param>
        /// <param name="requestId">The ID to search for.</param>
        /// <returns>The found node, or null if not found.</returns>
        private ServiceRequestNode FindRec(ServiceRequestNode node, string requestId)
        {
            if (node == null || node.Data.RequestId == requestId)
                return node;

            int comparison = string.Compare(requestId, node.Data.RequestId);
            return comparison < 0
                ? FindRec(node.Left, requestId)
                : FindRec(node.Right, requestId);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes a service request from the tree by its ID.
        /// </summary>
        /// <param name="requestId">The ID of the request to delete.</param>
        public override void Delete(string requestId)
        {
            root = DeleteRec(root, requestId);
            if (root != null) root.IsRed = false;
            nodeCount--;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Recursive helper method for deleting a node.
        /// </summary>
        /// <param name="node">The current node in the recursion.</param>
        /// <param name="requestId">The ID of the request to delete.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode DeleteRec(ServiceRequestNode node, string requestId)
        {
            if (node == null)
                return null;

            int comparison = string.Compare(requestId, node.Data.RequestId);
            
            if (comparison < 0)
            {
                if (!IsRed(node.Left) && !IsRed(node.Left?.Left))
                    node = MoveRedLeft(node);
                node.Left = DeleteRec(node.Left, requestId);
            }
            else
            {
                if (IsRed(node.Left))
                    node = RotateRight(node);
                if (comparison == 0 && node.Right == null)
                    return null;
                if (!IsRed(node.Right) && !IsRed(node.Right?.Left))
                    node = MoveRedRight(node);
                if (comparison == 0)
                {
                    var min = FindMin(node.Right);
                    node.Data = min.Data;
                    node.Right = DeleteMin(node.Right);
                }
                else
                    node.Right = DeleteRec(node.Right, requestId);
            }

            return FixUp(node);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Moves a red link to the left during deletion.
        /// </summary>
        /// <param name="node">The node to process.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode MoveRedLeft(ServiceRequestNode node)
        {
            FlipColors(node);
            if (IsRed(node.Right?.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                FlipColors(node);
            }
            return node;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Moves a red link to the right during deletion.
        /// </summary>
        /// <param name="node">The node to process.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode MoveRedRight(ServiceRequestNode node)
        {
            FlipColors(node);
            if (IsRed(node.Left?.Left))
            {
                node = RotateRight(node);
                FlipColors(node);
            }
            return node;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fixes any Red-Black tree violations after a modification.
        /// </summary>
        /// <param name="node">The node to fix up.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode FixUp(ServiceRequestNode node)
        {
            if (IsRed(node.Right))
                node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left))
                node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right))
                FlipColors(node);
            return node;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds the minimum node in a subtree.
        /// </summary>
        /// <param name="node">The root of the subtree.</param>
        /// <returns>The node with the minimum value.</returns>
        private ServiceRequestNode FindMin(ServiceRequestNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the minimum node from a subtree.
        /// </summary>
        /// <param name="node">The root of the subtree.</param>
        /// <returns>The new root of the subtree.</returns>
        private ServiceRequestNode DeleteMin(ServiceRequestNode node)
        {
            if (node.Left == null)
                return null;
            if (!IsRed(node.Left) && !IsRed(node.Left.Left))
                node = MoveRedLeft(node);
            node.Left = DeleteMin(node.Left);
            return FixUp(node);
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//