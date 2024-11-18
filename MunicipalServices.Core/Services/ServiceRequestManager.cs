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
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Manages service requests, including processing new requests and finding related issues
    /// </summary>
    public class ServiceRequestManager
    {
        private readonly ServiceRequestGraph _requestGraph;
        private readonly RedBlackTree _requestTree;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceRequestManager class
        /// </summary>
        public ServiceRequestManager()
        {
            _requestGraph = new ServiceRequestGraph();
            _requestTree = new RedBlackTree();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes a new service request by saving it to the database and updating related data structures
        /// </summary>
        /// <param name="request">The service request to process</param>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when request processing fails</exception>
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

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds service requests that are related to a given request ID based on location, category and time
        /// </summary>
        /// <param name="requestId">The ID of the request to find related issues for</param>
        /// <returns>A collection of related service requests</returns>
        public IEnumerable<ServiceRequest> GetRelatedIssues(string requestId)
        {
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                {
                    connection.Open();
                    var request = DatabaseService.Instance.GetRequestById(requestId);
                    if (request == null) return Enumerable.Empty<ServiceRequest>();

                    var allRequests = DatabaseService.Instance.GetAllRequests();
                    
                    var relatedRequests = allRequests.Where(r => 
                        r.RequestId != requestId && // Don't include self
                        (
                            (r.Category == request.Category && r.Location == request.Location) ||
                            (r.Category == request.Category && 
                             Math.Abs(r.Latitude - request.Latitude) < 0.001 && 
                             Math.Abs(r.Longitude - request.Longitude) < 0.001) ||
                            (Math.Abs((r.SubmissionDate - request.SubmissionDate).TotalHours) < 24 && 
                             r.Location == request.Location)
                        )
                    ).ToList();

                    // Save the relationships to the database
                    DatabaseService.Instance.SaveRelatedIssuesWithTransaction(connection, requestId, 
                        relatedRequests.Select(r => r.RequestId));

                    return relatedRequests;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding related issues: {ex}");
                return Enumerable.Empty<ServiceRequest>();
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the distance between two geographic coordinates using the Haversine formula
        /// </summary>
        /// <param name="coord1">The first coordinate</param>
        /// <param name="coord2">The second coordinate</param>
        /// <returns>The distance between the coordinates in meters</returns>
        private double CalculateDistance(Coordinates coord1, Coordinates coord2)
        {
            // Haversine formula to calculate distance between two points
            const double R = 6371e3; // Earth's radius in meters
            var φ1 = coord1.Latitude * Math.PI / 180;
            var φ2 = coord2.Latitude * Math.PI / 180;
            var Δφ = (coord2.Latitude - coord1.Latitude) * Math.PI / 180;
            var Δλ = (coord2.Longitude - coord1.Longitude) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in meters
        }

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//