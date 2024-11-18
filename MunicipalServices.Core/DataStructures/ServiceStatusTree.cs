using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceStatusTree
    {
        private readonly List<ServiceStatus> _statuses;

        public ServiceStatusTree()
        {
            _statuses = new List<ServiceStatus>
            {
                new ServiceStatus { Service = "Water Supply", Status = "Operational", StatusColor = "#007A4D" },
                new ServiceStatus { Service = "Electricity", Status = "Maintenance", StatusColor = "#FFB612" },
                new ServiceStatus { Service = "Waste Collection", Status = "Operational", StatusColor = "#007A4D" },
                new ServiceStatus { Service = "Road Maintenance", Status = "Disrupted", StatusColor = "#E03C31" }
            };
        }

        public List<ServiceStatus> GetAll() => _statuses;
    }
}