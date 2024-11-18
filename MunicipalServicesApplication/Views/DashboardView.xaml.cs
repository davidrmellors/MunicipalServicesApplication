using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Threading;
using System.Windows.Threading;

namespace MunicipalServicesApplication.Views
{
    public partial class DashboardView : UserControl
    {
        public event EventHandler<string> NavigationRequested;
        private readonly EmergencyNoticeTree noticesTree;
        private readonly ServiceStatusTree statusTree;
        private readonly CurrentUser currentUser;
        private readonly ServiceRequestManager _requestManager;
        private bool _isInitialized;

        public DashboardView(CurrentUser user)
        {
            InitializeComponent();
            currentUser = user;
            _requestManager = new ServiceRequestManager();
            noticesTree = EmergencyNoticeTree.Instance;
            statusTree = new ServiceStatusTree();
            
            // Show static data immediately
            EmergencyNoticesControl.ItemsSource = noticesTree.GetAll();
            ServiceStatusControl.ItemsSource = statusTree.GetAll();
            
            // Load dynamic data after UI is shown
            Loaded += (s, e) => 
            {
                if (!_isInitialized)
                {
                    _ = LoadDashboardDataAsync();
                    _isInitialized = true;
                }
            };
        }

        private async void ViewRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServiceRequest request)
            {
                try
                {
                    // Load full request details if not already loaded
                    if (request.RelatedIssues == null)
                    {
                        request.RelatedIssues = await Task.Run(() => 
                            _requestManager.GetRelatedIssues(request.RequestId)?.ToList() ?? new List<ServiceRequest>());
                    }
                    if (request.Attachments == null)
                    {
                        request.Attachments = await Task.Run(() => 
                            DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId));
                    }

                    // Navigate to detailed view
                    NavigationRequested?.Invoke(this, $"RequestDetails/{request.RequestId}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading request details: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Load all requests
                var allRequests = await Task.Run(() => DatabaseService.Instance.GetAllRequests());
                Debug.WriteLine($"Total requests loaded: {allRequests.Count}");
                Debug.WriteLine($"Current user ID: {currentUser.Id}");
                
                // Debug each request's UserId
                foreach (var request in allRequests)
                {
                    Debug.WriteLine($"Request {request.RequestId}: UserId = {request.UserId}");
                }

                // Get critical alerts (priority >= 8)
                var criticalAlerts = allRequests
                    .Where(r => r.Priority >= 8 && r.Status == "Pending")  // Only show pending critical alerts
                    .OrderByDescending(r => r.Priority)
                    .ToList();
                Debug.WriteLine($"Critical alerts: {criticalAlerts.Count}");

                // Get user's active requests
                var userRequests = allRequests
                    .Where(r => r.UserId == currentUser.Id)
                    .ToList();
                Debug.WriteLine($"Requests for current user: {userRequests.Count}");
                
                var pendingUserRequests = userRequests
                    .Where(r => r.Status == "Pending")
                    .OrderByDescending(r => r.SubmissionDate)
                    .ToList();
                Debug.WriteLine($"Pending requests for current user: {pendingUserRequests.Count}");

                // Calculate category statistics
                var categoryStats = allRequests
                    .Where(r => r.Status == "Pending")
                    .GroupBy(r => r.Category)
                    .Select(g => new ServiceStatus 
                    { 
                        Service = g.Key,
                        Count = g.Count(),
                        Status = "Issues Reported",
                        StatusColor = "#E03C31"  // Red for issues
                    })
                    .ToList();

                // Add categories with no issues
                var allCategories = new[] { "Public Safety", "Roads", "Water", "Electricity", "Noise Complaints" };
                foreach (var category in allCategories)
                {
                    if (!categoryStats.Any(s => s.Service == category))
                    {
                        categoryStats.Add(new ServiceStatus
                        {
                            Service = category,
                            Count = 0,
                            Status = "Operational",
                            StatusColor = "#007A4D"  // Green for operational
                        });
                    }
                }

                // Calculate service delivery statistics
                var totalRequests = allRequests.Count;
                var totalPending = allRequests.Count(r => r.Status == "Pending");
                var resolvedThisMonth = allRequests.Count(r => 
                    r.Status == "Resolved" && 
                    r.ResolvedDate?.Month == DateTime.Now.Month &&
                    r.ResolvedDate?.Year == DateTime.Now.Year);

                await Dispatcher.InvokeAsync(() =>
                {
                    EmergencyNoticesControl.ItemsSource = criticalAlerts;
                    ServiceStatusControl.ItemsSource = categoryStats.OrderBy(s => s.Service);
                    RecentRequestsControl.ItemsSource = pendingUserRequests;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadDashboardDataAsync: {ex}");
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error loading dashboard data: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private string DetermineStatus(int count)
        {
            if (count > 0) return "Issues Reported";  // If there are any active requests
            return "Operational";
        }

        private string DetermineStatusColor(int count)
        {
            if (count > 0) return "#E03C31";  // Red if there are issues
            return "#007A4D";  // Green if operational
        }

        private void UpdateStatistics(List<ServiceRequest> userRequests)
        {
            var activeRequests = userRequests.Count(r => r.Status == "Pending");
            var resolvedThisMonth = userRequests.Count(r => 
                r.Status == "Resolved" && 
                r.ResolvedDate?.Month == DateTime.Now.Month);
        }

        private async Task LoadAdditionalDataAsync(List<ServiceRequest> requests)
        {
            foreach (var request in requests)
            {
                try
                {
                    request.RelatedIssues = await Task.Run(() => 
                        _requestManager.GetRelatedIssues(request.RequestId)?.ToList() ?? new List<ServiceRequest>());
                    request.Attachments = await Task.Run(() => 
                        DatabaseService.Instance.GetAttachmentsForRequest(request.RequestId));
                }
                catch (Exception)
                {
                    // Log error but continue loading other requests
                    Debug.WriteLine($"Error loading additional data for request {request.RequestId}");
                }
            }
        }

        private double CalculateProgressPercentage(int value, int total)
        {
            if (total == 0) return 0;
            return (double)value / total * 100;
        }

        private string CalculateAverageResponseTime(List<ServiceRequest> requests)
        {
            var resolvedRequests = requests.Where(r => r.Status == "Resolved" && r.ResolvedDate.HasValue);
            if (!resolvedRequests.Any()) return "N/A";

            var avgTicks = resolvedRequests
                .Average(r => (r.ResolvedDate.Value - r.SubmissionDate).Ticks);
            var avgTimeSpan = TimeSpan.FromTicks((long)avgTicks);

            return avgTimeSpan.TotalDays >= 1 
                ? $"{avgTimeSpan.TotalDays:F1} days"
                : $"{avgTimeSpan.TotalHours:F1} hours";
        }

        private double CalculateResponseTimeProgress(List<ServiceRequest> requests)
        {
            var resolvedRequests = requests.Where(r => r.Status == "Resolved" && r.ResolvedDate.HasValue);
            if (!resolvedRequests.Any()) return 0;

            var avgDays = resolvedRequests
                .Average(r => (r.ResolvedDate.Value - r.SubmissionDate).TotalDays);
            
            // Consider 7 days as target response time (100%)
            const double targetDays = 7.0;
            return Math.Min(100, (targetDays - avgDays) / targetDays * 100);
        }

        public async Task RefreshData()
        {
            await LoadDashboardDataAsync();
        }

        private async void CopyRequestId_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is ServiceRequest request)
            {
                if (await TryCopyToClipboard(request.RequestId))
                {
                    MessageBox.Show("Request ID copied to clipboard!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to copy Request ID. Please try again.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<bool> TryCopyToClipboard(string text)
        {
            int retryCount = 3;
            while (retryCount > 0)
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        // Try different clipboard methods
                        try
                        {
                            // Method 1: DataObject
                            var dataObject = new DataObject();
                            dataObject.SetData(DataFormats.Text, text);
                            Clipboard.SetDataObject(dataObject, true);
                        }
                        catch
                        {
                            try
                            {
                                // Method 2: Direct text
                                Clipboard.SetText(text);
                            }
                            catch
                            {
                                // Method 3: Thread-safe alternative
                                Thread.Sleep(100); // Brief pause
                                var thread = new Thread(() =>
                                {
                                    try
                                    {
                                        Clipboard.SetDataObject(text, true);
                                    }
                                    catch { }
                                });
                                thread.SetApartmentState(ApartmentState.STA);
                                thread.Start();
                                thread.Join();
                            }
                        }
                    }, DispatcherPriority.Normal);

                    // Verify the copy was successful
                    string clipboardText = await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            return Clipboard.GetText();
                        }
                        catch
                        {
                            return null;
                        }
                    });

                    if (clipboardText == text)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Clipboard error (attempt {4 - retryCount}/3): {ex.Message}");
                }

                retryCount--;
                if (retryCount > 0)
                {
                    await Task.Delay(100); // Wait before retry
                }
            }

            return false;
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