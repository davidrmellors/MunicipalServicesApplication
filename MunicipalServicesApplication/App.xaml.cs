using MunicipalServicesApplication.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MunicipalServicesApplication.Models;

namespace MunicipalServicesApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static CurrentUser CurrentUser { get; set; }

        public EventService EventService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventService = new EventService();
            Task.Run(() => EventService.GetEventsAsync(null, 50));
        }
    }
}
