using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class DashboardView : UserControl
    {
        private List<Issue> AllIssues;
        public event EventHandler<string> NavigationRequested;

        public DashboardView()
        {
            InitializeComponent();
            AllIssues = Issue.ReportedIssues.ToList();
            UpdateIssuesDisplay();
        }

        private void UpdateIssuesDisplay()
        {
            IssuesItemsControl.ItemsSource = AllIssues;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredIssues = AllIssues.Where(issue =>
                issue.Location.ToLower().Contains(searchText) ||
                issue.Category.ToLower().Contains(searchText) ||
                issue.Description.ToLower().Contains(searchText) ||
                issue.Attachments.Any(attachment => attachment.Name.ToLower().Contains(searchText))
            ).ToList();

            IssuesItemsControl.ItemsSource = filteredIssues;
        }

        private void ViewAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Attachment attachment)
            {
                try
                {
                    byte[] fileBytes = Convert.FromBase64String(attachment.Content);
                    string tempFilePath = Path.Combine(Path.GetTempPath(), attachment.Name);
                    File.WriteAllBytes(tempFilePath, fileBytes);

                    var psi = new ProcessStartInfo
                    {
                        FileName = tempFilePath,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening attachment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

        public void RefreshIssues()
        {
            AllIssues = Issue.ReportedIssues.ToList();
            UpdateIssuesDisplay();
        }
    }
}