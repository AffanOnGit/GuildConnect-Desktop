using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GuildConnect_Desktop.Models;
using GuildConnect_Desktop.Services;
using GuildConnect_Desktop.UI;

namespace GuildConnect_Desktop.Forms
{
    /// <summary>
    /// Dashboard for Society Head users.
    /// Manages profile, members, events, and task assignments.
    /// Supports event update/cancel and report generation.
    /// </summary>
    public partial class SocietyHeadDashboard : Form
    {
        private User _currentUser;
        private readonly SocietyService _societyService;
        private int _currentSocietyId = -1;

        // Sidebar
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Content controls
        private DataGridView dgvMembers;
        private DataGridView dgvEvents;
        private DataGridView dgvTasks;

        private Button btnApproveMember;
        private Button btnRejectMember;
        private Button btnCreateEvent;
        private Button btnEditEvent;
        private Button btnCancelEvent;
        private Button btnAssignTask;

        public SocietyHeadDashboard(User user)
        {
            _currentUser = user;
            _societyService = new SocietyService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "GuildConnect — Society Head Dashboard";
            this.Size = new Size(1050, 750);
            AppTheme.StyleDashboardForm(this);

            // ═══════════════════════════════════════════════════
            //  SIDEBAR
            // ═══════════════════════════════════════════════════
            sidebarPanel = AppTheme.CreateSidebar(220, this.ClientSize.Height);

            Label lblBrand = new Label
            {
                Text = "⚔ GuildConnect",
                Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold),
                ForeColor = AppTheme.AccentPrimary,
                Location = new Point(15, 20),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblBrand);

            Label lblRole = new Label
            {
                Text = "SOCIETY HEAD",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.WarningColor,
                Location = new Point(20, 55),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblRole);

            Panel divider = new Panel { BackColor = AppTheme.BorderSubtle, Size = new Size(190, 1), Location = new Point(15, 80) };
            sidebarPanel.Controls.Add(divider);

            Label lblUser = new Label
            {
                Text = _currentUser.FullName,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(20, 95),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblUser);

            Label lblEmail = new Label
            {
                Text = _currentUser.Email,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(20, 115),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblEmail);

            // Sidebar buttons
            Button btnRefresh = AppTheme.CreateSidebarButton("↻  Refresh Data", 160, (s, e) => LoadData());
            sidebarPanel.Controls.Add(btnRefresh);

            Button btnExportReport = AppTheme.CreateSidebarButton("📊  Export Report", 210, (s, e) => ExportReport());
            btnExportReport.ForeColor = AppTheme.AccentSecondary;
            sidebarPanel.Controls.Add(btnExportReport);

            Button btnLogout = AppTheme.CreateSidebarButton("⏻  Logout", 260, (s, e) => this.Close());
            btnLogout.ForeColor = AppTheme.DangerColor;
            sidebarPanel.Controls.Add(btnLogout);

            this.Controls.Add(sidebarPanel);

            // ═══════════════════════════════════════════════════
            //  MAIN CONTENT
            // ═══════════════════════════════════════════════════
            contentPanel = new Panel
            {
                Location = new Point(220, 0),
                Size = new Size(this.ClientSize.Width - 220, this.ClientSize.Height),
                BackColor = AppTheme.BackgroundDark,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                Padding = new Padding(25)
            };
            this.Controls.Add(contentPanel);

            int yOffset = 20;
            int contentWidth = contentPanel.Width - 60;

            // ── Members Section ────────────────────────────────
            Label lblMembers = new Label
            {
                Text = "👥  Membership Requests & Members",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblMembers);
            contentPanel.Controls.Add(lblMembers);
            yOffset += 30;

            dgvMembers = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvMembers);
            contentPanel.Controls.Add(dgvMembers);
            yOffset += 138;

