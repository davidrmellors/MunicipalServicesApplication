using System;

namespace MunicipalServices.Core.Monitoring
{
    public class TreeOperation
    {
        public enum OperationType
        {
            RotateLeft,
            RotateRight,
            ColorFlip,
            Insert,
            Delete,
            Search
        }

        public string NodeId { get; set; }
        public OperationType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string TreeType { get; set; }  // "AVL" or "RB"
        public bool IsCompleted { get; set; }
        public double ExecutionTime { get; set; }
    }
}