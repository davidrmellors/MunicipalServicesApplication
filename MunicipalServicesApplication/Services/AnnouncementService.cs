using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MunicipalServicesApplication.Models;

namespace MunicipalServicesApplication.Services
{
    public class AnnouncementService
    {
        public Task<List<Announcement>> GetAnnouncementsAsync()
        {
            var announcements = new List<Announcement>
            {
                new Announcement { Title = "City-wide water conservation efforts", Url = "https://www.capetown.gov.za/water-conservation", Date = DateTime.Now.AddDays(-2) },
                new Announcement { Title = "New recycling program launch", Url = "https://www.capetown.gov.za/recycling", Date = DateTime.Now.AddDays(-1) },
                new Announcement { Title = "Upcoming town hall meeting", Url = "https://www.capetown.gov.za/town-hall", Date = DateTime.Now },
                new Announcement { Title = "Road maintenance schedule", Url = "https://www.capetown.gov.za/road-maintenance", Date = DateTime.Now.AddDays(1) },
                new Announcement { Title = "Summer events calendar released", Url = "https://www.capetown.gov.za/summer-events", Date = DateTime.Now.AddDays(2) }
            };

            return Task.FromResult(announcements);
        }
    }
}