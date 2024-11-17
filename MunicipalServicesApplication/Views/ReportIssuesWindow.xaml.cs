using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using MunicipalServices.Models;
using MunicipalServices.Core.DataStructures;

namespace MunicipalServicesApplication.Views
{
    public partial class ReportIssuesWindow : UserControl
    {
        private List<Attachment> attachments = new List<Attachment>();
        public event EventHandler BackToMainRequested;
        private readonly ServiceRequestBST requestsBST;

        public ReportIssuesWindow()
        {
            InitializeComponent();
            requestsBST = new ServiceRequestBST();
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
                        string base64String = Convert.ToBase64String(fileBytes);
                        attachments.Add(new Attachment { Name = Path.GetFileName(filename), Content = base64String });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error attaching file {filename}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrWhiteSpace(TxtLocation.Text) ||
                CmbCategory.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtDescription.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new ServiceRequest
            {
                RequestId = GenerateRequestId(),
                Location = TxtLocation.Text,
                Category = (CmbCategory.SelectedItem as ComboBoxItem).Content.ToString(),
                Description = TxtDescription.Text,
                Attachments = new List<Attachment>(attachments)
            };

            request.CalculatePriority();
            requestsBST.Insert(request);

            MessageBox.Show($"Service request submitted successfully!\nRequest ID: {request.RequestId}",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            ClearForm();
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
