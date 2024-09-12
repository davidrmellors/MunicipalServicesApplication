# Municipal Services Application

## About

The **Municipal Services Application** is a C# WPF (.NET Framework) desktop application designed to streamline citizen engagement and service delivery for municipalities in South Africa. This application provides an efficient and user-friendly platform where citizens can:
- **Report issues** such as sanitation, roads, and utility problems.
- **Attach media** (e.g., images, documents) to help clarify the reported issues.
- **Track the status** of service requests (this feature will be added in future updates).
- **View local events and announcements** (this feature will also be added in future updates).

The application is designed with a modern user interface, which includes animations, a clean colour scheme, and a data grid to display all reported issues.

## Features

1. **Report an Issue**
   - Citizens can report issues in various categories (e.g., Sanitation, Roads, Utilities).
   - They can specify the location, describe the issue in detail, and attach media files (images, etc.).
   - A progress bar dynamically updates as users complete each section of the report.
   - All submitted issues are displayed in a data grid in the main window.

2. **Modern User Interface**
   - The app has a modern, user-friendly interface with smooth animations.
   - Features such as a custom ComboBox template, fade-in animations, and consistent colour schemes make the app intuitive and engaging for users.

3. **Dynamic Engagement**
   - A progress bar provides feedback to users as they complete sections of the form, encouraging engagement and making the reporting process smooth and transparent.

4. **Responsive Design**
   - The application is designed to be responsive with a minimum window size to ensure it works well across different screen resolutions.

## Technologies Used

- **Language**: C# (WPF using .NET Framework 4.7.2)
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Data Structures**: ObservableCollection, List for handling issues and media attachments
- **IDE**: Visual Studio

---

## Requirements

To run the **Municipal Services Application**, the following software is required:
- **.NET Framework 4.7.2 or higher**
- **Windows OS**
- **Visual Studio** (for development and running the application)

---

## Getting Started

### 1. Clone the Repository

First, clone the repository from GitHub:

```bash
git clone https://github.com/davidrmellors/MunicipalServicesApplication.git
```

### 2. Open the Solution

1. Open **Visual Studio**.
2. From the top menu, select **File > Open > Project/Solution**.
3. Navigate to the folder where you cloned the repository and open the `.sln` file.

### 3. Build the Solution

1. In **Solution Explorer**, right-click on the project name (**MunicipalServicesApplication**).
2. Click **Build** to compile the project and ensure all dependencies are installed.

### 4. Run the Application

1. After the build completes successfully, press **F5** or click the **Start** button to run the application.
2. The main window will open, presenting options to report an issue or exit the application.

---

## How to Use the Application

### Main Menu

- When you open the app, you will see the **Main Menu** with the following options:
  - **Report an Issue**: Allows you to report a new issue to the municipality.
  - **Local Events and Announcements**: This feature is currently disabled and will be implemented in a future update.
  - **Service Request Status**: This feature is currently disabled and will be implemented in a future update.
  - **Exit**: Exits the application.

### Reporting an Issue

1. Click on **Report an Issue** to open the reporting window.
2. Enter the **location** of the issue (e.g., street name, neighborhood).
3. Select the **issue category** (e.g., Sanitation, Roads, Utilities) from the dropdown.
4. Write a **detailed description** of the issue.
5. If applicable, click **Attach Media** to upload any relevant images or documents that help illustrate the issue.
6. As you fill out the form, a **progress bar** will update, indicating your progress.
7. Once all fields are complete, click **Submit** to report the issue. A confirmation message will appear, and the window will close, taking you back to the main menu.

### Viewing Reported Issues

- After submitting an issue, it will appear in the **Reported Issues** section on the main menu.
- The **data grid** will display the location, category, description, and any attached files for each reported issue.

---

## File Structure

The project follows a structured folder layout to keep things organised:

```
MunicipalServicesApplication/
├── Models/
│   ├── Issue.cs               # Data model for reported issues
├── Views/
│   ├── MainWindow.xaml         # Main menu window
│   ├── ReportIssuesWindow.xaml # Window for reporting issues
│   ├── MainWindow.xaml.cs      # Logic for main window
│   ├── ReportIssuesWindow.xaml.cs # Logic for reporting issues window
├── Resources/
│   ├── zaFlag.ico              # Application icon
├── App.xaml                    # Application-level settings
├── App.xaml.cs                 # Entry point for the application
├── MunicipalServicesApplication.sln # Visual Studio solution file
├── README.md                   # Documentation for the project
```

### Models
The `Models` folder contains the **Issue** class, which defines the structure of a reported issue (location, category, description, attachments).

### Views
The `Views` folder contains the XAML files and their code-behind logic for the main window and the issue-reporting window.

---

## Customisation

Feel free to customise the following aspects of the application:
- **ComboBox Style**: Update the ComboBox template in the `ReportIssuesWindow.xaml` to match your preferred design.
- **Data Grid**: Adjust the appearance of the data grid in the main window for displaying reported issues.

---

## Future Updates

- **Service Request Tracking**: A feature to track the status of reported issues and requests.
- **Local Events and Announcements**: A section that will provide information about local municipal events and announcements.

---

## Contact

If you encounter any issues or have suggestions for improvements, feel free to reach out at st10241466@vcconnect.edu.za
