using System;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents an emergency notice with details about an urgent situation or alert
    /// </summary>
    public class EmergencyNotice
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the title/heading of the emergency notice
        /// </summary>
        public string Title { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the detailed description of the emergency situation
        /// </summary>
        public string Description { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the ID of the related service request, if applicable
        /// </summary>
        public string RelatedRequestId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the creation timestamp of the notice. Defaults to current time.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the severity level of the emergency (e.g. "Low", "Medium", "High", "Critical")
        /// </summary>
        public string Severity { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets whether the emergency notice is currently active. Defaults to true.
        /// </summary>
        public bool IsActive { get; set; } = true;
//-------------------------------------------------------------------------------------------------------------
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//