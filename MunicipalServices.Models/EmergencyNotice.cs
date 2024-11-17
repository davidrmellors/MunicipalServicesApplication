using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
    public class EmergencyNotice
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.Now;
        public string Severity { get; set; } // High, Medium, Low
    }
}
