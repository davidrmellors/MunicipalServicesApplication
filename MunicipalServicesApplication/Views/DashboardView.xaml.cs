using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class DashboardView : UserControl
    {
        public event EventHandler<string> NavigationRequested;
        private readonly ServiceRequestBST requestsBST;
        private readonly EmergencyNoticeTree noticesTree;
        private readonly ServiceStatusTree statusTree;
        private readonly CurrentUser currentUser;
        private readonly ServiceRequestManager _requestManager;

        public DashboardView(CurrentUser user)
        {
            InitializeComponent();
            currentUser = user;
            _requestManager = new ServiceRequestManager(DataManager.Instance);
            noticesTree = EmergencyNoticeTree.Instance;
            statusTree = new ServiceStatusTree();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            LoadEmergencyNotices();
            LoadServiceStatus();
            LoadRecentRequests();
            LoadCommunityStats();
        }

        private void HandleNewRequest(ServiceRequest request)
        {
            _requestManager.ProcessNewRequest(request);

            // Get related requests to show on dashboard
            var relatedRequests = _requestManager.GetRelatedRequests(request.RequestId);
            RelatedRequestsControl.ItemsSource = relatedRequests;
        }

        private void LoadEmergencyNotices()
        {
            EmergencyNoticesControl.ItemsSource = noticesTree.GetAll();
        }

        private void LoadServiceStatus()
        {
            ServiceStatusControl.ItemsSource = statusTree.GetAll();
        }

        private void LoadRecentRequests()
        {
            try
            {
                var allRequests = DatabaseService.Instance.GetAllRequests();
                var recentRequests = allRequests
                    .OrderByDescending(r => r.Priority)
                    .Take(5)
                    .ToList();
                RecentRequestsControl.ItemsSource = recentRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyRequestId_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                Clipboard.SetText(request.RequestId);
                MessageBox.Show("Request ID copied to clipboard!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadCommunityStats()
        {
            var allRequests = DatabaseService.Instance.GetAllRequests();
            var stats = new[]
            {
                new CommunityStat {
                    Label = "Active Requests",
                    Value = allRequests.Count(r => r.Status == "Pending").ToString()
                },
                new CommunityStat {
                    Label = "Resolved This Week",
                    Value = allRequests.Count(r => r.Status == "Resolved" &&
                                                   r.SubmissionDate >= DateTime.Now.AddDays(-7)).ToString()
                }
            };
            StatsControl.ItemsSource = stats;
        }

        public void RefreshData()
        {
            LoadDashboardData();
        }

        private void BtnEvents_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "Events");
        }

        private void BtnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "ReportIssues");
        }

        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "Status");
        }

        private void BtnPerformance_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, "Performance");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}