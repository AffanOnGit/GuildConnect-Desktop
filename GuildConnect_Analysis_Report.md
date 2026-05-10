# GuildConnect: Software Metrics Analysis Report

This report fulfills **Task 2, Task 3, and Task 4** of the Software Measurement and Metrics Project Statement.

---

## Task 2: Cyclomatic Complexity and Test Cases

Cyclomatic Complexity (CC) measures the number of linearly independent paths through a program's source code. It is calculated based on control flow statements (`if`, `while`, `catch`, etc.). Below is the evaluation for key functions across the system.

| Class / Module | Function Name | Cyclomatic Complexity (CC) | Test Case Inputs / Scenarios |
| :--- | :--- | :--- | :--- |
| `AuthService` | `Login(email, password)` | 3 | 1. Valid email & correct password<br>2. Valid email & wrong password<br>3. Non-existent email / DB offline |
| `AuthService` | `Register(user)` | 3 | 1. Unique email & valid data<br>2. Duplicate email (throws exception)<br>3. Null user object |
| `SocietyService` | `RequestToJoin(studentId, societyId)` | 2 | 1. Student not in society (Success)<br>2. Student already requested (Exception) |
| `SocietyService` | `GetUpcomingEvents()` | 1 | 1. Call method when events exist<br>2. Call method when no events exist |
| `AdminService` | `DeleteSociety(societyId)` | 1 | 1. Valid societyId with cascade dependencies<br>2. Invalid societyId |
| `ReportService` | `ExportToCsv(dgv, fileName)` | 6 | 1. Valid DataGridView with data<br>2. Empty DataGridView<br>3. File permission error (catch) |
| `StudentDashboard` | `BtnRegisterEvent_Click(...)` | 4 | 1. Event selected<br>2. No event selected<br>3. Already registered (catch logic) |
| `SocietyHeadDashboard`| `BtnEditEvent_Click(...)` | 4 | 1. Valid edit selection<br>2. Empty inputs provided in dialog<br>3. Database timeout during save |
| `AdminDashboard` | `BtnCreateSociety_Click(...)`| 4 | 1. Valid name & head ID<br>2. Empty name<br>3. Invalid format for Head ID |
| `DatabaseContext` | `GetConnection()` | 1 | 1. Called during initialization |

---

## Task 3: Module Evaluation using Structural Metrics

We evaluate the different modules of the system using **Coupling Between Objects (CBO)**, which measures the number of distinct classes a module is coupled to. High coupling indicates fragility, while low coupling indicates strong, independent design.

### Structural Metric Comparative Table

| Feature / Module | CBO Value | Coupled Classes / Dependencies |
| :--- | :--- | :--- |
| **Authentication Module** (`AuthService`) | 3 | `User` model, `DatabaseContext`, `SqlCommand` |
| **Society Management** (`SocietyService`) | 4 | `DatabaseContext`, `DataTable`, `SqlCommand`, `SqlDataAdapter` |
| **Admin Management** (`AdminService`) | 4 | `DatabaseContext`, `DataTable`, `SqlCommand`, `SqlDataAdapter` |
| **UI Rendering Engine** (`AppTheme`) | 6 | `Form`, `Panel`, `Button`, `TextBox`, `Color`, `Font` |
| **Reporting Engine** (`ReportService`) | 5 | `DataGridView`, `SaveFileDialog`, `StringBuilder`, `File`, `Exception` |
| **Dashboards** (`Student/Head/Admin`) | ~10 | UI Components, `User`, all Services, `AppTheme` |

### Justification of the Best Module
**Best Module:** `SocietyService` (Society Management Module).
**Defense:** `SocietyService` is the largest business-logic component in the system (handling 14 distinct operations, including event creation, memberships, and tasks). Despite its massive feature set, its **CBO is extremely low (4)**. It interacts strictly with the `DatabaseContext` and ADO.NET primitives, completely decoupling itself from UI elements or specific models. This low coupling combined with high functionality makes it highly robust, testable, and the best-structured module in the application.

---

## Task 4: Chidamber & Kemerer (CK) Metrics Analysis

The CK Metrics suite has been applied to the complete generated project.

### CK Metrics Data Table

*Legend: WMC = Weighted Methods per Class, DIT = Depth of Inheritance Tree, NOC = Number of Children, CBO = Coupling Between Objects, RFC = Response for Class, LCOM = Lack of Cohesion in Methods.*

| Class Name | WMC | DIT | NOC | CBO | RFC | LCOM |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `DatabaseContext` | 2 | 1 | 0 | 1 | 4 | Low |
| `User` (Model) | 0 | 1 | 0 | 0 | 0 | N/A |
| `AuthService` | 6 | 1 | 0 | 3 | 10 | Low |
| `SocietyService` | 28 | 1 | 0 | 4 | 24 | Low |
| `AdminService` | 14 | 1 | 0 | 4 | 15 | Low |
| `ReportService` | 15 | 1 | 0 | 5 | 18 | N/A (Static)|
| `AppTheme` | 12 | 1 | 0 | 6 | 20 | N/A (Static)|
| `LoginForm` | 12 | 7 | 0 | 6 | 20 | High |
| `RegistrationForm` | 10 | 7 | 0 | 6 | 18 | High |
| `StudentDashboard` | 18 | 7 | 0 | 8 | 25 | High |
| `AdminDashboard` | 25 | 7 | 0 | 9 | 32 | High |
| `SocietyHeadDashboard`| 28 | 7 | 0 | 10 | 35 | High |

### Analysis Questions Answered

*   **Maximum Depth of Inheritance:** 
    *   **Value:** `7`
    *   **Explanation:** All Dashboard and Form classes inherit from the .NET Windows Forms framework: `Object` → `MarshalByRefObject` → `Component` → `Control` → `ScrollableControl` → `ContainerControl` → `Form` → `CustomForm`.
*   **Highest and Lowest WMC and its explanation:** 
    *   **Highest WMC:** `SocietyHeadDashboard` (28) and `SocietyService` (28). *Explanation*: The Society Head dashboard has the most UI elements and click events (Create, Edit, Cancel, Approve, Reject, Assign). The service has 14 distinct database methods.
    *   **Lowest WMC:** `User` model (0). *Explanation*: The model consists entirely of auto-properties with no logical methods, resulting in zero complexity.
*   **Class with the greatest number of children (NOC):**
    *   Within our custom codebase, **NOC is 0** because no custom class acts as a base class for others. However, if looking at framework classes used, the base `Form` class has the greatest number of children in this project (5 derived forms).
*   **Most complex class:**
    *   **`SocietyHeadDashboard`**. It has the highest RFC (Response for Class = 35) and highest CBO (10), meaning it orchestrates the most interactions between different parts of the system (UI, User models, SocietyServices, and AppTheme).
*   **Most coupled class:**
    *   **`SocietyHeadDashboard`** (CBO = 10). It requires knowledge of UI controls, DataGridViews, Dialog Forms, `AppTheme`, `SocietyService`, the `User` model, and System event args.
*   **Least cohesive class:**
    *   **`AdminDashboard` & `SocietyHeadDashboard`**. High LCOM is common in UI classes. Methods like `BtnApproveSociety_Click` and `BtnToggleUserStatus_Click` do not share any instance variables (other than the Service instances) and perform entirely disparate functions.
