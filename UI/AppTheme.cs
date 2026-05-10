using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GuildConnect_Desktop.UI
{
    /// <summary>
    /// Centralized UI theming class for consistent modern styling across all forms.
    /// Keeps styling logic decoupled from form logic (low CBO).
    /// </summary>
    public static class AppTheme
    {
        // ── Color Palette ──────────────────────────────────────
        public static readonly Color BackgroundDark   = Color.FromArgb(22, 22, 37);      // #161625
        public static readonly Color PanelDark        = Color.FromArgb(30, 30, 50);      // #1E1E32
        public static readonly Color SurfaceDark      = Color.FromArgb(40, 40, 65);      // #282841
        public static readonly Color InputBackground  = Color.FromArgb(45, 45, 72);      // #2D2D48
        public static readonly Color BorderSubtle     = Color.FromArgb(60, 60, 95);      // #3C3C5F
        public static readonly Color TextPrimary      = Color.FromArgb(230, 230, 245);   // #E6E6F5
        public static readonly Color TextSecondary    = Color.FromArgb(160, 160, 190);   // #A0A0BE
        public static readonly Color TextMuted        = Color.FromArgb(110, 110, 140);   // #6E6E8C
        public static readonly Color AccentPrimary    = Color.FromArgb(110, 90, 230);    // #6E5AE6  (Vibrant Purple)
        public static readonly Color AccentHover      = Color.FromArgb(130, 110, 250);   // #826EFA
        public static readonly Color AccentSecondary  = Color.FromArgb(60, 180, 220);    // #3CB4DC  (Teal)
        public static readonly Color SuccessColor     = Color.FromArgb(80, 200, 120);    // #50C878
        public static readonly Color DangerColor      = Color.FromArgb(230, 80, 80);     // #E65050
        public static readonly Color WarningColor     = Color.FromArgb(240, 180, 50);    // #F0B432
        public static readonly Color SidebarColor     = Color.FromArgb(18, 18, 30);      // #12121E

        // ── Grid Colors ────────────────────────────────────────
        public static readonly Color GridHeaderBack   = Color.FromArgb(35, 35, 58);
        public static readonly Color GridRowAlt       = Color.FromArgb(28, 28, 46);
        public static readonly Color GridRowNormal    = Color.FromArgb(24, 24, 40);
        public static readonly Color GridSelection    = Color.FromArgb(80, 70, 160);

        // ── Fonts ──────────────────────────────────────────────
        public static readonly Font FontTitle       = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static readonly Font FontSubtitle    = new Font("Segoe UI", 14f, FontStyle.Regular);
        public static readonly Font FontHeading      = new Font("Segoe UI Semibold", 12f, FontStyle.Bold);
        public static readonly Font FontBody         = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontSmall        = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font FontButton       = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);

        // ── Dimensions ─────────────────────────────────────────
        public static readonly int CornerRadius = 12;
        public static readonly int InputHeight  = 38;
        public static readonly int ButtonHeight = 40;

        // ═══════════════════════════════════════════════════════
        //  STYLING METHODS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Applies the dark theme to any Form.
        /// </summary>
        public static void StyleForm(Form form)
        {
            form.BackColor = BackgroundDark;
            form.ForeColor = TextPrimary;
            form.Font = FontBody;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
        }

        /// <summary>
        /// Applies the dark theme to a resizable dashboard Form.
        /// </summary>
        public static void StyleDashboardForm(Form form)
        {
            form.BackColor = BackgroundDark;
            form.ForeColor = TextPrimary;
            form.Font = FontBody;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MinimumSize = new Size(900, 650);
        }

        /// <summary>
        /// Creates a styled primary action button.
        /// </summary>
        public static void StyleButton(Button btn, bool isPrimary = true)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = isPrimary ? AccentPrimary : SurfaceDark;
            btn.ForeColor = TextPrimary;
            btn.Font = FontButton;
            btn.Height = ButtonHeight;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = isPrimary ? AccentHover : BorderSubtle;
            btn.FlatAppearance.MouseDownBackColor = isPrimary ? AccentPrimary : SurfaceDark;

            // Rounded corners
            btn.Region = CreateRoundedRegion(btn.Width, btn.Height, 8);
            btn.Resize += (s, e) => btn.Region = CreateRoundedRegion(btn.Width, btn.Height, 8);
        }

        /// <summary>
        /// Creates a styled danger button (red).
        /// </summary>
        public static void StyleDangerButton(Button btn)
        {
            StyleButton(btn, false);
            btn.BackColor = DangerColor;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 100, 100);
        }

        /// <summary>
        /// Creates a styled success button (green).
        /// </summary>
        public static void StyleSuccessButton(Button btn)
        {
            StyleButton(btn, false);
            btn.BackColor = SuccessColor;
            btn.ForeColor = BackgroundDark;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 220, 140);
        }

        /// <summary>
        /// Styles a TextBox to match the dark theme.
        /// </summary>
        public static void StyleTextBox(TextBox txt)
        {
            txt.BackColor = InputBackground;
            txt.ForeColor = TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = FontBody;
            txt.Height = InputHeight;
        }

        /// <summary>
        /// Styles a Label as a section heading.
        /// </summary>
        public static void StyleSectionLabel(Label lbl)
        {
            lbl.Font = FontHeading;
            lbl.ForeColor = AccentSecondary;
            lbl.AutoSize = true;
        }

        /// <summary>
        /// Styles a Label as a muted subtitle.
        /// </summary>
        public static void StyleSubtitle(Label lbl)
        {
            lbl.Font = FontSubtitle;
            lbl.ForeColor = TextSecondary;
            lbl.AutoSize = true;
        }

        /// <summary>
        /// Comprehensively styles a DataGridView to match the modern dark theme.
        /// </summary>
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BorderSubtle;

            // Background
            dgv.BackgroundColor = PanelDark;
            dgv.DefaultCellStyle.BackColor = GridRowNormal;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = GridSelection;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontBody;
            dgv.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);

            // Alternating rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlt;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridSelection;

            // Column headers
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBack;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = AccentSecondary;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontButton;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 6, 6, 6);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Row headers (hide)
            dgv.RowHeadersVisible = false;

            // Auto size columns
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Selection mode
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            // Row sizing
            dgv.RowTemplate.Height = 35;
        }

        /// <summary>
        /// Creates a styled Panel that acts as a card/container.
        /// </summary>
        public static Panel CreateCard(int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = PanelDark,
                Padding = new Padding(16)
            };
            return panel;
        }

        /// <summary>
        /// Creates a sidebar panel for dashboard navigation.
        /// </summary>
        public static Panel CreateSidebar(int width, int formHeight)
        {
            var panel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(width, formHeight),
                BackColor = SidebarColor,
                Dock = DockStyle.Left
            };
            return panel;
        }

        /// <summary>
        /// Creates a sidebar navigation button.
        /// </summary>
        public static Button CreateSidebarButton(string text, int yOffset, EventHandler clickHandler)
        {
            var btn = new Button
            {
                Text = "  " + text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = TextSecondary,
                Font = FontBody,
                Size = new Size(200, 45),
                Location = new Point(0, yOffset),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = SurfaceDark;
            if (clickHandler != null) btn.Click += clickHandler;
            return btn;
        }

        /// <summary>
        /// Creates a status badge label.
        /// </summary>
        public static Label CreateStatusBadge(string text, Color bgColor, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                BackColor = bgColor,
                ForeColor = TextPrimary,
                Font = FontSmall,
                AutoSize = false,
                Size = new Size(90, 26),
                Location = new Point(x, y),
                TextAlign = ContentAlignment.MiddleCenter
            };
            return lbl;
        }

        // ═══════════════════════════════════════════════════════
        //  HELPER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Creates a rounded rectangle region for a control.
        /// </summary>
        private static Region CreateRoundedRegion(int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }
    }
}
