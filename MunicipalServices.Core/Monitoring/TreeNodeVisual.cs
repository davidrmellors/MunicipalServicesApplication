using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.Monitoring
{
    public class TreeNodeVisual
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public string BorderColor { get; set; }
        public List<TreeNodeVisual> Children { get; set; } = new List<TreeNodeVisual>();
        public bool IsAnimating { get; set; }
        public TreeOperation LastOperation { get; set; }
    }
}
