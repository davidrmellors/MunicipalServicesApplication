using MunicipalServices.Core.Services;
using System.Threading.Tasks;
using System.Windows;
using MunicipalServices.Models;
using System;
using MunicipalServicesApplication.Views;

namespace MunicipalServicesApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static CurrentUser CurrentUser { get; set; }

        public EventService EventService { get; private set; }
        public AnnouncementService AnnouncementService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventService = new EventService();
            AnnouncementService = new AnnouncementService();
            Task.Run(() => EventService.GetEventsAsync(null, 50));
        }
    }
}
