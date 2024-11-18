using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a community statistic with a label, value and progress percentage
    /// </summary>
    public class CommunityStat
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the display label for the statistic
        /// </summary>
        public string Label { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the value of the statistic
        /// </summary>
        public string Value { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the progress percentage (0.0 to 1.0) for the statistic
        /// </summary>
        public double Progress { get; set; }
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//