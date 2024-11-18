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
using System.ComponentModel;
using System.Timers;

namespace MunicipalServicesApplication.Views
{
    public partial class ServiceRequestStatusView : UserControl
    {
        public event EventHandler BackToMainRequested;
        private readonly ServiceRequestManager _requestManager;
        private bool _isLoading;
        private System.Timers.Timer _searchTimer;
        private const int PAGE_SIZE = 10;
        private int currentPage = 0;
        private bool isLoadingMore = false;
        private List<ServiceRequest> loadedRequests = new List<ServiceRequest>();

        public ServiceRequestStatusView()
        {
            InitializeComponent();
            _requestManager = new ServiceRequestManager();
            SetupSearchTimer();
            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            await ShowLoadingAsync(async () =>
            {
                // Clear everything
                loadedRequests.Clear();
                currentPage = 0;
                isLoadingMore = false;
                
                // Remove and re-add scroll event to prevent multiple subscriptions
                MainScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                MainScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                
                // Load first page
                var requests = await LoadRequestsPage(0);
                RequestsItemsControl.ItemsSource = requests;
            });
        }

        private async Task<List<ServiceRequest>> LoadRequestsPage(int page)
        {
            var allRequests = await Task.Run(() => DatabaseService.Instance.GetAllRequests());
            var pagedRequests = allRequests
                .OrderByDescending(r => r.Priority)
                .Skip(page * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList();

            // Pre-load attachments and related issues
            foreach (var request in pagedRequests)
            {
                try
                {
                    request.Attachments = await Task.Run(() => 
                        DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId));
                    
                    request.RelatedIssues = await Task.Run(() => 
                        _requestManager.GetRelatedIssues(request.RequestId)?.ToList() ?? new List<ServiceRequest>());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading request details: {ex}");
                    request.Attachments = new List<Attachment>();
                    request.RelatedIssues = new List<ServiceRequest>();
                }
            }

            if (page == 0)
            {
                loadedRequests.Clear();
            }
            loadedRequests.AddRange(pagedRequests);
            return loadedRequests;
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 200 && !isLoadingMore)
            {
                isLoadingMore = true;
                currentPage++;
                var newRequests = await LoadRequestsPage(currentPage);
                
                if (newRequests.Any())
                {
                    RequestsItemsControl.ItemsSource = loadedRequests;
                }
                isLoadingMore = false;
            }
        }

        private void SetupSearchTimer()
        {
            _searchTimer = new System.Timers.Timer(300); // 300ms delay
            _searchTimer.Elapsed += async (s, e) =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    await PerformSearch(SearchRequestId.Text);
                });
            };
            _searchTimer.AutoReset = false;

            SearchRequestId.TextChanged += SearchRequestId_TextChanged;
        }

        private void SearchRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop(); // Reset the timer
            _searchTimer.Start(); // Start the timer again
        }

        private async Task PerformSearch(string searchText)
        {
            // Disable scroll event handler during search
            MainScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                searchText = "REQ";
            }

            await ShowLoadingAsync(async () =>
            {
                var allRequests = await Task.Run(() => DatabaseService.Instance.GetAllRequests());
                var filteredRequests = allRequests
                    .Where(r => r.RequestId.ToUpperInvariant().Contains(searchText.ToUpperInvariant()))
                    .OrderByDescending(r => r.Priority)
                    .ToList();

                // Pre-load attachments and related issues for filtered results
                foreach (var request in filteredRequests)
                {
                    try
                    {
                        request.Attachments = await Task.Run(() => 
                            DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId));
                        
                        request.RelatedIssues = await Task.Run(() => 
                            _requestManager.GetRelatedIssues(request.RequestId)?.ToList() ?? new List<ServiceRequest>());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading request details during search: {ex}");
                        request.Attachments = new List<Attachment>();
                        request.RelatedIssues = new List<ServiceRequest>();
                    }
                }

                // Update UI with search results
                loadedRequests = filteredRequests;
                RequestsItemsControl.ItemsSource = loadedRequests;
            });
        }

        private async Task ShowLoadingAsync(Func<Task> action)
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                LoadingOverlay.Visibility = Visibility.Visible;

                await action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex}");
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
                _isLoading = false;
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            BackToMainRequested?.Invoke(this, EventArgs.Empty);
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

        // Add this method to handle expander state changes
        private async void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander && expander.DataContext is ServiceRequest request)
            {
                try
                {
                    if (request.Attachments == null)
                    {
                        request.Attachments = await Task.Run(() => 
                            DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId));
                        Debug.WriteLine($"Loaded {request.Attachments?.Count ?? 0} attachments on expand");
                    }
                    
                    if (request.RelatedIssues == null)
                    {
                        request.RelatedIssues = await Task.Run(() => 
                            _requestManager.GetRelatedIssues(request.RequestId)?.ToList() ?? new List<ServiceRequest>());
                        Debug.WriteLine($"Loaded {request.RelatedIssues?.Count ?? 0} related issues on expand");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading request details: {ex}");
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