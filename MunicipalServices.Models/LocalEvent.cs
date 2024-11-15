using System;

namespace MunicipalServices.Models
{
    public class LocalEvent
    {
        public bool IsRecommended { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; } // Add this property
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
    }
}
