using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a geographic coordinate with latitude and longitude values
    /// </summary>
    public class Coordinates
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the latitude coordinate in decimal degrees
        /// </summary>
        /// <remarks>Valid values range from -90 to 90 degrees</remarks>
        public double Latitude { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the longitude coordinate in decimal degrees
        /// </summary>
        /// <remarks>Valid values range from -180 to 180 degrees</remarks>
        public double Longitude { get; set; }
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
