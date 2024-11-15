using System.Collections.Generic;

namespace MunicipalServices.Models
{
    public class CurrentUser
    {
        public string Id { get; set; }
        public List<string> SearchHistory { get; set; } = new List<string>();
        public Dictionary<string, int> CategoryInteractions { get; set; } = new Dictionary<string, int>();
        public List<string> ViewedEventIds { get; set; } = new List<string>();
    }
}