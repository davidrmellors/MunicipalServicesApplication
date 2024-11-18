using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.Services
{
    public class ServiceAnalytics
    {
        private readonly ServiceRequestGraph _requestGraph;

        public IEnumerable<ServiceRequest> AnalyzeRelatedIssues(string requestId)
        {
            // Get related issues using different traversal strategies
            var breadthFirst = _requestGraph.GetRelatedRequests(requestId, ServiceRequestGraph.TraversalType.BFS);
            var priorityBased = _requestGraph.GetRelatedRequests(requestId, ServiceRequestGraph.TraversalType.Priority);

            // Compare results and identify patterns
            return priorityBased.Where(p => breadthFirst.Contains(p));
        }
    }
}
