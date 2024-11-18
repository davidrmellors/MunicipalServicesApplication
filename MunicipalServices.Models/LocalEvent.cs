using System;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a local event with details like title, date, description, and category
    /// </summary>
    public class LocalEvent
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets whether this event is recommended to the user
        /// </summary>
        public bool IsRecommended { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the unique identifier for the event. Defaults to a new GUID string.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the title/name of the event
        /// </summary>
        public string Title { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the date and time when the event occurs
        /// </summary>
        public DateTime Date { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the formatted date string representation
        /// </summary>
        public string DateString { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the detailed description of the event
        /// </summary>
        public string Description { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the category/type of the event
        /// </summary>
        public string Category { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the URL to the event's image
        /// </summary>
        public string ImageUrl { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the URL to the event's webpage or details
        /// </summary>
        public string Url { get; set; }
//-------------------------------------------------------------------------------------------------------------
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
