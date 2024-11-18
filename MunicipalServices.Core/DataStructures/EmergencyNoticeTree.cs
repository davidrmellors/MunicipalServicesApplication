using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public class EmergencyNoticeTree
    {
        private static EmergencyNoticeTree _instance;
        private readonly List<EmergencyNotice> _notices;
        
        public static EmergencyNoticeTree Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmergencyNoticeTree();
                }
                return _instance;
            }
        }

        private EmergencyNoticeTree()
        {
            _notices = new List<EmergencyNotice>
            {
                new EmergencyNotice 
                { 
                    Title = "Planned Water Interruption",
                    Description = "Maintenance work scheduled for Central District - 15 April, 08:00-16:00",
                    Severity = "Warning"
                },
                new EmergencyNotice 
                { 
                    Title = "Power Outage Alert",
                    Description = "Emergency repairs in Progress - Northern Suburbs affected",
                    Severity = "Critical"
                }
            };
        }

        public List<EmergencyNotice> GetAll() => _notices;
        
        public void Insert(EmergencyNotice notice)
        {
            _notices.Add(notice);
        }
    }
}
