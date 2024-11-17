using System;
using System.Collections.Generic;

namespace MunicipalServices.Models
{
    public class ServiceRequest : IComparable<ServiceRequest>
    {
        public string RequestId { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string Category { get; set; }
        public string Location { get; set; }
        public int Priority { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

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
                    Priority = 5;
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
    }
}