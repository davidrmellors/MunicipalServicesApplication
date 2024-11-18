using System.Collections.Generic;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a user profile containing search history, category interactions, and viewed events
    /// </summary>
    public class CurrentUser
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        public string Id { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of search queries made by the user
        /// </summary>
        public List<string> SearchHistory { get; set; } = new List<string>();

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the dictionary tracking how many times the user has interacted with each category
        /// </summary>
        public Dictionary<string, int> CategoryInteractions { get; set; } = new Dictionary<string, int>();

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of event IDs that the user has viewed
        /// </summary>
        public List<string> ViewedEventIds { get; set; } = new List<string>();
//-------------------------------------------------------------------------------------------------------------
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//