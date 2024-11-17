using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class ServiceRequestStatusView : UserControl
    {
        public event EventHandler BackToMainRequested;
        private readonly ServiceRequestBST requestsBST;

        public ServiceRequestStatusView()
        {
            InitializeComponent();
            requestsBST = new ServiceRequestBST();
            SearchRequestId.TextChanged += SearchRequestId_TextChanged;
            UpdateRequestsDisplay();
        }

        private void UpdateRequestsDisplay()
        {
            var requests = requestsBST.GetAll();
            if (!requests.Any())
            {
                RequestsItemsControl.ItemsSource = null;
                return;
            }
            RequestsItemsControl.ItemsSource = requests;
        }

        private void SearchRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchRequestId.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateRequestsDisplay();
                return;
            }

            var requests = requestsBST.GetAll()
                .Where(r => r.RequestId.ToLower().Contains(searchText))
                .ToList();
            RequestsItemsControl.ItemsSource = requests;
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