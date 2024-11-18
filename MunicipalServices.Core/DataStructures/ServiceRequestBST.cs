using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestBST : ServiceRequestTree
    {
        private ServiceRequestNode root;
        private int nodeCount;

        public ServiceRequestBST()
        {
            root = null;
        }

        public override int Count => nodeCount;

        private int GetTreeHeight(ServiceRequestNode node)
        {
            if (node == null) return 0;
            return 1 + Math.Max(GetTreeHeight(node.Left), GetTreeHeight(node.Right));
        }

        public override void Insert(ServiceRequest request)
        {
            root = InsertRec(root, request);
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

            return node;
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

        public IEnumerable<ServiceRequest> GetAll()
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

        public override void Delete(string requestId)
        {
            root = DeleteRec(root, requestId);
            nodeCount--;
        }

        private ServiceRequestNode DeleteRec(ServiceRequestNode node, string requestId)
        {
            if (node == null)
                return null;

            int comparison = string.Compare(requestId, node.Data.RequestId);
            
            if (comparison < 0)
                node.Left = DeleteRec(node.Left, requestId);
            else if (comparison > 0)
                node.Right = DeleteRec(node.Right, requestId);
            else
            {
                // Node to delete found
                if (node.Left == null)
                    return node.Right;
                else if (node.Right == null)
                    return node.Left;

                // Node with two children
                var successor = FindMin(node.Right);
                node.Data = successor.Data;
                node.Right = DeleteRec(node.Right, successor.Data.RequestId);
            }

            return node;
        }

        private ServiceRequestNode FindMin(ServiceRequestNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }
    }
}
