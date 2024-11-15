using MunicipalServices.Models;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestTree
    {
        private ServiceRequestNode root;

        public void Insert(ServiceRequest request)
        {
            root = InsertRec(root, request);
            root.IsRed = false; // Root must be black
        }

        private ServiceRequestNode InsertRec(ServiceRequestNode node, ServiceRequest request)
        {
            if (node == null)
                return new ServiceRequestNode(request);

            int compareResult = string.Compare(request.RequestId, node.Data.RequestId);

            if (compareResult < 0)
                node.Left = InsertRec(node.Left, request);
            else if (compareResult > 0)
                node.Right = InsertRec(node.Right, request);
            else
                return node;

            // Red-Black Tree balancing
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

        public ServiceRequest Find(string requestId)
        {
            return FindRec(root, requestId)?.Data;
        }

        private ServiceRequestNode FindRec(ServiceRequestNode node, string requestId)
        {
            if (node == null) return null;

            int compareResult = string.Compare(requestId, node.Data.RequestId);

            if (compareResult < 0)
                return FindRec(node.Left, requestId);
            if (compareResult > 0)
                return FindRec(node.Right, requestId);

            return node;
        }

        public List<ServiceRequest> GetInOrderTraversal()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(ServiceRequestNode node, List<ServiceRequest> result)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, result);
                result.Add(node.Data);
                InOrderTraversal(node.Right, result);
            }
        }
    }
}
