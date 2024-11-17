using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class DashboardView : UserControl
    {
        public event EventHandler<string> NavigationRequested;
        private readonly ServiceRequestBST requestsBST;
        private readonly EmergencyNoticeTree noticesTree;
        private readonly ServiceStatusTree statusTree;
        public DashboardView()
        {
            InitializeComponent();
            requestsBST = new ServiceRequestBST();
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
            var highPriorityRequests = requestsBST.GetHighestPriorityRequests(5)
                .OrderByDescending(r => r.RequestId) // Show newest first
                .ToList();
            RecentRequestsControl.ItemsSource = highPriorityRequests;
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
            var stats = new[]
            {
                new CommunityStat { Label = "Active Requests", Value = requestsBST.CountByStatus("Pending").ToString() },
                new CommunityStat { Label = "Resolved This Week", Value = requestsBST.CountByStatus("Resolved").ToString() }
            };
            StatsControl.ItemsSource = stats;
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

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}