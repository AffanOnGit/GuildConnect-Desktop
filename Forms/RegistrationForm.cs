using System;
using System.Drawing;
using System.Windows.Forms;
using GuildConnect_Desktop.Services;
using GuildConnect_Desktop.UI;

namespace GuildConnect_Desktop.Forms
{
    /// <summary>
    /// Registration form for new students.
    /// </summary>
    public partial class RegistrationForm : Form
    {
        private TextBox txtRollNumber;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnRegister;
        private Button btnBack;
        private Label lblMessage;

        private readonly AuthService _authService;

        public RegistrationForm()
        {
            _authService = new AuthService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "GuildConnect - Register";
            this.Size = new Size(480, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            AppTheme.StyleForm(this);

            // ── Center Card Panel ──────────────────────────────
            Panel cardPanel = new Panel
            {
                Size = new Size(380, 510),
                Location = new Point(50, 45),
                BackColor = AppTheme.PanelDark
            };
            this.Controls.Add(cardPanel);

            // ── Accent stripe ──────────────────────────────────
            Panel accentStripe = new Panel
            {
                Size = new Size(380, 4),
                Location = new Point(0, 0),
                BackColor = AppTheme.AccentSecondary
            };
            cardPanel.Controls.Add(accentStripe);

            // ── Title ──────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "Create Account",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(75, 25)
            };
            cardPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Join the GuildConnect community",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(85, 60)
            };
            cardPanel.Controls.Add(lblSubtitle);

            // ── Roll Number ────────────────────────────────────
            Label lblRoll = new Label
            {
                Text = "ROLL NUMBER",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 100),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblRoll);

            txtRollNumber = new TextBox
            {
                Location = new Point(35, 120),
                Size = new Size(310, AppTheme.InputHeight)
            };
            AppTheme.StyleTextBox(txtRollNumber);
            cardPanel.Controls.Add(txtRollNumber);

            // ── Full Name ──────────────────────────────────────
            Label lblName = new Label
            {
                Text = "FULL NAME",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 160),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblName);

            txtFullName = new TextBox
            {
                Location = new Point(35, 180),
                Size = new Size(310, AppTheme.InputHeight)
            };
            AppTheme.StyleTextBox(txtFullName);
            cardPanel.Controls.Add(txtFullName);

            // ── Email ──────────────────────────────────────────
            Label lblEmail = new Label
            {
                Text = "EMAIL",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 220),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(35, 240),
                Size = new Size(310, AppTheme.InputHeight)
            };
            AppTheme.StyleTextBox(txtEmail);
            cardPanel.Controls.Add(txtEmail);

            // ── Password ───────────────────────────────────────
            Label lblPassword = new Label
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 280),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(35, 300),
                Size = new Size(310, AppTheme.InputHeight),
                PasswordChar = '●'
            };
            AppTheme.StyleTextBox(txtPassword);
            cardPanel.Controls.Add(txtPassword);

            // ── Register Button ────────────────────────────────
            btnRegister = new Button
            {
                Text = "CREATE ACCOUNT",
                Location = new Point(35, 360),
                Size = new Size(310, 44)
            };
            AppTheme.StyleButton(btnRegister, true);
            btnRegister.BackColor = AppTheme.AccentSecondary;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 200, 240);
            btnRegister.Click += BtnRegister_Click;
            cardPanel.Controls.Add(btnRegister);

            // ── Back Button ────────────────────────────────────
            btnBack = new Button
            {
                Text = "← BACK TO LOGIN",
                Location = new Point(35, 415),
                Size = new Size(310, 38)
            };
            AppTheme.StyleButton(btnBack, false);
            btnBack.Click += (s, e) => this.Close();
            cardPanel.Controls.Add(btnBack);

            // ── Message Label ──────────────────────────────────
            lblMessage = new Label
            {
                Location = new Point(35, 462),
                Size = new Size(310, 35),
                ForeColor = AppTheme.DangerColor,
                Font = AppTheme.FontSmall,
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(lblMessage);

            this.AcceptButton = btnRegister;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.ForeColor = AppTheme.DangerColor;
                string roll = txtRollNumber.Text.Trim();
                string name = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string pass = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(roll) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
                {
                    lblMessage.Text = "All fields are required.";
                    return;
                }

                bool success = _authService.Register(roll, name, email, pass);
                if (success)
                {
                    lblMessage.ForeColor = AppTheme.SuccessColor;
                    lblMessage.Text = "Account created! You can now log in.";
                    btnRegister.Enabled = false;
                }
                else
                {
                    lblMessage.Text = "Registration failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate"))
                    lblMessage.Text = "Email or Roll Number already in use.";
                else
                    lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}
