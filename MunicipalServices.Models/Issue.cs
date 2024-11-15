﻿using System;
using System.Collections.Generic;

namespace MunicipalServices.Models
{
    public class Issue
    {
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public DateTime ReportedDate { get; set; } = DateTime.Now;

        public static List<Issue> ReportedIssues { get; set; } = new List<Issue>();
    }
}
