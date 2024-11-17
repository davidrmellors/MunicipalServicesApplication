using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        private EmergencyNoticeNode BalanceTree(EmergencyNoticeNode node)
        {
            if (node == null) return null;

            // Red-Black Tree balancing
            if (IsRed(node.Right) && !IsRed(node.Left))
                node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left))
                node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right))
                FlipColors(node);

            return node;
        }

        private int GetSeverityValue(string severity)
        {
            if (severity == null) return 0;
            if (severity.ToLower() == "high") return 3;
            if (severity.ToLower() == "medium") return 2;
            if (severity.ToLower() == "low") return 1;
            return 0;
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
