using System;
using System.Collections.Generic;

namespace MunicipalServicesApplication.Models
{
    public class Issue
    {
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
        public DateTime ReportedDate { get; set; } = DateTime.Now;

        public static List<Issue> ReportedIssues { get; set; } = new List<Issue>();
    }
}
