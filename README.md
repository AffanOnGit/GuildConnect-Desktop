# ⚔ GuildConnect — Society Management System

GuildConnect is a professional-grade .NET desktop application designed to centralize and automate the management of university student societies. Built with a focus on modern aesthetics and robust architecture, it provides a unified platform for students, society leaders, and university administration.

---

## 🚀 Key Features

### 💎 Premium Modern UI
- **Glassmorphism Design**: A sleek, dark-themed interface with vibrant accent colors.
- **Dynamic Dashboards**: Role-specific sidebars and interactive data grids.
- **Custom Styling Engine**: Centralized `AppTheme` system for consistent visual excellence.

### 👥 Role-Based Workflows
- **Student Module**:
  - Secure Registration & Authentication.
  - Browse available societies and view upcoming events.
  - One-click join requests and event registration.
  - Track assigned tasks and view membership status.
- **Society Head Module**:
  - Manage society profiles and member lists.
  - Approve/Reject membership requests.
  - Create, edit, and cancel society events.
  - Assign and track tasks for members.
  - Generate comprehensive CSV reports for members and events.
- **Admin Module**:
  - University-wide oversight of all users, societies, and events.
  - Approve new societies and pending events.
  - Suspend or delete societies (with cascading cleanup).
  - Manage user account status (Activate/Deactivate).
  - Export university-wide metrics and activity reports.

---

## 🛠 Tech Stack
- **Core**: .NET (WinForms)
- **Language**: C#
- **Database**: SQL Server (ADO.NET)
- **Architecture**: Service-Oriented (Separation of UI, Business Logic, and Data Access)

---

## ⚙️ Setup & Installation

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Execute the script provided in `DatabaseSchema.sql` to create the `GuildConnectDB`.
3. (Optional) Run `PopulateSampleData.sql` to seed the database with demo users and societies.

### 2. Configuration
Update the connection string in `DataAccess/DatabaseContext.cs` to match your SQL Server instance. The default is set to `localhost\SQLEXPRESS01`.

```csharp
// DataAccess/DatabaseContext.cs
private readonly string _connectionString = @"Server=localhost\SQLEXPRESS01;Database=GuildConnectDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

### 3. Run the Application
Open the solution in Visual Studio and press **F5**, or use the CLI:
```powershell
dotnet run
```

---

## 🔑 Demo Credentials

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@guildconnect.edu` | `admin123` |
| **Society Head** | `head@guildconnect.edu` | `head123` |
| **Student** | `student@guildconnect.edu` | `student123` |

---

## 📊 Software Metrics Analysis
This project was developed for the **Software Measurement and Metrics** course. Key metrics evaluated include:
- **Max DIT (Depth of Inheritance Tree)**: 7 (WinForms hierarchy).
- **CBO (Coupling Between Objects)**: Optimized service layer (Avg CBO < 5).
- **WMC (Weighted Methods per Class)**: High functional density in dashboards.

*Full analysis available in `GuildConnect_Analysis_Report.md`.*

---

## 📂 Project Structure
- **/DataAccess**: Centralized SQL connection management.
- **/Services**: Business logic for Auth, Societies, and Admin operations.
- **/Forms**: UI Layouts and event handling.
- **/UI**: Design tokens, color palettes, and styling utilities.
- **/Models**: Core data entities.

---

## 📄 License
This project is for academic purposes as part of the Software Engineering curriculum.
