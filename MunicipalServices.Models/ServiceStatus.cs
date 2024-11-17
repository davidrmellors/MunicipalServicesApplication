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
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
