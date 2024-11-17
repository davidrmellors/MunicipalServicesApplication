using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceStatusTree
    {
        private class Node
        {
            public ServiceStatus Data;
            public Node Left, Right;
            public bool IsRed;

            public Node(ServiceStatus data)
            {
                Data = data;
                IsRed = true;
            }
        }

        private Node root;

        public void Insert(ServiceStatus status)
        {
            root = InsertRec(root, status);
            root.IsRed = false;
        }

        private Node InsertRec(Node node, ServiceStatus status)
        {
            if (node == null)
                return new Node(status);

            int compareResult = string.Compare(status.Service, node.Data.Service);

            if (compareResult < 0)
                node.Left = InsertRec(node.Left, status);
            else if (compareResult > 0)
                node.Right = InsertRec(node.Right, status);

            return node;
        }

        public IEnumerable<ServiceStatus> GetAll()
        {
            var result = new List<ServiceStatus>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(Node node, List<ServiceStatus> result)
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