using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestGraph
    {
        private readonly Dictionary<string, ServiceRequest> _nodes;
        private readonly Dictionary<string, HashSet<string>> _adjacencyList;
        private readonly DataManager _dataManager;

        public enum TraversalType
        {
            BFS,            // Breadth-First Search
            DFS,            // Depth-First Search
            Priority        // Priority-based traversal
        }

        public ServiceRequestGraph(DataManager dataManager)
        {
            _dataManager = dataManager;
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
            return r1.Location == r2.Location || r1.Category == r2.Category;
        }

        public IEnumerable<ServiceRequest> GetRelatedRequests(string requestId, TraversalType traversalType = TraversalType.BFS)
        {
            if (!_nodes.ContainsKey(requestId))
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
            var priorityQueue = new SortedSet<(int priority, string id)>();
            priorityQueue.Add((-_nodes[startId].Priority, startId));
            visited.Add(startId);

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Min;
                priorityQueue.Remove(current);
                var currentId = current.id;
                result.Add(_nodes[currentId]);

                foreach (var neighborId in _adjacencyList[currentId])
                {
                    if (!visited.Contains(neighborId))
                    {
                        visited.Add(neighborId);
                        priorityQueue.Add((-_nodes[neighborId].Priority, neighborId));
                    }
                }
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
