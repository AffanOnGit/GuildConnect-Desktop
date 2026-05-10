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
    /// Dashboard for Student users.
    /// Allows browsing societies, registering for events, viewing tasks, and checking tickets.
    /// </summary>
    public partial class StudentDashboard : Form
    {
        private User _currentUser;
        private readonly SocietyService _societyService;

        // Sidebar
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Content controls
        private DataGridView dgvSocieties;
        private DataGridView dgvEvents;
        private DataGridView dgvTasks;
        private DataGridView dgvTickets;
        private DataGridView dgvMemberships;

        private Button btnJoinSociety;
        private Button btnRegisterEvent;
        private Button btnRefresh;

        public StudentDashboard(User user)
        {
            _currentUser = user;
            _societyService = new SocietyService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = $"GuildConnect — Student Dashboard";
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
                Text = "STUDENT PANEL",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextMuted,
                Location = new Point(20, 55),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblRole);

            // Divider
            Panel divider = new Panel { BackColor = AppTheme.BorderSubtle, Size = new Size(190, 1), Location = new Point(15, 80) };
            sidebarPanel.Controls.Add(divider);

            // User info
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
            btnRefresh = AppTheme.CreateSidebarButton("↻  Refresh Data", 160, (s, e) => LoadData());
            sidebarPanel.Controls.Add(btnRefresh);

            Button btnLogout = AppTheme.CreateSidebarButton("⏻  Logout", 210, (s, e) => this.Close());
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

            // ── Societies Section ──────────────────────────────
            Label lblSocieties = new Label
            {
                Text = "📋  Available Societies",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblSocieties);
            contentPanel.Controls.Add(lblSocieties);
            yOffset += 30;

            dgvSocieties = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvSocieties);
            contentPanel.Controls.Add(dgvSocieties);
            yOffset += 128;

            btnJoinSociety = new Button
            {
                Text = "  REQUEST TO JOIN  ",
                Location = new Point(25, yOffset),
                Size = new Size(180, 36)
            };
            AppTheme.StyleButton(btnJoinSociety, true);
            btnJoinSociety.Click += BtnJoinSociety_Click;
            contentPanel.Controls.Add(btnJoinSociety);
            yOffset += 50;

            // ── My Memberships Section ─────────────────────────
            Label lblMemberships = new Label
            {
                Text = "🏷  My Membership Status",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblMemberships);
            contentPanel.Controls.Add(lblMemberships);
            yOffset += 30;

            dgvMemberships = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvMemberships);
            contentPanel.Controls.Add(dgvMemberships);
            yOffset += 115;

            // ── Events Section ─────────────────────────────────
            Label lblEvents = new Label
            {
                Text = "📅  Upcoming Events",
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

            btnRegisterEvent = new Button
            {
                Text = "  REGISTER FOR EVENT  ",
                Location = new Point(25, yOffset),
                Size = new Size(200, 36)
            };
            AppTheme.StyleButton(btnRegisterEvent, true);
            btnRegisterEvent.BackColor = AppTheme.AccentSecondary;
            btnRegisterEvent.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 200, 240);
            btnRegisterEvent.Click += BtnRegisterEvent_Click;
            contentPanel.Controls.Add(btnRegisterEvent);
            yOffset += 50;

            // ── My Tickets / Event Registrations Section ───────
            Label lblTickets = new Label
            {
                Text = "🎫  My Event Tickets / Passes",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblTickets);
            contentPanel.Controls.Add(lblTickets);
            yOffset += 30;

            dgvTickets = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvTickets);
            contentPanel.Controls.Add(dgvTickets);
            yOffset += 120;

            // ── Tasks Section ──────────────────────────────────
            Label lblTasks = new Label
            {
                Text = "✅  My Assigned Tasks",
                Location = new Point(25, yOffset),
            };
            AppTheme.StyleSectionLabel(lblTasks);
            contentPanel.Controls.Add(lblTasks);
            yOffset += 30;

            dgvTasks = new DataGridView
            {
                Location = new Point(25, yOffset),
                Size = new Size(contentWidth, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleDataGridView(dgvTasks);
            contentPanel.Controls.Add(dgvTasks);
        }

        private void LoadData()
        {
            try
            {
                dgvSocieties.DataSource = _societyService.GetApprovedSocieties();
                dgvMemberships.DataSource = _societyService.GetStudentMemberships(_currentUser.UserID);
                dgvEvents.DataSource = _societyService.GetUpcomingEvents();
                dgvTickets.DataSource = _societyService.GetStudentRegistrations(_currentUser.UserID);
                dgvTasks.DataSource = _societyService.GetTasksByStudent(_currentUser.UserID);

                // Hide internal ID columns for cleaner display
                HideColumn(dgvSocieties, "SocietyID");
                HideColumn(dgvMemberships, "MembershipID");
                HideColumn(dgvEvents, "EventID");
                HideColumn(dgvTickets, "RegistrationID");
                HideColumn(dgvTasks, "TaskID");
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

        private void BtnJoinSociety_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSocieties.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a society first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int societyId = Convert.ToInt32(dgvSocieties.SelectedRows[0].Cells["SocietyID"].Value);
                _societyService.RequestToJoin(_currentUser.UserID, societyId);
                MessageBox.Show("Join request sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Already Joined", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegisterEvent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEvents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select an event first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int eventId = Convert.ToInt32(dgvEvents.SelectedRows[0].Cells["EventID"].Value);
                _societyService.RegisterForEvent(_currentUser.UserID, eventId);
                MessageBox.Show("Successfully registered for event!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Already Registered", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
