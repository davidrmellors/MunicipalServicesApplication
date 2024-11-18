using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a collection of municipal service statuses.
    /// Provides functionality to track and retrieve the operational status of various city services.
    /// </summary>
    public class ServiceStatusTree
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Internal list storing the status of each municipal service.
        /// </summary>
        private readonly List<ServiceStatus> _statuses;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceStatusTree class.
        /// Populates the initial status list with default service statuses.
        /// </summary>
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

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves all service statuses currently tracked by the system.
        /// </summary>
        /// <returns>A list containing the status of all municipal services.</returns>
        public List<ServiceStatus> GetAll() => _statuses;

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//