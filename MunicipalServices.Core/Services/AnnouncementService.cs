using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MunicipalServices.Models;

namespace MunicipalServices.Core.Services
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Service for managing and retrieving municipal announcements.
    /// Provides functionality to access public announcements and notifications.
    /// </summary>
    public class AnnouncementService
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves a list of current municipal announcements asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of announcements ordered by date.
        /// </returns>
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

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//