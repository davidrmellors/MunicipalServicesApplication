using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class ServiceRequestStatusView : UserControl
    {
        public event EventHandler BackToMainRequested;
        private readonly ServiceRequestManager _requestManager;

        public ServiceRequestStatusView()
        {
            InitializeComponent();
            _requestManager = new ServiceRequestManager(DataManager.Instance);
            SearchRequestId.TextChanged += SearchRequestId_TextChanged;
            UpdateRequestsDisplay();
        }

        private void UpdateRequestsDisplay()
        {
            try
            {
                var allRequests = DatabaseService.Instance.GetAllRequests();
                
                // Load attachments for each request
                foreach (var request in allRequests)
                {
                    request.Attachments = DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId);
                }
                
                RequestsItemsControl.ItemsSource = allRequests.OrderByDescending(r => r.Priority);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchRequestId.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateRequestsDisplay();
                return;
            }

            try
            {
                var allRequests = DatabaseService.Instance.GetAllRequests();
                
                // Load attachments for each request
                foreach (var request in allRequests)
                {
                    request.Attachments = DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId);
                }
                
                var filteredRequests = allRequests.Where(r => r.MatchesSearch(searchText));
                RequestsItemsControl.ItemsSource = filteredRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Attachment attachment)
            {
                try
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(), attachment.Name);
                    File.WriteAllBytes(tempFilePath, attachment.Content);

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

        private void CopyRequestId_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                bool success = false;
                int retryCount = 3;
                while (!success && retryCount > 0)
                {
                    try
                    {
                        Clipboard.SetText(request.RequestId);
                        success = true;
                        MessageBox.Show("Request ID copied to clipboard!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (COMException)
                    {
                        retryCount--;
                        System.Threading.Thread.Sleep(100); // Wait a bit before retrying
                    }
                }

                if (!success)
                {
                    MessageBox.Show("Failed to copy Request ID to clipboard. Please try again.", "Error",
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