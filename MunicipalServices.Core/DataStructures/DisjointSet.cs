using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.DataStructures
{
    public class DisjointSet
    {
        private Dictionary<string, string> parent;
        private Dictionary<string, int> rank;

        public DisjointSet(IEnumerable<string> vertices)
        {
            parent = new Dictionary<string, string>();
            rank = new Dictionary<string, int>();
            
            foreach (var vertex in vertices)
            {
                parent[vertex] = vertex;
                rank[vertex] = 0;
            }
        }

        public string Find(string vertex)
        {
            if (parent[vertex] != vertex)
            {
                parent[vertex] = Find(parent[vertex]);
            }
            return parent[vertex];
        }

        public void Union(string x, string y)
        {
            var rootX = Find(x);
            var rootY = Find(y);

            if (rootX != rootY)
            {
                if (rank[rootX] < rank[rootY])
                {
                    parent[rootX] = rootY;
                }
                else if (rank[rootX] > rank[rootY])
                {
                    parent[rootY] = rootX;
                }
                else
                {
                    parent[rootY] = rootX;
                    rank[rootX]++;
                }
            }
        }

        public bool Connected(string x, string y) => Find(x) == Find(y);
    }
}
