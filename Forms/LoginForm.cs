using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GuildConnect_Desktop.Services;
using GuildConnect_Desktop.Models;
using GuildConnect_Desktop.UI;

namespace GuildConnect_Desktop.Forms
{
    /// <summary>
    /// The main authentication form.
    /// Routes the user to the correct dashboard based on their role.
    /// </summary>
    public partial class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblMessage;

        private readonly AuthService _authService;

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "GuildConnect";
            this.Size = new Size(480, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            AppTheme.StyleForm(this);

            // ── Center Card Panel ──────────────────────────────
            Panel cardPanel = new Panel
            {
                Size = new Size(380, 440),
                Location = new Point(50, 50),
                BackColor = AppTheme.PanelDark
            };
            this.Controls.Add(cardPanel);

            // ── Accent stripe at top of card ───────────────────
            Panel accentStripe = new Panel
            {
                Size = new Size(380, 4),
                Location = new Point(0, 0),
                BackColor = AppTheme.AccentPrimary
            };
            cardPanel.Controls.Add(accentStripe);

            // ── App Icon / Title ───────────────────────────────
            Label lblIcon = new Label
            {
                Text = "⚔",
                Font = new Font("Segoe UI Emoji", 36f),
                ForeColor = AppTheme.AccentPrimary,
                AutoSize = true,
                Location = new Point(155, 25)
            };
            cardPanel.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = "GuildConnect",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(90, 85)
            };
            cardPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Sign in to your account",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(120, 118)
            };
            cardPanel.Controls.Add(lblSubtitle);

            // ── Email Field ────────────────────────────────────
            Label lblEmail = new Label
            {
                Text = "EMAIL",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 155),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(35, 175),
                Size = new Size(310, AppTheme.InputHeight)
            };
            AppTheme.StyleTextBox(txtEmail);
            cardPanel.Controls.Add(txtEmail);

            // ── Password Field ─────────────────────────────────
            Label lblPassword = new Label
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(35, 215),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(35, 235),
                Size = new Size(310, AppTheme.InputHeight),
                PasswordChar = '●'
            };
            AppTheme.StyleTextBox(txtPassword);
            cardPanel.Controls.Add(txtPassword);

            // ── Login Button ───────────────────────────────────
            btnLogin = new Button
            {
                Text = "SIGN IN",
                Location = new Point(35, 295),
                Size = new Size(310, 44)
            };
            AppTheme.StyleButton(btnLogin, true);
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // ── Divider ────────────────────────────────────────
            Label lblDivider = new Label
            {
                Text = "─────────  or  ─────────",
                ForeColor = AppTheme.TextMuted,
                Font = AppTheme.FontSmall,
                AutoSize = true,
                Location = new Point(95, 352)
            };
            cardPanel.Controls.Add(lblDivider);

            // ── Register Button ────────────────────────────────
            btnRegister = new Button
            {
                Text = "CREATE NEW ACCOUNT",
                Location = new Point(35, 380),
                Size = new Size(310, 40)
            };
            AppTheme.StyleButton(btnRegister, false);
            btnRegister.Click += BtnRegister_Click;
            cardPanel.Controls.Add(btnRegister);

            // ── Message Label ──────────────────────────────────
            lblMessage = new Label
            {
                Location = new Point(35, 430),
                Size = new Size(310, 20),
                ForeColor = AppTheme.DangerColor,
                Font = AppTheme.FontSmall,
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(lblMessage);

            // ── Allow Enter key to submit ──────────────────────
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    lblMessage.Text = "Please enter email and password.";
                    return;
                }

                User user = _authService.Login(email, password);

                if (user != null)
                {
                    this.Hide();
                    // Route to correct dashboard
                    switch (user.Role)
                    {
                        case "Student":
                            new StudentDashboard(user).ShowDialog();
                            break;
                        case "SocietyHead":
                            new SocietyHeadDashboard(user).ShowDialog();
                            break;
                        case "Admin":
                            new AdminDashboard(user).ShowDialog();
                            break;
                    }
                    this.Close(); // Close app when dashboard closes
                }
                else
                {
                    lblMessage.Text = "Invalid credentials or inactive account.";
                }
            }
            catch (Exception ex)
            {
                // Show the real exception — critical for diagnosing DB issues
                MessageBox.Show(
                    $"Connection Error:\n\n{ex.Message}\n\n{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            new RegistrationForm().ShowDialog();
        }
    }
}
