using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.Monitoring
{
    public class GraphNodeVisual
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public List<GraphNodeVisual> ConnectedNodes { get; set; } = new List<GraphNodeVisual>();
    }
}
