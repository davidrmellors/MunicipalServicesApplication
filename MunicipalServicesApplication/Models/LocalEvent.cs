using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApplication.Models
{
    public class LocalEvent
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; } // Add this property
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
    }
}
