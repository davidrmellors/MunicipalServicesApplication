using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MunicipalServicesApplication.Models;
using System.Windows.Media.Animation;
using System;

namespace MunicipalServicesApplication.Views
{
    //-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// ReportIssuesWindow class handles the user interface and logic for reporting issues.
    /// It allows users to input location, select categories, attach media, and submit reports.
    /// </summary>
    public partial class ReportIssuesWindow : Window
    {
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// A list to store all reported issues.
        /// </summary>
        private List<Issue> reportedIssues = new List<Issue>();

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// A list to store the paths of the files attached by the user.
        /// </summary>
        private List<string> attachedFiles = new List<string>();

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for ReportIssuesWindow.
        /// Initializes the components and sets up the window.
        /// </summary>
        public ReportIssuesWindow()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the GotFocus event for text boxes.
        /// Clears the placeholder text when the user focuses on the text box.
        /// </summary>
        /// <param name="sender">The text box that triggered the event.</param>
        /// <param name="e">Event data for the GotFocus event.</param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Enter Location" || textBox.Text == "Enter Description")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.White;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the LostFocus event for text boxes.
        /// Restores the placeholder text if the user leaves the text box empty.
        /// Also updates the progress bar.
        /// </summary>
        /// <param name="sender">The text box that triggered the event.</param>
        /// <param name="e">Event data for the LostFocus event.</param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "txtLocation")
                {
                    textBox.Text = "Enter Location";
                }
                else if (textBox.Name == "txtDescription")
                {
                    textBox.Text = "Enter Description";
                }
                textBox.Foreground = Brushes.Gray;
            }
            UpdateProgress(); // Update the progress bar after leaving the field
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the SelectionChanged event for the ComboBox (category selection).
        /// Updates the progress bar when a category is selected.
        /// </summary>
        /// <param name="sender">The ComboBox that triggered the event.</param>
        /// <param name="e">Event data for the SelectionChanged event.</param>
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProgress(); // Update the progress bar when a category is selected
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the click event for the Attach Media button.
        /// Opens a file dialog to allow users to attach media and updates the progress bar.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Event data for the click event.</param>
        private void AttachMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;  // Allow multiple file attachments
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    attachedFiles.Add(file);
                }
                MessageBox.Show("Files Attached: " + string.Join(", ", openFileDialog.SafeFileNames));
            }
            UpdateProgress(); // Update progress after attaching media
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the click event for the Submit button.
        /// Validates the form, creates a new issue, adds it to the reported issues list, and closes the window.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Event data for the click event.</param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text) || string.IsNullOrWhiteSpace(txtDescription.Text) || cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Add the issue to the centralized collection
            Issue.AddIssue(
                txtLocation.Text,
                ((ComboBoxItem)cmbCategory.SelectedItem).Content.ToString(),
                txtDescription.Text,
                new List<string>(attachedFiles)  // Pass the attached files
            );

            MessageBox.Show("Issue reported successfully!");
            this.Close();  // Close the window after submission
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the click event for the Back to Main Menu button.
        /// Closes the current window and returns to the main menu.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Event data for the click event.</param>
        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the window to return to the main menu
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the progress bar based on the form completion (location, description, category, and media).
        /// </summary>
        private void UpdateProgress()
        {
            double oldValue = progressBar.Value;

            // Calculate the new progress value
            double newValue = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text) && txtLocation.Text != "Enter Location")
            {
                newValue += 25;
            }
            if (!string.IsNullOrWhiteSpace(txtDescription.Text) && txtDescription.Text != "Enter Description")
            {
                newValue += 25;
            }
            if (cmbCategory.SelectedItem != null)
            {
                newValue += 25;
            }
            if (attachedFiles.Count > 0)
            {
                newValue += 25;
            }

            // Animate the progress bar value
            AnimateProgressBar(oldValue, newValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Animates the progress bar to smoothly transition from oldValue to newValue.
        /// </summary>
        /// <param name="oldValue">The current value of the progress bar.</param>
        /// <param name="newValue">The new value to animate to.</param>
        private void AnimateProgressBar(double oldValue, double newValue)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = oldValue,   // Start from the old value
                To = newValue,     // Animate to the new value
                Duration = new Duration(TimeSpan.FromSeconds(0.5)) // Animation duration
            };
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);  // Apply the animation
        }
    }

    //-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// The Issue class represents a reported issue, including location, category, description, and attached media.
    /// This class is used to store and manage reported issues in the Municipal Services Application.
    /// </summary>
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
