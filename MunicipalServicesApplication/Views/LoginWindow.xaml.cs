using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Data.SqlClient;
using MunicipalServicesApplication.Models;
using MunicipalServicesApplication.Services;

namespace MunicipalServicesApplication.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string idNumber = TxtIdNumber.Text.Trim();

            if (string.IsNullOrEmpty(idNumber))
            {
                ShowError("Please enter your South African ID number.");
                return;
            }

            if (IsValidSouthAfricanID(idNumber))
            {
                HideError();
                var userProfileService = new UserProfileService();
                App.CurrentUser = userProfileService.GetOrCreateUser(idNumber);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError("Invalid South African ID number.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsValidSouthAfricanID(string idNumber)
        {
            // Regex breakdown:
            // ^ - Start of the string
            // \d{2} - Two digits for the year (YY)
            // (0[1-9]|1[0-2]) - Month (MM): Ensures month is between 01 and 12
            // (0[1-9]|[12]\d|3[01]) - Day (DD): Ensures day is between 01 and 31
            // \d{4} - Four digits for gender (SSSS)
            // [01] - Citizenship (C): 0 for SA citizens, 1 for permanent residents
            // \d - A digit for the "A" section
            // \d$ - Last digit for checksum
            string pattern = @"^\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{4}[01]\d\d$";

            return Regex.IsMatch(idNumber, pattern);
        }

        private void ShowError(string message)
        {
            LblError.Text = message;
            LblError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            LblError.Text = string.Empty;
            LblError.Visibility = Visibility.Collapsed;
        }

        // Allow dragging the window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}