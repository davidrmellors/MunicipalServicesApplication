using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MunicipalServices.Models
{
    public class ServiceRequest : IComparable<ServiceRequest>, INotifyPropertyChanged
    {
        public string RequestId { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        private int _priority;
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
        public List<Attachment> Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                OnPropertyChanged(nameof(Attachments));
            }
        }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FormattedAddress { get; set; }

        private List<ServiceRequest> _relatedIssues;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ServiceRequest> RelatedIssues
        {
            get => _relatedIssues;
            set
            {
                _relatedIssues = value;
                OnPropertyChanged(nameof(RelatedIssues));
            }
        }

        public Coordinates Coordinates => new Coordinates 
        { 
            Latitude = this.Latitude, 
            Longitude = this.Longitude 
        };

        public DateTime CreatedDate => SubmissionDate;

        private static int counter = 0;
        private static readonly object lockObject = new object();

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

        public bool MatchesSearch(string searchText)
        {
            searchText = searchText.ToLower();
            return RequestId.ToLower().Contains(searchText) ||
                   Category.ToLower().Contains(searchText) ||
                   Description.ToLower().Contains(searchText) ||
                   Location.ToLower().Contains(searchText) ||
                   Status.ToLower().Contains(searchText);
        }

        public int CompareTo(ServiceRequest other)
        {
            // Higher priority numbers are more important
            return other.Priority.CompareTo(this.Priority);
        }

        // Calculate priority based on category
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}