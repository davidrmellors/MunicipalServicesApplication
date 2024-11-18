using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;
using MunicipalServices.Core.Services;
using System.Diagnostics;
using System.Data.SQLite;

namespace MunicipalServices.Core.Services
{
    public class ServiceRequestManager
    {
        private readonly ServiceRequestGraph _requestGraph;
        private readonly RedBlackTree _requestTree;

        public ServiceRequestManager()
        {
            _requestGraph = new ServiceRequestGraph();
            _requestTree = new RedBlackTree();
        }

        public void ProcessNewRequest(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            using (var connection = new SQLiteConnection(DatabaseService.Instance.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save the main request
                        DatabaseService.Instance.SaveServiceRequestWithTransaction(connection, request);

                        // Add to graph and tree
                        _requestGraph.AddRequest(request);
                        _requestTree.Insert(request);

                        // Get and save related requests within the same transaction
                        var relatedRequests = _requestGraph.GetImpactCluster(request.RequestId);
                        if (relatedRequests.Any())
                        {
                            DatabaseService.Instance.SaveRelatedIssuesWithTransaction(
                                connection, 
                                request.RequestId, 
                                relatedRequests.Select(r => r.RequestId));
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"Failed to process request: {ex.Message}", ex);
                    }
                }
            }
        }

        public IEnumerable<ServiceRequest> GetRelatedIssues(string requestId)
        {
            try
            {
                return _requestGraph.GetImpactCluster(requestId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting related issues: {ex.Message}");
                return Enumerable.Empty<ServiceRequest>();
            }
        }
    }
}
