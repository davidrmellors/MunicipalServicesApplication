using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a service request submitted by a user, with details about the issue and its status
    /// </summary>
    public class ServiceRequest : IComparable<ServiceRequest>, INotifyPropertyChanged
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the unique identifier for the service request
        /// </summary>
        public string RequestId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the category/type of the service request
        /// </summary>
        public string Category { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the ID of the user who submitted the request
        /// </summary>
        public string UserId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the detailed description of the issue
        /// </summary>
        public string Description { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the location description of the issue
        /// </summary>
        public string Location { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the current status of the request
        /// </summary>
        public string Status { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the date when the request was submitted
        /// </summary>
        public DateTime SubmissionDate { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the date when the request was resolved, if applicable
        /// </summary>
        public DateTime? ResolvedDate { get; set; }
        private int _priority;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the priority level of the request
        /// </summary>
        public int Priority
        {
            get => _priority;
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }

        private List<Attachment> _attachments;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of attachments associated with the request
        /// </summary>
        public List<Attachment> Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                OnPropertyChanged(nameof(Attachments));
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the specific type of the service request
        /// </summary>
        public string Type { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the creation timestamp of the request
        /// </summary>
        public DateTime CreatedAt { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the latitude coordinate of the issue location
        /// </summary>
        public double Latitude { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the longitude coordinate of the issue location
        /// </summary>
        public double Longitude { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the formatted address of the issue location
        /// </summary>
        public string FormattedAddress { get; set; }

        private List<ServiceRequest> _relatedIssues;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Event that is raised when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of service requests related to this request
        /// </summary>
        public List<ServiceRequest> RelatedIssues
        {
            get => _relatedIssues;
            set
            {
                _relatedIssues = value;
                OnPropertyChanged(nameof(RelatedIssues));
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the coordinates of the issue location
        /// </summary>
        public Coordinates Coordinates => new Coordinates 
        { 
            Latitude = this.Latitude, 
            Longitude = this.Longitude 
        };

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the creation date of the request (alias for SubmissionDate)
        /// </summary>
        public DateTime CreatedDate => SubmissionDate;

        private static int counter = 0;
        private static readonly object lockObject = new object();

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceRequest class
        /// </summary>
        public ServiceRequest()
        {
            _relatedIssues = new List<ServiceRequest>();
            lock (lockObject)
            {
                counter++;
                // Format: REQ-{Category First 3 Letters}-{Counter}
                RequestId = $"REQ-{Category?.Substring(0, Math.Min(3, Category?.Length ?? 0)).ToUpper() ?? "XXX"}-{counter:D4}";
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if the request matches the given search text
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <returns>True if the request matches the search text, false otherwise</returns>
        public bool MatchesSearch(string searchText)
        {
            searchText = searchText.ToLower();
            return RequestId.ToLower().Contains(searchText) ||
                   Category.ToLower().Contains(searchText) ||
                   Description.ToLower().Contains(searchText) ||
                   Location.ToLower().Contains(searchText) ||
                   Status.ToLower().Contains(searchText);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Compares this request with another based on priority
        /// </summary>
        /// <param name="other">The other service request to compare with</param>
        /// <returns>A value indicating the relative priority order</returns>
        public int CompareTo(ServiceRequest other)
        {
            // Higher priority numbers are more important
            return other.Priority.CompareTo(this.Priority);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the priority level based on the request category
        /// </summary>
        public void CalculatePriority()
        {
            if (Category == null)
            {
                Priority = 1;
                return;
            }

            switch (Category.ToLower())
            {
                case "public safety":
                    Priority = 8;
                    break;
                case "utilities":
                    Priority = 4;
                    break;
                case "sanitation":
                case "roads":
                    Priority = 3;
                    break;
                case "environmental issues":
                    Priority = 2;
                    break;
                default:
                    Priority = 1;
                    break;
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//