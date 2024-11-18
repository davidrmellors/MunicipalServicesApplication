﻿using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using MunicipalServices.Core.Monitoring;
using System.Linq;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestBST : ServiceRequestTree
    {
        private ServiceRequestNode root;
        private int nodeCount;
        private readonly PerformanceTracker _performanceTracker = new PerformanceTracker();

        public override int Count => nodeCount;

        public override void Insert(ServiceRequest request)
        {
            _performanceTracker.TrackOperation("BST", "Insert", nodeCount, () =>
            {
                root = InsertRec(root, request);
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

            return node;
        }

        public override ServiceRequest Find(string requestId)
        {
            ServiceRequest result = null;
            _performanceTracker.TrackOperation("BST", "Search", nodeCount, () =>
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
    }
}
