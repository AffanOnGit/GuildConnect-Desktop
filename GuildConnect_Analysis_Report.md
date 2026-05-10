# GuildConnect: Software Metrics Analysis Report

**Team Members:**
*   **S1:** Affan Hameed
*   **S2:** Shoaib Mehmood
*   **S3:** Bilal Mehmood

---

## Task 2: Cyclomatic Complexity & Test Case Generation (S1 & S2)

Cyclomatic Complexity $V(G)$ is calculated using the formula $V(G) = D + 1$, where $D$ is the number of decision points (`if`, `while`, `catch`, etc.).

| Function Name | Decision Points (D) | Cyclomatic Complexity | Test Case Inputs | Expected Output |
| :--- | :--- | :--- | :--- | :--- |
| `AuthService.Login(user, pass)` | 2 (`if` valid user, `if` valid pass) | 3 | Valid User / Valid Pass | Success (User Object) |
| | | | Invalid User / Any Pass | "Database error during login" |
| `AuthService.Register(...)` | 2 (`catch` block, validation) | 3 | Unique Email / Valid Data | True (Success) |
| | | | Duplicate Email | Exception Thrown |
| `SocietyService.RequestToJoin(...)` | 1 (`if` already requested) | 2 | New Student / Society | True (Request Sent) |
| | | | Existing Member | InvalidOperationException |
| `ReportService.ExportToCsv(...)` | 5 (loops and null checks) | 6 | DataGridView with rows | CSV File Generated |
| `AdminService.DeleteSociety(...)`| 0 (sequential deletes) | 1 | Valid societyId | True (Deleted) |

---

## Task 3: Structural Metric Module Justification (S1)

We use the **Information Flow (IF)** metric to evaluate the structural integrity of our modules.
Formula: $IF = (\text{Fan-in} \times \text{Fan-out})^2$

| Module | Fan-in (Calls to it) | Fan-out (Calls it makes) | IF Value |
| :--- | :--- | :--- | :--- |
| `AdminService` | 1 (AdminDashboard) | 2 (DB Context, DataTable) | **4** |
| `AuthService` | 2 (Login, Register Forms) | 2 (DB Context, User Model) | **16** |
| `SocietyService` | 3 (All 3 Dashboards) | 2 (DB Context, DataTable) | **36** |
| `ReportService` | 2 (Admin, Head Dashboards)| 4 (DataGridView, File, UI) | **64** |

**Justification:**
While `AdminService` has the lowest mathematical $IF$ value (4), **`SocietyService`** is justified as the most structurally sound module. It possesses a high Fan-in (3), meaning it is highly reused across all three dashboards, yet maintains a remarkably low Fan-out (2) despite handling 14 distinct database operations. This indicates excellent encapsulation and adherence to the Single Responsibility Principle.

---

## Task 4: CK Metrics Suite Analysis (S2)

*Metrics: WMC (Weighted Methods per Class), DIT (Depth of Inheritance), NOC (Number of Children), CBO (Coupling Between Objects), RFC (Response for Class), LCOM (Lack of Cohesion).*

| Class Name | WMC | DIT | NOC | CBO | RFC | LCOM |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `DatabaseContext` | 2 | 1 | 0 | 1 | 4 | Low |
| `AuthService` | 6 | 1 | 0 | 3 | 10 | Low |
| `SocietyService` | 28 | 1 | 0 | 4 | 24 | Low |
| `SocietyHeadDashboard`| 28 | 7 | 0 | 10 | 35 | High |
| `AdminDashboard` | 25 | 7 | 0 | 9 | 32 | High |
| `StudentDashboard` | 18 | 7 | 0 | 8 | 25 | High |

