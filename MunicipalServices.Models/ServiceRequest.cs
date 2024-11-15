using System;

namespace MunicipalServices.Models
{
    public class ServiceRequest : IComparable<ServiceRequest>
    {
        public string RequestId { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public int Priority { get; set; }

        public int CompareTo(ServiceRequest other)
        {
            return this.Priority.CompareTo(other.Priority);
        }
    }
}