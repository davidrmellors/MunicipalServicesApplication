//-------------------------------------------------------------------------------------------------------------
/// <summary>
/// Main window for user login. Validates South African ID numbers and manages user authentication.
/// </summary>
using System.Windows;
using System.Text.RegularExpressions;
using MunicipalServices.Core.Services;

namespace MunicipalServicesApplication.Views
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the LoginWindow class
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the login button click event. Validates ID number and creates/retrieves user profile.
        /// </summary>
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
                App.CurrentUser = new UserProfileService().GetOrCreateUser(idNumber);
                
                // Show main window immediately
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                ShowError("Please enter a valid South African ID number.");
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the close button click event
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates if the provided string matches South African ID number format
        /// </summary>
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

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays an error message to the user
        /// </summary>
        private void ShowError(string message)
        {
            LblError.Text = message;
            LblError.Visibility = Visibility.Visible;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Hides the error message display
        /// </summary>
        private void HideError()
        {
            LblError.Text = string.Empty;
            LblError.Visibility = Visibility.Collapsed;
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//