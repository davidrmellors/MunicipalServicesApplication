using System;

namespace MunicipalServices.Core.Monitoring
{
    public class TreePerformanceMetrics
    {
        public string TreeType { get; set; }
        public string OperationType { get; set; }
        public double ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
    }
}