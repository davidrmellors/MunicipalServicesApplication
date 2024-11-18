using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
    public class ServiceStatus
    {
        public string Service { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public int Count { get; set; }
    }
}
