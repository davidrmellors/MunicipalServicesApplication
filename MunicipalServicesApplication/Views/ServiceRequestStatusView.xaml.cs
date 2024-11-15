using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class ServiceRequestStatusView : UserControl
    {
        public event EventHandler BackToMainRequested;

        public ServiceRequestStatusView()
        {
            InitializeComponent();
            SearchRequestId.TextChanged += SearchRequestId_TextChanged;
            UpdateRequestsDisplay();
        }

        private void UpdateRequestsDisplay()
        {
            var issues = Issue.ReportedIssues.ToList();
            if (!issues.Any())
            {
                RequestsItemsControl.ItemsSource = null;
                return;
            }
            RequestsItemsControl.ItemsSource = issues;
        }

        private void SearchRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchRequestId.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateRequestsDisplay();
                return;
            }

            var filteredIssues = Issue.ReportedIssues
                .Where(i => i.Location.ToLower().Contains(searchText) ||
                            i.Category.ToLower().Contains(searchText) ||
                            i.Description.ToLower().Contains(searchText))
                .ToList();

            RequestsItemsControl.ItemsSource = filteredIssues;
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
                    MessageBox.Show($"Error opening attachment: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            BackToMainRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}