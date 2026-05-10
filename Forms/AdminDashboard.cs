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
    /// Dashboard for Admin users.
    /// Full oversight: manage users, create/approve/suspend/delete societies, approve events, export reports.
    /// </summary>
    public partial class AdminDashboard : Form
    {
        private User _currentUser;
        private readonly AdminService _adminService;
        private readonly SocietyService _societyService;

        // Sidebar
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Content controls
        private DataGridView dgvUsers;
        private DataGridView dgvSocieties;
        private DataGridView dgvEvents;

        private Button btnToggleUserStatus;
        private Button btnCreateSociety;
        private Button btnApproveSociety;
        private Button btnSuspendSociety;
        private Button btnDeleteSociety;
        private Button btnApproveEvent;

        public AdminDashboard(User user)
        {
            _currentUser = user;
            _adminService = new AdminService();
            _societyService = new SocietyService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "GuildConnect — Admin Dashboard";
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
                Text = "ADMINISTRATOR",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.DangerColor,
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

            // ── Users Section ──────────────────────────────────
            Label lblUsers = new Label
            {
                Text = "👤  Manage Users",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblUsers);
            contentPanel.Controls.Add(lblUsers);
            yOffset += 30;

            dgvUsers = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvUsers);
            contentPanel.Controls.Add(dgvUsers);
            yOffset += 138;

            btnToggleUserStatus = new Button { Text = "  ⟳ TOGGLE ACTIVE STATUS  ", Location = new Point(25, yOffset), Size = new Size(220, 36) };
            AppTheme.StyleButton(btnToggleUserStatus, true);
            btnToggleUserStatus.BackColor = AppTheme.WarningColor;
            btnToggleUserStatus.ForeColor = AppTheme.BackgroundDark;
            btnToggleUserStatus.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 70);
            btnToggleUserStatus.Click += BtnToggleUserStatus_Click;
            contentPanel.Controls.Add(btnToggleUserStatus);
            yOffset += 50;

            // ── Societies Section ──────────────────────────────
            Label lblSocieties = new Label
            {
                Text = "🏛  Manage Societies",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblSocieties);
            contentPanel.Controls.Add(lblSocieties);
            yOffset += 30;

            dgvSocieties = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvSocieties);
            contentPanel.Controls.Add(dgvSocieties);
            yOffset += 138;

            // Society action buttons row
            btnCreateSociety = new Button { Text = "  + CREATE  ", Location = new Point(25, yOffset), Size = new Size(130, 36) };
            AppTheme.StyleButton(btnCreateSociety, true);
            btnCreateSociety.Click += BtnCreateSociety_Click;
            contentPanel.Controls.Add(btnCreateSociety);

            btnApproveSociety = new Button { Text = "  ✓ APPROVE  ", Location = new Point(165, yOffset), Size = new Size(135, 36) };
            AppTheme.StyleSuccessButton(btnApproveSociety);
            btnApproveSociety.Click += BtnApproveSociety_Click;
            contentPanel.Controls.Add(btnApproveSociety);

            btnSuspendSociety = new Button { Text = "  ⏸ SUSPEND  ", Location = new Point(310, yOffset), Size = new Size(140, 36) };
            AppTheme.StyleButton(btnSuspendSociety, false);
            btnSuspendSociety.BackColor = AppTheme.WarningColor;
            btnSuspendSociety.ForeColor = AppTheme.BackgroundDark;
            btnSuspendSociety.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 70);
            btnSuspendSociety.Click += BtnSuspendSociety_Click;
            contentPanel.Controls.Add(btnSuspendSociety);

            btnDeleteSociety = new Button { Text = "  🗑 DELETE  ", Location = new Point(460, yOffset), Size = new Size(130, 36) };
            AppTheme.StyleDangerButton(btnDeleteSociety);
            btnDeleteSociety.Click += BtnDeleteSociety_Click;
            contentPanel.Controls.Add(btnDeleteSociety);
            yOffset += 50;

            // ── Events Section ─────────────────────────────────
            Label lblEvents = new Label
            {
                Text = "📅  All Events",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblEvents);
            contentPanel.Controls.Add(lblEvents);
            yOffset += 30;

            dgvEvents = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvEvents);
            contentPanel.Controls.Add(dgvEvents);
            yOffset += 138;

            btnApproveEvent = new Button { Text = "  ✓ APPROVE EVENT  ", Location = new Point(25, yOffset), Size = new Size(180, 36) };
            AppTheme.StyleSuccessButton(btnApproveEvent);
            btnApproveEvent.Click += BtnApproveEvent_Click;
            contentPanel.Controls.Add(btnApproveEvent);
        }

        private void LoadData()
        {
            try
            {
                dgvUsers.DataSource = _adminService.GetAllUsers();
                dgvSocieties.DataSource = _societyService.GetAllSocieties();
                dgvEvents.DataSource = _societyService.GetAllEvents();

                HideColumn(dgvUsers, "UserID");
                HideColumn(dgvSocieties, "SocietyID");
                HideColumn(dgvEvents, "EventID");
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
                "UniversityWideReport.csv",
                ("All Users", dgvUsers),
                ("All Societies", dgvSocieties),
                ("All Events", dgvEvents)
            );
        }

        // ═══════════════════════════════════════════════════════
        //  USER ACTIONS
        // ═══════════════════════════════════════════════════════

        private void BtnToggleUserStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count == 0) { MessageBox.Show("Select a user first."); return; }
                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
                _adminService.ToggleUserStatus(userId);
                MessageBox.Show("User status toggled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ═══════════════════════════════════════════════════════
        //  SOCIETY ACTIONS (Create, Approve, Suspend, Delete)
        // ═══════════════════════════════════════════════════════

        private void BtnCreateSociety_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Create New Society";
                dialog.Size = new Size(420, 340);
                dialog.StartPosition = FormStartPosition.CenterParent;
                AppTheme.StyleForm(dialog);

                Panel card = new Panel { Location = new Point(20, 15), Size = new Size(370, 270), BackColor = AppTheme.PanelDark };
                dialog.Controls.Add(card);

                Label lblN = new Label { Text = "SOCIETY NAME", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 15), AutoSize = true };
                card.Controls.Add(lblN);
                TextBox txtName = new TextBox { Location = new Point(15, 35), Size = new Size(340, 30) };
                AppTheme.StyleTextBox(txtName);
                card.Controls.Add(txtName);

                Label lblD = new Label { Text = "DESCRIPTION", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 75), AutoSize = true };
                card.Controls.Add(lblD);
                TextBox txtDesc = new TextBox { Location = new Point(15, 95), Size = new Size(340, 30) };
                AppTheme.StyleTextBox(txtDesc);
                card.Controls.Add(txtDesc);

                Label lblH = new Label { Text = "HEAD USER ID", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = AppTheme.TextSecondary, Location = new Point(15, 135), AutoSize = true };
                card.Controls.Add(lblH);
                TextBox txtHeadId = new TextBox { Location = new Point(15, 155), Size = new Size(340, 30) };
                AppTheme.StyleTextBox(txtHeadId);
                card.Controls.Add(txtHeadId);

                Button btnSave = new Button { Text = "CREATE SOCIETY", Location = new Point(15, 210), Size = new Size(340, 40) };
                AppTheme.StyleButton(btnSave, true);
                btnSave.Click += (s2, e2) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtHeadId.Text, out int headId))
                    { MessageBox.Show("Please fill all fields correctly."); return; }
                    try
                    {
                        _adminService.CreateSociety(txtName.Text.Trim(), txtDesc.Text.Trim(), headId);
                        MessageBox.Show("Society created (status: Pending).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialog.Close();
                        LoadData();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                };
                card.Controls.Add(btnSave);
                dialog.ShowDialog(this);
            }
        }

        private void BtnApproveSociety_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSocieties.SelectedRows.Count == 0) { MessageBox.Show("Select a society first."); return; }
                int societyId = Convert.ToInt32(dgvSocieties.SelectedRows[0].Cells["SocietyID"].Value);
                if (_adminService.ApproveSociety(societyId))
                    MessageBox.Show("Society approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Society is not in Pending state.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnSuspendSociety_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSocieties.SelectedRows.Count == 0) { MessageBox.Show("Select a society first."); return; }
                int societyId = Convert.ToInt32(dgvSocieties.SelectedRows[0].Cells["SocietyID"].Value);
                string name = dgvSocieties.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "";

                var confirm = MessageBox.Show($"Suspend society \"{name}\"?", "Confirm Suspend",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    _adminService.SuspendSociety(societyId);
                    MessageBox.Show("Society suspended.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnDeleteSociety_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSocieties.SelectedRows.Count == 0) { MessageBox.Show("Select a society first."); return; }
                int societyId = Convert.ToInt32(dgvSocieties.SelectedRows[0].Cells["SocietyID"].Value);
                string name = dgvSocieties.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "";

                var confirm = MessageBox.Show(
                    $"PERMANENTLY DELETE society \"{name}\" and ALL its events, tasks, and memberships?\n\nThis cannot be undone!",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    _adminService.DeleteSociety(societyId);
                    MessageBox.Show("Society deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ═══════════════════════════════════════════════════════
        //  EVENT ACTIONS
        // ═══════════════════════════════════════════════════════

        private void BtnApproveEvent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEvents.SelectedRows.Count == 0) { MessageBox.Show("Select an event first."); return; }
                int eventId = Convert.ToInt32(dgvEvents.SelectedRows[0].Cells["EventID"].Value);
                if (_adminService.ApproveEvent(eventId))
                    MessageBox.Show("Event approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Event is not in PendingApproval state.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
