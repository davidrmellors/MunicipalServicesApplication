using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Diagnostics;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a graph data structure for storing and analyzing relationships between service requests.
    /// Provides methods for traversal, path finding, and component analysis.
    /// </summary>
    public class ServiceRequestGraph
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Dictionary storing service request nodes, keyed by request ID.
        /// </summary>
        private readonly Dictionary<string, ServiceRequest> _nodes;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adjacency list representation of edges between nodes.
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> _adjacencyList;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a cluster of related service requests based on impact priority.
        /// </summary>
        /// <param name="requestId">The ID of the request to start clustering from.</param>
        /// <returns>A collection of related service requests.</returns>
        public IEnumerable<ServiceRequest> GetImpactCluster(string requestId)
        {
            var cluster = new HashSet<ServiceRequest>();
            var heap = new ServiceRequestHeap();
            var visited = new HashSet<string>();
            
            if (!_nodes.ContainsKey(requestId))
                return cluster;
                
            var initial = _nodes[requestId];
            heap.Insert(initial);
            
            while (true)
            {
                ServiceRequest current;
                try
                {
                    current = heap.ExtractMax();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                
                if (!visited.Add(current.RequestId)) 
                    continue;
                
                cluster.Add(current);
                
                foreach (var relatedId in _adjacencyList[current.RequestId])
                {
                    if (!visited.Contains(relatedId))
                    {
                        var related = _nodes[relatedId];
                        heap.Insert(related);
                    }
                }
            }

            return cluster;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Defines the available graph traversal strategies.
        /// </summary>
        public enum TraversalType
        {
            /// <summary>Breadth-First Search traversal</summary>
            BFS,
            /// <summary>Depth-First Search traversal</summary>
            DFS,
            /// <summary>Priority-based traversal</summary>
            Priority
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceRequestGraph class.
        /// </summary>
        public ServiceRequestGraph()
        {
            _nodes = new Dictionary<string, ServiceRequest>();
            _adjacencyList = new Dictionary<string, HashSet<string>>();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new service request to the graph and creates edges to related requests.
        /// </summary>
        /// <param name="request">The service request to add.</param>
        public void AddRequest(ServiceRequest request)
        {
            if (request == null) return;

            // Add node if it doesn't exist
            if (!_nodes.ContainsKey(request.RequestId))
            {
                _nodes[request.RequestId] = request;
                _adjacencyList[request.RequestId] = new HashSet<string>();
            }

            // Connect related requests based on location and category
            foreach (var existingRequest in _nodes.Values)
            {
                if (existingRequest.RequestId == request.RequestId) continue;

                if (AreRequestsRelated(request, existingRequest))
                {
                    AddEdge(request.RequestId, existingRequest.RequestId);
                }
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds an undirected edge between two service requests.
        /// </summary>
        /// <param name="sourceId">The ID of the source request.</param>
        /// <param name="targetId">The ID of the target request.</param>
        private void AddEdge(string sourceId, string targetId)
        {
            if (!_adjacencyList.ContainsKey(sourceId))
                _adjacencyList[sourceId] = new HashSet<string>();
            if (!_adjacencyList.ContainsKey(targetId))
                _adjacencyList[targetId] = new HashSet<string>();

            _adjacencyList[sourceId].Add(targetId);
            _adjacencyList[targetId].Add(sourceId);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines if two service requests are related based on multiple criteria.
        /// </summary>
        /// <param name="r1">First service request to compare.</param>
        /// <param name="r2">Second service request to compare.</param>
        /// <returns>True if the requests are related, false otherwise.</returns>
        private bool AreRequestsRelated(ServiceRequest r1, ServiceRequest r2)
        {
            if (r1 == null || r2 == null) return false;

            int relationshipScore = 0;
            
            // Category matching (highest weight)
            if (r1.Category == r2.Category)
                relationshipScore += 40;
            
            // Location proximity (medium weight)
            if (AreLocationsNearby(r1, r2))
                relationshipScore += 30;
            
            // Time proximity (lower weight)
            var timeDiff = Math.Abs((r1.SubmissionDate - r2.SubmissionDate).TotalHours);
            if (timeDiff <= 24) // Within 24 hours
                relationshipScore += 20;
            
            // Priority similarity (lowest weight)
            if (Math.Abs(r1.Priority - r2.Priority) <= 1)
                relationshipScore += 10;
            
            return relationshipScore >= 40; // Threshold for considering requests related
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if two service requests are geographically nearby.
        /// </summary>
        /// <param name="r1">First service request to compare.</param>
        /// <param name="r2">Second service request to compare.</param>
        /// <returns>True if the locations are within 1km of each other.</returns>
        private bool AreLocationsNearby(ServiceRequest r1, ServiceRequest r2)
        {
            if (r1.Latitude == 0 || r1.Longitude == 0 || r2.Latitude == 0 || r2.Longitude == 0)
                return r1.Location.Equals(r2.Location, StringComparison.OrdinalIgnoreCase);

            // Calculate distance using Haversine formula
            const double earthRadius = 6371; // Earth's radius in kilometers
            var lat1 = ToRadian(r1.Latitude);
            var lon1 = ToRadian(r1.Longitude);
            var lat2 = ToRadian(r2.Latitude);
            var lon2 = ToRadian(r2.Longitude);

            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            var distance = earthRadius * c;

            return distance <= 1.0; // Within 1 kilometer
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degree">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private double ToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all service requests related to a given request using specified traversal method.
        /// </summary>
        /// <param name="requestId">The ID of the starting request.</param>
        /// <param name="traversalType">The type of graph traversal to use.</param>
        /// <returns>A collection of related service requests.</returns>
        public IEnumerable<ServiceRequest> GetRelatedRequests(string requestId, TraversalType traversalType = TraversalType.BFS)
        {
            if (!_adjacencyList.ContainsKey(requestId))
                return Enumerable.Empty<ServiceRequest>();

            var visited = new HashSet<string>();
            var result = new List<ServiceRequest>();

            switch (traversalType)
            {
                case TraversalType.BFS:
                    BFSTraversal(requestId, visited, result);
                    break;

                case TraversalType.DFS:
                    DFSTraversal(requestId, visited, result);
                    break;

                case TraversalType.Priority:
                    PriorityTraversal(requestId, visited, result);
                    break;
            }

            Debug.WriteLine($"Found {result.Count} related requests for {requestId} using {traversalType} traversal");
            return result;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a breadth-first search traversal starting from a given node.
        /// </summary>
        /// <param name="startId">The ID of the starting request.</param>
        /// <param name="visited">Set of visited node IDs.</param>
        /// <param name="result">List to store traversal results.</param>
        private void BFSTraversal(string startId, HashSet<string> visited, List<ServiceRequest> result)
        {
            var queue = new Queue<string>();
            queue.Enqueue(startId);
            visited.Add(startId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (_nodes.ContainsKey(currentId))
                    result.Add(_nodes[currentId]);

                foreach (var neighborId in _adjacencyList[currentId])
                {
                    if (!visited.Contains(neighborId))
                    {
                        visited.Add(neighborId);
                        queue.Enqueue(neighborId);
                    }
                }
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a depth-first search traversal starting from a given node.
        /// </summary>
        /// <param name="currentId">The ID of the current request.</param>
        /// <param name="visited">Set of visited node IDs.</param>
        /// <param name="result">List to store traversal results.</param>
        private void DFSTraversal(string currentId, HashSet<string> visited, List<ServiceRequest> result)
        {
            visited.Add(currentId);
            if (_nodes.ContainsKey(currentId))
                result.Add(_nodes[currentId]);

            foreach (var neighborId in _adjacencyList[currentId])
            {
                if (!visited.Contains(neighborId))
                {
                    DFSTraversal(neighborId, visited, result);
                }
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a priority-based traversal starting from a given node.
        /// </summary>
        /// <param name="startId">The ID of the starting request.</param>
        /// <param name="visited">Set of visited node IDs.</param>
        /// <param name="result">List to store traversal results.</param>
        private void PriorityTraversal(string startId, HashSet<string> visited, List<ServiceRequest> result)
        {
            var priorityQueue = new SortedDictionary<int, Queue<string>>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            
            var startPriority = _nodes[startId].Priority;
            if (!priorityQueue.ContainsKey(startPriority))
            {
                priorityQueue.Add(startPriority, new Queue<string>());
            }
            priorityQueue[startPriority].Enqueue(startId);
            visited.Add(startId);

            while (priorityQueue.Any())
            {
                var currentPriority = priorityQueue.First();
                var currentId = currentPriority.Value.Dequeue();

                if (_nodes.ContainsKey(currentId))
                    result.Add(_nodes[currentId]);

                foreach (var neighborId in _adjacencyList[currentId])
                {
                    if (!visited.Contains(neighborId))
                    {
                        visited.Add(neighborId);
                        var priority = _nodes[neighborId].Priority;
                        
                        if (!priorityQueue.ContainsKey(priority))
                            priorityQueue.Add(priority, new Queue<string>());
                            
                        priorityQueue[priority].Enqueue(neighborId);
                    }
                }

                if (!currentPriority.Value.Any())
                    priorityQueue.Remove(currentPriority.Key);
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all nodes in the graph.
        /// </summary>
        /// <returns>A collection of all service requests in the graph.</returns>
        public IEnumerable<ServiceRequest> GetNodes()
        {
            return _nodes.Values;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all edges in the graph.
        /// </summary>
        /// <returns>A collection of all edges between service requests.</returns>
        public IEnumerable<Edge> GetEdges()
        {
            var edges = new List<Edge>();
            foreach (var source in _adjacencyList.Keys)
            {
                foreach (var target in _adjacencyList[source])
                {
                    // Only add each edge once (avoid duplicates)
                    if (string.CompareOrdinal(source, target) < 0)
                    {
                        edges.Add(new Edge 
                        { 
                            Source = source, 
                            Destination = target 
                        });
                    }
                }
            }
            return edges;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Represents an edge between two service requests in the graph.
        /// </summary>
        public class Edge
        {
            /// <summary>
            /// The ID of the source service request.
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// The ID of the destination service request.
            /// </summary>
            public string Destination { get; set; }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds the shortest path between two service requests using Dijkstra's algorithm.
        /// </summary>
        /// <param name="startId">The ID of the starting request.</param>
        /// <param name="endId">The ID of the ending request.</param>
        /// <returns>A list of service requests representing the shortest path.</returns>
        public List<ServiceRequest> FindShortestPath(string startId, string endId)
        {
            var distances = new Dictionary<string, int>();
            var previous = new Dictionary<string, string>();
            var unvisited = new HashSet<string>(_nodes.Keys);

            foreach (var node in _nodes.Keys)
            {
                distances[node] = int.MaxValue;
            }
            distances[startId] = 0;

            while (unvisited.Count > 0)
            {
                string current = unvisited.OrderBy(n => distances[n]).First();
                if (current == endId) break;

                unvisited.Remove(current);

                foreach (var neighbor in _adjacencyList[current])
                {
                    if (!unvisited.Contains(neighbor)) continue;

                    int alt = distances[current] + 1;
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }

            // Reconstruct path
            var path = new List<ServiceRequest>();
            string currentId = endId;
            while (previous.ContainsKey(currentId))
            {
                path.Insert(0, _nodes[currentId]);
                currentId = previous[currentId];
            }
            path.Insert(0, _nodes[startId]);
            return path;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds all connected components in the graph using depth-first search.
        /// </summary>
        /// <returns>A dictionary mapping component IDs to sets of request IDs in each component.</returns>
        public Dictionary<string, HashSet<string>> FindConnectedComponents()
        {
            var visited = new HashSet<string>();
            var components = new Dictionary<string, HashSet<string>>();
            var componentId = 0;

            foreach (var node in _nodes.Keys)
            {
                if (!visited.Contains(node))
                {
                    var component = new HashSet<string>();
                    DFSComponent(node, visited, component);
                    components[$"Component_{componentId++}"] = component;
                }
            }

            return components;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Helper method for finding connected components using depth-first search.
        /// </summary>
        /// <param name="currentId">The ID of the current request being processed.</param>
        /// <param name="visited">Set of visited node IDs.</param>
        /// <param name="component">Set to store the current component's node IDs.</param>
        private void DFSComponent(string currentId, HashSet<string> visited, HashSet<string> component)
        {
            visited.Add(currentId);
            component.Add(currentId);

            foreach (var neighborId in _adjacencyList[currentId])
            {
                if (!visited.Contains(neighborId))
                {
                    DFSComponent(neighborId, visited, component);
                }
            }
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//