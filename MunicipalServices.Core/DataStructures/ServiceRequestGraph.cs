using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Diagnostics;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestGraph
    {
        private readonly Dictionary<string, ServiceRequest> _nodes;
        private readonly Dictionary<string, HashSet<string>> _adjacencyList;

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

        public enum TraversalType
        {
            BFS,            // Breadth-First Search
            DFS,            // Depth-First Search
            Priority        // Priority-based traversal
        }

        public ServiceRequestGraph()
        {
            _nodes = new Dictionary<string, ServiceRequest>();
            _adjacencyList = new Dictionary<string, HashSet<string>>();
        }

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

        private void AddEdge(string sourceId, string targetId)
        {
            if (!_adjacencyList.ContainsKey(sourceId))
                _adjacencyList[sourceId] = new HashSet<string>();
            if (!_adjacencyList.ContainsKey(targetId))
                _adjacencyList[targetId] = new HashSet<string>();

            _adjacencyList[sourceId].Add(targetId);
            _adjacencyList[targetId].Add(sourceId);
        }

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

        private double ToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

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

        // Add public methods to expose nodes and edges
        public IEnumerable<ServiceRequest> GetNodes()
        {
            return _nodes.Values;
        }

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

        public class Edge
        {
            public string Source { get; set; }
            public string Destination { get; set; }
        }

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
    }
}
