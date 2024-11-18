using MunicipalServices.Models;
using MunicipalServices.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MunicipalServices.Core.Monitoring;

namespace MunicipalServices.Core.DataStructures
{
    public class RedBlackTree : ServiceRequestTree
    {
        private ServiceRequestNode root;
        private int nodeCount;
        private readonly PerformanceTracker _performanceTracker = new PerformanceTracker();

        public override int Count => nodeCount;

        public override void Insert(ServiceRequest request)
        {
            _performanceTracker.TrackOperation("RB", "Insert", nodeCount, () =>
            {
                root = InsertRec(root, request);
                root.IsRed = false; // Root must be black
                nodeCount++;
            });
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
            _performanceTracker.TrackOperation("RB", "Search", nodeCount, () =>
            {
                result = FindRec(root, requestId)?.Data;
            });
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
    }
}
