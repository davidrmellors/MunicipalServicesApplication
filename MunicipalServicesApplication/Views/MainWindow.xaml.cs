using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using MunicipalServicesApplication.Models;

namespace MunicipalServicesApplication.Views
{
    //-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// MainWindow class is the entry point for the application and inherits from the Window class.
    /// It handles navigation to the ReportIssuesWindow and manages interaction with the main user interface.
    /// </summary>
    public partial class MainWindow : Window
    {
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for MainWindow.
        /// Initializes the MainWindow components and sets the ItemsSource for the IssuesDataGrid to display reported issues.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            IssuesDataGrid.ItemsSource = Issue.ReportedIssues;  // Bind the DataGrid to the static collection of reported issues
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the click event for the Report Issues button.
        /// This method hides the MainWindow and opens the ReportIssuesWindow.
        /// The MainWindow is shown again when the ReportIssuesWindow is closed.
        /// </summary>
        /// <param name="sender">The source of the event (usually the Report Issues button).</param>
        /// <param name="e">Event data for the click event.</param>
        private void btnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            //-------------------------------------------------------------------------------------------------------------
            // Create and show the ReportIssuesWindow
            ReportIssuesWindow reportIssuesWindow = new ReportIssuesWindow();

            // Hide the MainWindow while the ReportIssuesWindow is active
            this.Hide();

            //-------------------------------------------------------------------------------------------------------------
            // Show ReportIssuesWindow
            reportIssuesWindow.Show();

            //-------------------------------------------------------------------------------------------------------------
            // Show the MainWindow again when ReportIssuesWindow is closed
            reportIssuesWindow.Closed += (s, args) =>
            {
                this.Show();  // Show MainWindow again after ReportIssuesWindow is closed
            };

            //-------------------------------------------------------------------------------------------------------------
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the RequestNavigate event for hyperlinks within the application.
        /// This method opens the file using the default application when the hyperlink is clicked.
        /// </summary>
        /// <param name="sender">The source of the event (usually the Hyperlink control).</param>
        /// <param name="e">Event data for the RequestNavigate event.</param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Open the file using the default application (e.g., open PDF in default PDF viewer)
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            // Mark the event as handled to prevent further processing
            e.Handled = true;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the click event for the Exit button.
        /// Closes the entire application by shutting down the current instance of the application.
        /// </summary>
        /// <param name="sender">The source of the event (usually the Exit button).</param>
        /// <param name="e">Event data for the click event.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Shuts down the entire application
        }

        //-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
