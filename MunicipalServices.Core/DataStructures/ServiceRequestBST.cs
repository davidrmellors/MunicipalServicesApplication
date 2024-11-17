using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestBST
    {
        private class Node
        {
            public ServiceRequest Data;
            public Node Left, Right;
            public int Height;

            public Node(ServiceRequest data)
            {
                Data = data;
                Height = 1;
            }
        }

        private Node root;

        public void Insert(ServiceRequest request)
        {
            root = InsertRec(root, request);
        }

        private Node InsertRec(Node node, ServiceRequest request)
        {
            if (node == null)
                return new Node(request);

            if (request.Priority < node.Data.Priority)
                node.Left = InsertRec(node.Left, request);
            else
                node.Right = InsertRec(node.Right, request);

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            return BalanceNode(node);
        }

        private Node BalanceNode(Node node)
        {
            int balance = GetBalance(node);

            // Left Heavy
            if (balance > 1)
            {
                if (GetBalance(node.Left) >= 0)
                    return RotateRight(node);
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right Heavy
            if (balance < -1)
            {
                if (GetBalance(node.Right) <= 0)
                    return RotateLeft(node);
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        private int GetHeight(Node node)
        {
            return node?.Height ?? 0;
        }

        private int GetBalance(Node node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        private Node RotateRight(Node y)
        {
            Node x = y.Left;
            Node T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.Right;
            Node T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        public IEnumerable<ServiceRequest> GetHighestPriorityRequests(int count)
        {
            var result = new List<ServiceRequest>();
            GetHighestPriorityRec(root, result, count);
            return result;
        }

        private void GetHighestPriorityRec(Node node, List<ServiceRequest> result, int count)
        {
            if (node == null || result.Count >= count) return;

            // Traverse right first to get highest priority
            GetHighestPriorityRec(node.Right, result, count);

            if (result.Count < count)
            {
                result.Add(node.Data);
                GetHighestPriorityRec(node.Left, result, count);
            }
        }

        public int CountByStatus(string status)
        {
            return CountByStatusRec(root, status);
        }

        private int CountByStatusRec(Node node, string status)
        {
            if (node == null) return 0;

            int count = node.Data.Status == status ? 1 : 0;
            return count + CountByStatusRec(node.Left, status) + CountByStatusRec(node.Right, status);
        }

        public IEnumerable<ServiceRequest> GetAll()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(Node node, List<ServiceRequest> result)
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
