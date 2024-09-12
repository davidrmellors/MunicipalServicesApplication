using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MunicipalServicesApplication.Models
{
    /// <summary>
    /// The Issue class represents a municipal issue reported by a user, including details like location, category, description, and any attached files.
    /// </summary>
    public class Issue
    {
        /// <summary>
        /// Gets or sets the location where the issue is reported.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the category of the reported issue (e.g., sanitation, roads).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the description of the issue reported by the user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of file attachments related to the reported issue.
        /// This list contains file paths of the attachments.
        /// </summary>
        public List<string> Attachments { get; set; } = new List<string>();

        //-------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the attachments as a comma-separated string of file names, instead of full file paths.
        /// If no attachments are available, it returns "No attachments".
        /// </summary>
        public string AttachmentsDisplay
        {
            get
            {
                // Return file names instead of full paths, or "No attachments" if there are none.
                return Attachments != null && Attachments.Count > 0
                    ? string.Join(", ", Attachments.Select(f => Path.GetFileName(f)))
                    : "No attachments";
            }
        }

        //-------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// A static collection of reported issues that can be accessed and updated globally.
        /// This collection stores all the issues that have been reported.
        /// </summary>
        public static ObservableCollection<Issue> ReportedIssues { get; set; } = new ObservableCollection<Issue>();

        //-------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a new issue to the ReportedIssues collection. The method takes the location, category, description, and attachments for the issue.
        /// If no attachments are provided, an empty list is assigned.
        /// </summary>
        /// <param name="location">The location where the issue occurred.</param>
        /// <param name="category">The category of the issue (e.g., sanitation, roads).</param>
        /// <param name="description">A description of the issue provided by the user.</param>
        /// <param name="attachments">An optional list of file paths representing the attachments for the issue.</param>
        public static void AddIssue(string location, string category, string description, List<string> attachments = null)
        {
            ReportedIssues.Add(new Issue
            {
                Location = location,
                Category = category,
                Description = description,
                Attachments = attachments ?? new List<string>() // If no attachments, assign an empty list.
            });
        }
    }
}
