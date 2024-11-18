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
   - **Address Autocomplete**: Utilizes the Places API for autocomplete suggestions when searching for addresses in the report issue window.

2. **Modern User Interface**
   - The app has a modern, user-friendly interface with smooth animations.
   - Features such as a custom ComboBox template, fade-in animations, and consistent colour schemes make the app intuitive and engaging for users.

3. **Dynamic Engagement**
   - A progress bar provides feedback to users as they complete sections of the form, encouraging engagement and making the reporting process smooth and transparent.

4. **Responsive Design**
   - The application is designed to be responsive with a minimum window size to ensure it works well across different screen resolutions.

5. **Advanced Data Structures Implementation**
   - **Red-Black Tree**: Ensures O(log n) operations for service request management
   - **Priority Heap**: Efficiently handles emergency and priority-based requests
   - **Service Request Graph**: Manages relationships between related service requests
   - **Service Status Tree**: Provides hierarchical view of service statuses
   - **Emergency Notice Tree**: Manages critical municipal announcements and alerts

## Technologies Used

- **Language**: C# (WPF using .NET Framework 4.7.2)
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Data Structures**: 
  - Red-Black Tree for balanced request management
  - Binary Heap for priority queue implementation
  - Graph structure for related issues
  - Service Status Tree for status management
  - Emergency Notice Tree for managing alerts
- **API**: Google Places API for address autocomplete
- **IDE**: Visual Studio

## Requirements

To run the **Municipal Services Application**, the following software is required:
- **.NET Framework 4.7.2 or higher**
- **Windows OS**
- **Visual Studio** (for development and running the application)

## Implementation Details

### 1. Data Structures

#### Red-Black Tree
- Ensures O(log n) time complexity for insertions and searches
- Self-balancing for consistent performance
- Used for efficient service request storage and retrieval

Example usage:
```csharp
var requestTree = new RedBlackTree();
requestTree.Insert(new ServiceRequest { RequestId = "REQ-123" });
var request = requestTree.Find("REQ-123");
```

#### Priority Heap
- Manages priority-based service requests
- O(log n) insertion and extraction of highest-priority requests
- Efficient handling of emergency situations

Example usage:
```csharp
var requestHeap = new ServiceRequestHeap();
requestHeap.Insert(new ServiceRequest { 
    RequestId = "EMERGENCY-123",
    Priority = 10
});
var urgentRequest = requestHeap.ExtractMax();
```

#### Service Request Graph
- Tracks relationships between service requests
- Enables pattern analysis and related issue identification
- Supports geographical clustering of issues

#### Service Status Tree
- Manages overall status of municipal services
- Provides quick status lookups and updates
- Hierarchical organization of service statuses

#### Emergency Notice Tree
- Singleton pattern implementation for emergency notifications
- Manages critical municipal announcements and alerts
- Provides quick access to emergency notices by severity

Example usage:
```csharp
var emergencyTree = EmergencyNoticeTree.Instance;
emergencyTree.Insert(new EmergencyNotice { 
    Title = "Water Interruption",
    Severity = "Warning"
});
var notices = emergencyTree.GetAll();
```

### 2. Performance Considerations

#### Time Complexity
- Red-Black Tree Operations: O(log n)
- Priority Heap Operations: O(log n)
- Graph Operations: O(1) for adjacent lookups

#### Memory Optimization
- Lazy loading of related data
- Efficient node structure design
- Minimal memory overhead in balanced trees

#### Scalability Features
- Balanced tree structures for consistent performance
- Priority-based processing for critical issues
- Graph-based relationship management

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

## How to Use the Application

### Main Menu

- When you open the app, you will see the **Main Menu** with the following options:
  - **Report an Issue**: Allows you to report a new issue to the municipality.
  - **Local Events and Announcements**: This feature is currently disabled and will be implemented in a future update.
  - **Service Request Status**: This feature is currently disabled and will be implemented in a future update.
  - **Exit**: Exits the application.

### Reporting an Issue

1. Click on **Report an Issue** to open the reporting window.
2. Enter the **location** of the issue (e.g., street name, neighborhood). The application will provide autocomplete suggestions using the Google Places API.
3. Select the **issue category** (e.g., Sanitation, Roads, Utilities) from the dropdown.
4. Write a **detailed description** of the issue.
5. If applicable, click **Attach Media** to upload any relevant images or documents that help illustrate the issue.
6. As you fill out the form, a **progress bar** will update, indicating your progress.
7. Once all fields are complete, click **Submit** to report the issue. A confirmation message will appear, and the window will close, taking you back to the main menu.

### Advanced Features

1. **Request Processing**
   - Priority-based handling using heap structure
   - Efficient request lookup using Red-Black Tree
   - Related issue detection using graph structure

2. **Status Management**
   - Real-time status updates
   - Hierarchical status organization
   - Efficient status lookup and modification

3. **Performance Features**
   - O(log n) operations for core functionalities
   - Memory-efficient data structures
   - Scalable architecture for growing municipalities

## File Structure

The project follows a structured folder layout:

```
MunicipalServicesApplication/
├── MunicipalServices.Core/
│   ├── DataStructures/
│   │   ├── DisjointSet.cs
│   │   ├── EmergencyNoticeNode.cs
│   │   ├── EmergencyNoticeTree.cs
│   │   ├── RedBlackTree.cs
│   │   ├── ServiceRequestBST.cs
│   │   ├── ServiceRequestGraph.cs
│   │   ├── ServiceRequestHeap.cs
│   │   ├── ServiceRequestNode.cs
│   │   ├── ServiceRequestTree.cs
│   │   └── ServiceStatusTree.cs
│   └── Services/
│       ├── AnnouncementService.cs
│       ├── DatabaseService.cs
│       ├── EmergencyManager.cs
│       ├── EmergencyResponseCoordinator.cs
│       ├── EventService.cs
│       └── GooglePlacesService.cs
├── MunicipalServices.Models/
│   ├── Announcement.cs
│   ├── Attachment.cs
│   ├── CommunityStat.cs
│   ├── Coordinates.cs
│   ├── CurrentUser.cs
│   ├── EmergencyNotice.cs
│   └── GooglePlacesModel.cs
├── Views/
│   ├── DashboardView.xaml
│   ├── LocalEventsWindow.xaml
│   ├── LoginWindow.xaml
│   ├── MainWindow.xaml
│   ├── ReportIssuesWindow.xaml
│   └── ServiceRequestStatusView.xaml
```

## Technical Documentation

For detailed technical documentation about the implementation of data structures and algorithms, please refer to the following files:

- `MunicipalServices.Core/DataStructures/RedBlackTree.cs`
- `MunicipalServices.Core/DataStructures/ServiceRequestHeap.cs`
- `MunicipalServices.Core/DataStructures/ServiceRequestGraph.cs`
- `MunicipalServices.Core/DataStructures/ServiceStatusTree.cs`
- `MunicipalServices.Core/DataStructures/EmergencyNoticeTree.cs`

## Future Updates

- **Service Request Tracking**: A feature to track the status of reported issues and requests.
- **Local Events and Announcements**: A section that will provide information about local municipal events and announcements.

## Contact

If you encounter any issues or have suggestions for improvements, feel free to reach out at st10241466@vcconnect.edu.za
