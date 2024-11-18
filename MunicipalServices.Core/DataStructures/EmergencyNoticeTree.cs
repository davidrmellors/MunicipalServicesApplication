using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class EmergencyNoticeTree
    {
        private EmergencyNoticeNode root;
        private static EmergencyNoticeTree instance;

        public static EmergencyNoticeTree Instance
        {
            get
            {
                if (instance == null)
                    instance = new EmergencyNoticeTree();
                return instance;
            }
        }

        public void Insert(EmergencyNotice notice)
        {
            var node = new EmergencyNoticeNode(notice);
            root = InsertRec(root, node);
            root.IsRed = false;
        }

        private EmergencyNoticeNode InsertRec(EmergencyNoticeNode node, EmergencyNoticeNode newNode)
        {
            if (node == null) return newNode;

            int comparison = string.Compare(newNode.Data.Title, node.Data.Title);
            if (comparison < 0)
                node.Left = InsertRec(node.Left, newNode);
            else if (comparison > 0)
                node.Right = InsertRec(node.Right, newNode);

            return BalanceTree(node);
        }

        private bool IsRed(EmergencyNoticeNode node)
        {
            return node != null && node.IsRed;
        }

        private EmergencyNoticeNode RotateLeft(EmergencyNoticeNode h)
        {
            EmergencyNoticeNode x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

        private EmergencyNoticeNode RotateRight(EmergencyNoticeNode h)
        {
            EmergencyNoticeNode x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            return x;
        }

        private void FlipColors(EmergencyNoticeNode h)
        {
            h.IsRed = true;
            h.Left.IsRed = false;
            h.Right.IsRed = false;
        }

        private EmergencyNoticeNode BalanceTree(EmergencyNoticeNode node)
        {
            if (node == null) return null;

            if (IsRed(node.Right) && !IsRed(node.Left))
                node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left))
                node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right))
                FlipColors(node);

            return node;
        }

        public IEnumerable<EmergencyNotice> GetAll()
        {
            var result = new List<EmergencyNotice>();
            InOrderTraversal(root, result);
            return result;
        }

        private void InOrderTraversal(EmergencyNoticeNode node, List<EmergencyNotice> result)
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