            btnApproveMember = new Button { Text = "  ✓ APPROVE  ", Location = new Point(25, yOffset), Size = new Size(140, 36) };
            AppTheme.StyleSuccessButton(btnApproveMember);
            btnApproveMember.Click += BtnApproveMember_Click;
            contentPanel.Controls.Add(btnApproveMember);

            btnRejectMember = new Button { Text = "  ✗ REJECT  ", Location = new Point(175, yOffset), Size = new Size(130, 36) };
            AppTheme.StyleDangerButton(btnRejectMember);
            btnRejectMember.Click += BtnRejectMember_Click;
            contentPanel.Controls.Add(btnRejectMember);
            yOffset += 50;

            // ── Events Section ─────────────────────────────────
            Label lblEvents = new Label
            {
                Text = "📅  Society Events",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblEvents);
            contentPanel.Controls.Add(lblEvents);
            yOffset += 30;

            dgvEvents = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvEvents);
            contentPanel.Controls.Add(dgvEvents);
            yOffset += 128;

            btnCreateEvent = new Button { Text = "  + CREATE  ", Location = new Point(25, yOffset), Size = new Size(130, 36) };
            AppTheme.StyleButton(btnCreateEvent, true);
            btnCreateEvent.Click += BtnCreateEvent_Click;
            contentPanel.Controls.Add(btnCreateEvent);

            btnEditEvent = new Button { Text = "  ✎ EDIT  ", Location = new Point(165, yOffset), Size = new Size(120, 36) };
            AppTheme.StyleButton(btnEditEvent, false);
            btnEditEvent.BackColor = AppTheme.WarningColor;
            btnEditEvent.ForeColor = AppTheme.BackgroundDark;
            btnEditEvent.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 70);
            btnEditEvent.Click += BtnEditEvent_Click;
            contentPanel.Controls.Add(btnEditEvent);

            btnCancelEvent = new Button { Text = "  ✗ CANCEL  ", Location = new Point(295, yOffset), Size = new Size(130, 36) };
            AppTheme.StyleDangerButton(btnCancelEvent);
            btnCancelEvent.Click += BtnCancelEvent_Click;
            contentPanel.Controls.Add(btnCancelEvent);
            yOffset += 50;

            // ── Tasks Section ──────────────────────────────────
            Label lblTasks = new Label
            {
                Text = "✅  Task Assignments",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblTasks);
            contentPanel.Controls.Add(lblTasks);
            yOffset += 30;

            dgvTasks = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvTasks);
            contentPanel.Controls.Add(dgvTasks);
            yOffset += 128;

            btnAssignTask = new Button { Text = "  + ASSIGN TASK  ", Location = new Point(25, yOffset), Size = new Size(170, 36) };
            AppTheme.StyleButton(btnAssignTask, true);
            btnAssignTask.BackColor = AppTheme.AccentSecondary;
            btnAssignTask.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 200, 240);
            btnAssignTask.Click += BtnAssignTask_Click;
            contentPanel.Controls.Add(btnAssignTask);
        }

        private void LoadData()
        {
            try
            {
                // Find this head's society
                var societyTable = _societyService.GetSocietyByHead(_currentUser.UserID);
                if (societyTable.Rows.Count > 0)
                {
                    _currentSocietyId = Convert.ToInt32(societyTable.Rows[0]["SocietyID"]);

                    dgvMembers.DataSource = _societyService.GetMembersBySociety(_currentSocietyId);
                    dgvEvents.DataSource = _societyService.GetEventsBySociety(_currentSocietyId);
                    dgvTasks.DataSource = _societyService.GetTasksBySociety(_currentSocietyId);

                    HideColumn(dgvMembers, "MembershipID");
                    HideColumn(dgvEvents, "EventID");
                    HideColumn(dgvTasks, "TaskID");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HideColumn(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].Visible = false;
        }

        // ═══════════════════════════════════════════════════════
        //  REPORT GENERATION
        // ═══════════════════════════════════════════════════════

        private void ExportReport()
        {
            ReportService.ExportCombinedReport(
                "SocietyReport.csv",
                ("Members", dgvMembers),
                ("Events", dgvEvents),
                ("Task Assignments", dgvTasks)
            );
        }

        // ═══════════════════════════════════════════════════════
        //  MEMBER ACTIONS
        // ═══════════════════════════════════════════════════════

        private void BtnApproveMember_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMembers.SelectedRows.Count == 0) { MessageBox.Show("Select a member first."); return; }
                int membershipId = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MembershipID"].Value);
                _societyService.UpdateMembershipStatus(membershipId, "Approved");
                MessageBox.Show("Member approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnRejectMember_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMembers.SelectedRows.Count == 0) { MessageBox.Show("Select a member first."); return; }
                int membershipId = Convert.ToInt32(dgvMembers.SelectedRows[0].Cells["MembershipID"].Value);
                _societyService.UpdateMembershipStatus(membershipId, "Rejected");
                MessageBox.Show("Member rejected.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ═══════════════════════════════════════════════════════
        //  EVENT ACTIONS (Create, Edit, Cancel)
        // ═══════════════════════════════════════════════════════

        private void BtnCreateEvent_Click(object sender, EventArgs e)
        {
            if (_currentSocietyId < 0) { MessageBox.Show("No society found."); return; }
            ShowEventDialog("Create New Event", "", "", DateTime.Now.AddDays(7), (title, desc, date) =>
            {
                _societyService.CreateEvent(_currentSocietyId, title, date.Date, desc);
                MessageBox.Show("Event created (pending admin approval).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void BtnEditEvent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEvents.SelectedRows.Count == 0) { MessageBox.Show("Select an event first."); return; }
                var row = dgvEvents.SelectedRows[0];
                int eventId = Convert.ToInt32(row.Cells["EventID"].Value);
                string currentTitle = row.Cells["Title"].Value?.ToString() ?? "";
                string currentDesc = row.Cells["Description"].Value?.ToString() ?? "";
                DateTime currentDate = Convert.ToDateTime(row.Cells["EventDate"].Value);

                ShowEventDialog("Edit Event", currentTitle, currentDesc, currentDate, (title, desc, date) =>
                {
                    _societyService.UpdateEvent(eventId, title, date.Date, desc);
                    MessageBox.Show("Event updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnCancelEvent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEvents.SelectedRows.Count == 0) { MessageBox.Show("Select an event first."); return; }
                int eventId = Convert.ToInt32(dgvEvents.SelectedRows[0].Cells["EventID"].Value);
                string title = dgvEvents.SelectedRows[0].Cells["Title"].Value?.ToString() ?? "";

                var confirm = MessageBox.Show($"Are you sure you want to cancel \"{title}\"?", "Confirm Cancel",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    _societyService.CancelEvent(eventId);
                    MessageBox.Show("Event cancelled.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary>
        /// Reusable dialog for creating or editing an event.
        /// </summary>
        private void ShowEventDialog(string dialogTitle, string prefillTitle, string prefillDesc, DateTime prefillDate, Action<string, string, DateTime> onSave)
        {
            using (var dialog = new Form())
            {
                dialog.Text = dialogTitle;
                dialog.Size = new Size(420, 340);
                dialog.StartPosition = FormStartPosition.CenterParent;
                AppTheme.StyleForm(dialog);

                Panel card = new Panel { Location = new Point(20, 15), Size = new Size(370, 270), BackColor = AppTheme.PanelDark };
                dialog.Controls.Add(card);

                Label lblT = new Label { Text = "EVENT TITLE", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 15), AutoSize = true };
                card.Controls.Add(lblT);
                TextBox txtTitle = new TextBox { Location = new Point(15, 35), Size = new Size(340, 30), Text = prefillTitle };
                AppTheme.StyleTextBox(txtTitle);
                card.Controls.Add(txtTitle);

                Label lblD = new Label { Text = "DESCRIPTION", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 75), AutoSize = true };
                card.Controls.Add(lblD);
                TextBox txtDesc = new TextBox { Location = new Point(15, 95), Size = new Size(340, 30), Text = prefillDesc };
                AppTheme.StyleTextBox(txtDesc);
                card.Controls.Add(txtDesc);

                Label lblDt = new Label { Text = "EVENT DATE", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 135), AutoSize = true };
                card.Controls.Add(lblDt);
                DateTimePicker dtp = new DateTimePicker { Location = new Point(15, 155), Size = new Size(340, 30), Format = DateTimePickerFormat.Short, Value = prefillDate };
                dtp.CalendarMonthBackground = AppTheme.InputBackground;
                card.Controls.Add(dtp);

                Button btnSave = new Button { Text = dialogTitle.ToUpper().Contains("EDIT") ? "SAVE CHANGES" : "CREATE EVENT", Location = new Point(15, 210), Size = new Size(340, 40) };
                AppTheme.StyleButton(btnSave, true);
                btnSave.Click += (s2, e2) =>
                {
                    if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("Title is required."); return; }
                    try
                    {
                        onSave(txtTitle.Text.Trim(), txtDesc.Text.Trim(), dtp.Value);
                        dialog.Close();
                        LoadData();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                };
                card.Controls.Add(btnSave);
                dialog.ShowDialog(this);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  TASK ACTIONS
        // ═══════════════════════════════════════════════════════

        private void BtnAssignTask_Click(object sender, EventArgs e)
        {
            if (_currentSocietyId < 0) { MessageBox.Show("No society found."); return; }

            using (var dialog = new Form())
            {
                dialog.Text = "Assign New Task";
                dialog.Size = new Size(420, 340);
                dialog.StartPosition = FormStartPosition.CenterParent;
                AppTheme.StyleForm(dialog);

                Panel card = new Panel { Location = new Point(20, 15), Size = new Size(370, 270), BackColor = AppTheme.PanelDark };
                dialog.Controls.Add(card);

                Label lblSID = new Label { Text = "STUDENT ID (UserID)", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 15), AutoSize = true };
                card.Controls.Add(lblSID);
                TextBox txtStudentId = new TextBox { Location = new Point(15, 35), Size = new Size(340, 30) };
                AppTheme.StyleTextBox(txtStudentId);
                card.Controls.Add(txtStudentId);

                Label lblD = new Label { Text = "TASK DESCRIPTION", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 75), AutoSize = true };
                card.Controls.Add(lblD);
                TextBox txtDesc = new TextBox { Location = new Point(15, 95), Size = new Size(340, 30) };
                AppTheme.StyleTextBox(txtDesc);
                card.Controls.Add(txtDesc);

                Label lblDue = new Label { Text = "DUE DATE", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 135), AutoSize = true };
                card.Controls.Add(lblDue);
                DateTimePicker dtp = new DateTimePicker { Location = new Point(15, 155), Size = new Size(340, 30), Format = DateTimePickerFormat.Short };
                card.Controls.Add(dtp);

                Button btnSave = new Button { Text = "ASSIGN TASK", Location = new Point(15, 210), Size = new Size(340, 40) };
                AppTheme.StyleButton(btnSave, true);
                btnSave.BackColor = AppTheme.AccentSecondary;
                btnSave.Click += (s2, e2) =>
                {
                    if (!int.TryParse(txtStudentId.Text, out int studentId) || string.IsNullOrWhiteSpace(txtDesc.Text))
                    { MessageBox.Show("Please fill all fields correctly."); return; }
                    try
                    {
                        _societyService.AssignTask(_currentSocietyId, studentId, txtDesc.Text.Trim(), dtp.Value);
                        MessageBox.Show("Task assigned!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialog.Close();
                        LoadData();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                };
                card.Controls.Add(btnSave);
                dialog.ShowDialog(this);
            }
        }
    }
}
