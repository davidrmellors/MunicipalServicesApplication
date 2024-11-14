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
    public partial class MainWindow : Window
    {
        private List<Issue> AllIssues;

        public MainWindow()
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
            LocalEventsWindow localEventsWindow = new LocalEventsWindow();
            localEventsWindow.Width = this.Width;
            localEventsWindow.Height = this.Height;
            this.Hide(); // Hide the MainWindow
            localEventsWindow.Closed += (s, args) =>
            {
                this.Show(); // Show the MainWindow when LocalEventsWindow is closed
            };
            localEventsWindow.Show();
        }

        private void BtnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            ReportIssuesWindow reportIssuesWindow = new ReportIssuesWindow();
            reportIssuesWindow.Width = this.Width;
            reportIssuesWindow.Height = this.Height;
            this.Hide();
            reportIssuesWindow.Closed += (s, args) =>
            {
                this.Show();
                AllIssues = Issue.ReportedIssues.ToList();
                UpdateIssuesDisplay();
            };
            reportIssuesWindow.Show();
        }


        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Service Request Status functionality is under development.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
