using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.Services;

namespace MunicipalServicesApplication.Views
{
    public partial class MainWindow : Window
    {
        private readonly UserProfileService _userService;
        private UserControl _currentView;
        private DashboardView _dashboardView;

        public MainWindow()
        {
            InitializeComponent();
            _userService = new UserProfileService();
            _dashboardView = new DashboardView();
            _dashboardView.NavigationRequested += OnNavigationRequested;
            SwitchView(_dashboardView);
        }

        private void OnNavigationRequested(object sender, string destination)
        {
            switch (destination)
            {
                case "Events":
                    SwitchView(new LocalEventsWindow());
                    break;
                case "ReportIssues":
                    SwitchView(new ReportIssuesWindow());
                    break;
                case "Dashboard":
                    _dashboardView.RefreshIssues();
                    SwitchView(_dashboardView);
                    break;
                case "Status":
                    SwitchView(new ServiceRequestStatusView());
                    break;
            }
        }

        private void SwitchView(UserControl newView)
        {
            if (newView != _dashboardView)
            {
                // Wire up navigation for other views
                if (newView is LocalEventsWindow eventsWindow)
                {
                    eventsWindow.BackToMainRequested += (s, e) => SwitchView(_dashboardView);
                }
                else if (newView is ReportIssuesWindow reportWindow)
                {
                    reportWindow.BackToMainRequested += (s, e) => SwitchView(_dashboardView);
                }
                else if (newView is ServiceRequestStatusView statusWindow)
                {
                    statusWindow.BackToMainRequested += (s, e) => SwitchView(_dashboardView);
                }
            }

            MainContent.Children.Clear();
            MainContent.Children.Add(newView);
            _currentView = newView;
        }
    }
}