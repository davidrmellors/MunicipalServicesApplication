using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MunicipalServicesApplication.Models;

namespace MunicipalServicesApplication.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IssuesItemsControl.ItemsSource = Issue.ReportedIssues;
        }

        private void BtnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            ReportIssuesWindow reportIssuesWindow = new ReportIssuesWindow();
            reportIssuesWindow.Width = this.Width;
            reportIssuesWindow.Height = this.Height;
            this.Hide(); // Hide the MainWindow
            reportIssuesWindow.Closed += (s, args) =>
            {
                this.Show(); // Show the MainWindow when ReportIssuesWindow is closed
                IssuesItemsControl.Items.Refresh();
            };
            reportIssuesWindow.Show();
        }

        private void ViewAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string base64String)
            {
                try
                {
                    byte[] fileBytes = Convert.FromBase64String(base64String);
                    string tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, fileBytes);

                    Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
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