**Analysis Answers:**
*   **Most Coupled:** `SocietyHeadDashboard` (CBO = 10). It interacts with the UI, Dialogs, `SocietyService`, User Models, and `AppTheme`.
*   **Least Cohesive:** `AdminDashboard` (High LCOM). UI event handlers (e.g., `BtnCreateSociety_Click` vs `BtnToggleUserStatus_Click`) do not share instance variables, making the class structurally disjointed.
*   **Most Complex:** `SocietyHeadDashboard` / `SocietyService` (WMC = 28). They contain the highest sum of cyclomatic complexities due to heavy UI validation and SQL logic, respectively.
*   **Maximum Depth of Inheritance:** `7`. All custom forms inherit deep from the .NET Framework: `Object` → `MarshalByRefObject` → `Component` → `Control` → `ScrollableControl` → `ContainerControl` → `Form`.
*   **Class with Greatest NOC:** `Form` (Framework class). In our code, custom NOC is 0.

---

## Task 5: Fault Injection & Reliability (S3)

We injected 5 logic faults (e.g., changing `>=` to `<`, invalidating connection strings) into core modules to test resilience based on Task 2 test cases.
Formula: $Pr(\text{no more than } E \text{ errors}) = \frac{C}{I+1}$. Threshold $E = 1$, Injected $I = 5$.

| Module | Injected (I) | Caught (C) | Probability (Pr) | Status (Pr = 1 if C > E) |
| :--- | :--- | :--- | :--- | :--- |
| `AuthService` | 5 | 5 | 5 / 6 = 0.833 | 1 (Highly Reliable) |
| `SocietyService` | 5 | 5 | 5 / 6 = 0.833 | 1 (Highly Reliable) |
| `AdminDashboard` | 5 | 3 | 3 / 6 = 0.500 | 1 (Acceptable) |

*   **Most Reliable Feature:** `AuthService` & `SocietyService` (Highest catch rate via strict exception handling).
*   **Least Reliable Feature:** `AdminDashboard` (Some UI logic errors were not caught by unit tests alone).

---

## Task 6: KLM Usability Evaluation (S2)

Estimating the time an expert takes to complete the **"Login as Society Head"** task.
*Operators: K (280ms), M (1350ms), P (1100ms), H (400ms)*

1.  **M** (Think of credentials) = 1350 ms
2.  **H** (Move hand to keyboard) = 400 ms
3.  **K** (Type 'head@guildconnect.edu' - 24 chars) = $24 \times 280$ = 6720 ms
4.  **P** (Point mouse to Password field) = 1100 ms
5.  **K** (Type 'head123' - 7 chars) = $7 \times 280$ = 1960 ms
6.  **P** (Point to Login button) = 1100 ms
7.  **K** (Click) = 280 ms

**Total Task Time:** 1350 + 400 + 6720 + 1100 + 1960 + 1100 + 280 = **12,910 ms (12.91 seconds)**.

---

## Task 7: COCOMO Model Application (S1)

We apply the **Intermediate COCOMO I (Organic Mode)** model. The Organic mode is chosen because this is a relatively small, familiar, in-house academic project developed by a small team (3 students).

*   **Estimated KLOC:** ~2.5 (2,500 lines of code)
*   **Effort Formula:** $Effort = a \times (KLOC)^b \times EAF$
*   **Constants (Organic):** $a = 3.2$, $b = 1.05$
*   **EAF (Effort Adjustment Factor):** Assumed 1.0 (Nominal)

**Calculation:**
$Effort = 3.2 \times (2.5)^{1.05} \times 1.0$
$Effort = 3.2 \times 2.607 = \mathbf{8.34 \text{ Person-Months}}$

---

## Task 8: Documentation Ratio (S3)

Evaluating code maintainability.
Formula: $\text{Documentation Ratio} = \frac{\text{Total LOC}}{\text{Total Commented Lines}}$

*   **Total Lines of Code (approx):** 2,500
*   **Total Commented Lines (XML Summaries & Inline):** 185
*   **Ratio Calculation:** $2500 / 185 = \mathbf{13.51}$

**Conclusion:** A ratio of 13.51 indicates that approximately 1 in every 13.5 lines is a comment. This is a healthy, low ratio indicating well-documented code that will be easy to refactor in the future.
