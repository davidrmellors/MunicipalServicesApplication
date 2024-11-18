using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunicipalServices.Core.Monitoring
{
    public class PerformanceTracker
    {
        private readonly Dictionary<string, List<OperationMetrics>> _metrics = new Dictionary<string, List<OperationMetrics>>();

        public void TrackOperation(string treeType, string operation, int nodeCount, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action.Invoke();

            stopwatch.Stop();

            var metrics = new OperationMetrics
            {
                TreeType = treeType,
                Operation = operation,
                NodeCount = nodeCount,
                ExecutionTime = stopwatch.ElapsedMilliseconds
            };

            if (!_metrics.ContainsKey(treeType))
                _metrics[treeType] = new List<OperationMetrics>();

            _metrics[treeType].Add(metrics);
        }

        public IEnumerable<OperationMetrics> GetMetrics(string treeType)
        {
            return _metrics.ContainsKey(treeType) ? _metrics[treeType] : new List<OperationMetrics>();
        }
    }

    public class OperationMetrics
    {
        public string TreeType { get; set; }
        public string Operation { get; set; }
        public int NodeCount { get; set; }
        public long ExecutionTime { get; set; }
    }
}