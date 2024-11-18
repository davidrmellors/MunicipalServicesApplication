using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using MunicipalServices.Models;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Core.Services;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MunicipalServicesApplication.Views
{
    public partial class ReportIssuesWindow : UserControl
    {
        private List<Attachment> attachments = new List<Attachment>();
        public event EventHandler BackToMainRequested;
        private readonly CurrentUser currentUser;
        private double selectedLatitude;
        private double selectedLongitude;
        private string formattedAddress;
        private readonly GooglePlacesService placesService;
        private readonly ServiceRequestManager _requestManager;

        public ReportIssuesWindow(CurrentUser user)
        {
            InitializeComponent();
            attachments = new List<Attachment>();
            currentUser = user;
            placesService = new GooglePlacesService("AIzaSyCyt7gd-FzBDBA9h3r7AwNH5Zp40Rh1EhI");
            
            selectedLatitude = 0;
            selectedLongitude = 0;
            formattedAddress = string.Empty;
            _requestManager = new ServiceRequestManager();
        }

        private async void Location_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LocationSuggestionsList == null) return;
            
            if (string.IsNullOrWhiteSpace(TxtLocation.Text) || TxtLocation.Text == "Enter Location")
            {
                LocationSuggestionsList.Visibility = Visibility.Collapsed;
                return;
            }

            if (LocationSuggestionsList.SelectedItem != null)
            {
                LocationSuggestionsList.SelectedItem = null;
                return;
            }

            try
            {

                var predictions = await placesService.GetPlacePredictions(TxtLocation.Text);
                if (predictions != null && predictions.Any())
                {
                    LocationSuggestionsList.ItemsSource = predictions;
                    LocationSuggestionsList.Visibility = Visibility.Visible;
                }
                else
                {
                    LocationSuggestionsList.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting location predictions: {ex.Message}");
                LocationSuggestionsList.Visibility = Visibility.Collapsed;
            }
        }

        

        private async void LocationSuggestion_Selected(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = LocationSuggestionsList.SelectedItem;
            if (selectedItem is PlacePrediction prediction)
            {
                try
                {
                    
                    var placeDetails = await placesService.GetPlaceDetails(prediction.PlaceId);
                    
                    selectedLatitude = placeDetails.Latitude;
                    selectedLongitude = placeDetails.Longitude;
                    formattedAddress = placeDetails.FormattedAddress;
                    
                    TxtLocation.Text = prediction.Description;
                    LocationSuggestionsList.Visibility = Visibility.Collapsed;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error getting location details: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AttachFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Select Files to Attach";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {
                        byte[] fileBytes = File.ReadAllBytes(filename);
                        var attachment = new Attachment 
                        { 
                            Name = Path.GetFileName(filename),
                            ContentBase64 = Convert.ToBase64String(fileBytes)
                        };
                        attachments.Add(attachment);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error attaching file {filename}: {ex.Message}", 
                            "Error", 
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                UpdateAttachmentCount();
            }
        }

        private void UpdateAttachmentCount()
        {
            TxtAttachmentCount.Text = $"{attachments.Count} file(s) attached";
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLocation.Text) || TxtLocation.Text == "Enter Location" ||
                string.IsNullOrWhiteSpace(TxtDescription.Text) || TxtDescription.Text == "Enter Description" ||
                CmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var request = new ServiceRequest
                {
                    RequestId = GenerateRequestId(),
                    Location = formattedAddress ?? TxtLocation.Text,
                    Category = (CmbCategory.SelectedItem as ComboBoxItem).Content.ToString(),
                    Description = TxtDescription.Text,
                    Status = "Pending",
                    UserId = currentUser?.Id ?? "anonymous",
                    SubmissionDate = DateTime.Now,
                    Latitude = selectedLatitude,
                    Longitude = selectedLongitude,
                    FormattedAddress = formattedAddress
                };

                request.CalculatePriority();

                // Save attachments first
                foreach (var attachment in attachments)
                {
                    attachment.RequestId = request.RequestId;
                }
                request.Attachments = attachments;

                // Process the request with all data
                _requestManager.ProcessNewRequest(request);

                MessageBox.Show($"Service request submitted successfully!\nRequest ID: {request.RequestId}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
                BackToMainRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting request: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private string GenerateRequestId()
        {
            return $"REQ-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Enter Location" || textBox.Text == "Enter Description")
            {
                textBox.Text = string.Empty;
            }
        }

        private void ClearForm()
        {
            TxtLocation.Text = "Enter Location";
            TxtDescription.Text = "Enter Description";
            CmbCategory.SelectedIndex = -1;
            attachments.Clear();
            selectedLatitude = 0;
            selectedLongitude = 0;
            formattedAddress = string.Empty;
            UpdateAttachmentCount();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Name == "TxtLocation" ? "Enter Location" : "Enter Description";
            }
        }

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            BackToMainRequested?.Invoke(this, EventArgs.Empty);
        }


    }
}
