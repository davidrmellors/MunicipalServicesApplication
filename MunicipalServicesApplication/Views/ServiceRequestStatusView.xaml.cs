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
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;

namespace MunicipalServicesApplication.Views
{
    public partial class ServiceRequestStatusView : UserControl
    {
        public event EventHandler BackToMainRequested;
        private readonly ServiceRequestManager _requestManager;

        public ServiceRequestStatusView()
        {
            InitializeComponent();
            _requestManager = new ServiceRequestManager();
            SearchRequestId.TextChanged += SearchRequestId_TextChanged;
            UpdateRequestsDisplay();
        }

        private void SearchRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchRequestId.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateRequestsDisplay();
                RequestDetailsPanel.DataContext = null;
                return;
            }

            try
            {
                var allRequests = DatabaseService.Instance.GetAllRequests();
                var filteredRequests = allRequests
                    .Where(r => r.MatchesSearch(searchText))
                    .ToList();

                foreach (var request in filteredRequests)
                {
                    request.Attachments = DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId);
                    request.RelatedIssues = _requestManager.GetRelatedIssues(request.RequestId);
                }

                RequestsItemsControl.ItemsSource = filteredRequests.OrderByDescending(r => r.Priority);
                
                // Set details panel for exact match
                var exactMatch = filteredRequests.FirstOrDefault(r => r.RequestId.Equals(searchText, StringComparison.OrdinalIgnoreCase));
                if (exactMatch != null)
                {
                    RequestDetailsPanel.DataContext = exactMatch;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRequestsDisplay()
        {
            try
            {
                var allRequests = DatabaseService.Instance.GetAllRequests()
                    .OrderByDescending(r => r.Priority)
                    .ToList();
                
                foreach (var request in allRequests)
                {
                    // Always load attachments to ensure they're fresh
                    request.Attachments = DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId);
                    Debug.WriteLine($"Loaded {request.Attachments?.Count ?? 0} attachments for request {request.RequestId}");
                    
                    if (request.RelatedIssues == null)
                    {
                        request.RelatedIssues = _requestManager.GetRelatedIssues(request.RequestId);
                    }
                }
                
                RequestsItemsControl.ItemsSource = allRequests;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateRequestsDisplay: {ex}");
                MessageBox.Show($"Error loading requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewAttachment_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ViewAttachment_Click triggered");
            if (sender is Button button)
            {
                Debug.WriteLine($"Button DataContext type: {button.DataContext?.GetType().Name ?? "null"}");
                if (button.DataContext is Attachment attachment)
                {
                    try
                    {
                        Debug.WriteLine($"Opening attachment: {attachment.Name}");
                        
                        if (string.IsNullOrEmpty(attachment.ContentBase64))
                        {
                            MessageBox.Show("Attachment content is empty", "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string tempFilePath = Path.Combine(Path.GetTempPath(), attachment.Name);
                        Debug.WriteLine($"Saving to temp path: {tempFilePath}");
                        
                        byte[] fileBytes = Convert.FromBase64String(attachment.ContentBase64);
                        File.WriteAllBytes(tempFilePath, fileBytes);

                        var psi = new ProcessStartInfo
                        {
                            FileName = tempFilePath,
                            UseShellExecute = true,
                            Verb = "open"
                        };
                        Process.Start(psi);
                        
                        Debug.WriteLine($"Successfully opened attachment: {attachment.Name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error opening attachment: {ex}");
                        MessageBox.Show($"Error opening attachment: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Debug.WriteLine("Button DataContext is not an Attachment");
                }
            }
        }

        private async void CopyRequestId_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                int retryCount = 3;
                while (retryCount > 0)
                {
                    try
                    {
                        // Use a dispatcher frame to ensure synchronous execution
                        var frame = new DispatcherFrame();
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            try
                            {
                                Clipboard.SetDataObject(request.RequestId, true);
                                MessageBox.Show("Request ID copied to clipboard!", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception)
                            {
                                // Swallow the exception and let the retry logic handle it
                            }
                            finally
                            {
                                frame.Continue = false;
                            }
                        }));
                        
                        // Wait for the dispatcher frame to complete
                        Dispatcher.PushFrame(frame);
                        return; // Success - exit the method
                    }
                    catch (Exception)
                    {
                        retryCount--;
                        if (retryCount > 0)
                        {
                            await Task.Delay(100); // Wait before retry
                        }
                    }
                }

                // If we get here, all retries failed
                MessageBox.Show("Failed to copy Request ID to clipboard. Please try again.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            BackToMainRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchRequestId_TextChanged(SearchRequestId, null);
        }

        private void ViewRelatedRequests_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                try
                {
                    // Get related requests using the graph
                    var relatedRequests = _requestManager.GetRelatedIssues(request.RequestId).ToList();
                    
                    // Update the request's RelatedIssues property
                    request.RelatedIssues = relatedRequests;
                    
                    // Find the parent expander
                    var grid = button.Parent as Grid;
                    var expander = grid?.Parent as Expander;
                    
                    if (expander != null)
                    {
                        expander.IsExpanded = true;
                    }
                    
                    // Debug information
                    Debug.WriteLine($"Found {relatedRequests.Count()} related requests for {request.RequestId}");
                    foreach (var related in relatedRequests)
                    {
                        Debug.WriteLine($"Related request: {related.RequestId} - {related.Category}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in ViewRelatedRequests_Click: {ex}");
                    MessageBox.Show($"Error loading related requests: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewAttachmentsExpander_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                try
                {
                    Debug.WriteLine($"Attempting to expand attachments for request {request.RequestId}");
                    
                    // Find the parent Card that contains both button and expander
                    var parentElement = button.Parent as FrameworkElement;
                    while (parentElement != null && !(parentElement is MaterialDesignThemes.Wpf.Card))
                    {
                        parentElement = VisualTreeHelper.GetParent(parentElement) as FrameworkElement;
                        Debug.WriteLine($"Traversing visual tree: {parentElement?.GetType().Name}");
                    }

                    if (parentElement != null)
                    {
                        // Find all expanders within this Card
                        var expanders = FindVisualChildren<Expander>(parentElement);
                        var attachmentsExpander = expanders.FirstOrDefault(exp => exp.Header?.ToString() == "Attachments");
                        
                        if (attachmentsExpander != null)
                        {
                            attachmentsExpander.IsExpanded = true;
                            Debug.WriteLine("Successfully expanded attachments section");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in ViewAttachmentsExpander_Click: {ex}");
                }
            }
        }

        // Helper method to find visual children of a specific type
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void VerifyAttachments(ServiceRequest request)
        {
            if (request != null)
            {
                Debug.WriteLine($"Request {request.RequestId} has {request.Attachments?.Count ?? 0} attachments");
                if (request.Attachments != null)
                {
                    foreach (var attachment in request.Attachments)
                    {
                        Debug.WriteLine($"Attachment: {attachment.Name}, Content length: {attachment.ContentBase64?.Length ?? 0}");
                    }
                }
            }
        }

    }
}