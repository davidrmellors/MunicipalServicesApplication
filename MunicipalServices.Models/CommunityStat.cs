using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
    public class CommunityStat
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
