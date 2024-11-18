using MunicipalServices.Models;
using MunicipalServices.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.DataStructures
{
    public class RedBlackTree : ServiceRequestTree
    {
        private ServiceRequestNode root;
        private int nodeCount;


        public RedBlackTree()
        {
            root = null;
            nodeCount = 0;
        }

        public override int Count => nodeCount;

        public IEnumerable<ServiceRequest> GetInOrderTraversal()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversalHelper(root, result);
            return result;
        }

        private void InOrderTraversalHelper(ServiceRequestNode node, List<ServiceRequest> result)
        {
            if (node == null) return;
            InOrderTraversalHelper(node.Left, result);
            result.Add(node.Data);
            InOrderTraversalHelper(node.Right, result);
        }

        public override void Insert(ServiceRequest request)
        {
            
            root = InsertRec(root, request);
            root.IsRed = false; // root must always be black
            nodeCount++;

        }

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

        private bool IsRed(ServiceRequestNode node)
        {
            return node != null && node.IsRed;
        }

        private ServiceRequestNode RotateLeft(ServiceRequestNode h)
        {
            ServiceRequestNode x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

        private ServiceRequestNode RotateRight(ServiceRequestNode h)
        {
            ServiceRequestNode x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

        private void FlipColors(ServiceRequestNode h)
        {
            h.IsRed = true;
            h.Left.IsRed = false;
            h.Right.IsRed = false;
        }

        public override ServiceRequest Find(string requestId)
        {
            ServiceRequest result = null;
            

            result = FindRec(root, requestId)?.Data;

            return result;
        }

        private ServiceRequestNode FindRec(ServiceRequestNode node, string requestId)
        {
            if (node == null || node.Data.RequestId == requestId)
                return node;

            int comparison = string.Compare(requestId, node.Data.RequestId);
            return comparison < 0
                ? FindRec(node.Left, requestId)
                : FindRec(node.Right, requestId);
        }

        public override void Delete(string requestId)
        {
            root = DeleteRec(root, requestId);
            if (root != null) root.IsRed = false;
            nodeCount--;

        }

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

        private ServiceRequestNode FindMin(ServiceRequestNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private ServiceRequestNode DeleteMin(ServiceRequestNode node)
        {
            if (node.Left == null)
                return null;
            if (!IsRed(node.Left) && !IsRed(node.Left.Left))
                node = MoveRedLeft(node);
            node.Left = DeleteMin(node.Left);
            return FixUp(node);
        }
    }
}
