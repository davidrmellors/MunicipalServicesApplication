using System;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Threading.Tasks;

namespace MunicipalServicesApplication.Views
{
    public partial class MainWindow : Window
    {
        private readonly UserProfileService _userService;
        private UserControl _currentView;
        private DashboardView _dashboardView;
        private bool isMetricsWindowShown;
        public CurrentUser CurrentUser { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            _userService = new UserProfileService();
            
            // Show dashboard immediately
            _dashboardView = new DashboardView(App.CurrentUser);
            _dashboardView.NavigationRequested += OnNavigationRequested;
            SwitchView(_dashboardView);
        }

        private async Task InitializeApplicationAsync()
        {
            try
            {
                CurrentUser = _userService.GetOrCreateUser(Environment.UserName);

                // Initialize database first
                
                // Setup dashboard after data is initialized
                _dashboardView = new DashboardView(CurrentUser);
                _dashboardView.NavigationRequested += OnNavigationRequested;
                SwitchView(_dashboardView);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnNavigationRequested(object sender, string destination)
        {
            switch (destination)
            {
                case "Events":
                    SwitchView(new LocalEventsWindow());
                    break;
                case "ReportIssues":
                    SwitchView(new ReportIssuesWindow(CurrentUser));
                    break;
                case "Dashboard":
                    SwitchView(_dashboardView);
                    break;
                case "Status":
                    SwitchView(new ServiceRequestStatusView());
                    break;
            }
        }

        private void SwitchView(UserControl newView)
        {
            // Create the new view immediately
            MainContent.Children.Clear();
            MainContent.Children.Add(newView);
            _currentView = newView;

            // Wire up navigation events
            if (newView != _dashboardView)
            {
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

            // Refresh dashboard data in background when returning
            if (newView == _dashboardView)
            {
                _ = Task.Run(async () => await _dashboardView.RefreshData());
            }
        }

    }
}