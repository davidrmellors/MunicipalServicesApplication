using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents the status of a municipal service
    /// </summary>
    public class ServiceStatus
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the municipal service
        /// </summary>
        public string Service { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the current status of the service (e.g. "Active", "Down", "Maintenance")
        /// </summary>
        public string Status { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color used to represent the status visually
        /// </summary>
        public string StatusColor { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the count of active instances or relevant metrics for this service
        /// </summary>
        public int Count { get; set; }
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//