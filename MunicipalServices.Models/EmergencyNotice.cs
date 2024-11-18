using System;

namespace MunicipalServices.Models
{
    public class EmergencyNotice
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RelatedRequestId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Severity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
