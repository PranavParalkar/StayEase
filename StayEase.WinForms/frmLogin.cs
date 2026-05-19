using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmLogin : Form
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Panel pnlMain = null!;
        private CheckBox chkShowPassword = null!;
        private Label lblError = null!;

        private readonly AuthService _authService = new AuthService();

        public Account? LoggedInAccount { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "StayEase — Login";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(15, 23, 42);

            // Main panel
            pnlMain = new Panel
            {
                Size = new Size(380, 420),
                Location = new Point(260, 60),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(30)
            };

            // Title
            lblTitle = new Label
            {
                Text = "🏨 StayEase",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                AutoSize = true,
                Location = new Point(85, 25)
            };

            lblSubtitle = new Label
            {
                Text = "Hotel Management System",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(95, 65)
            };

            // Username
            var lblUser = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(203, 213, 225),
                Location = new Point(30, 110),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(30, 135),
                Size = new Size(320, 30),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Password
            var lblPass = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(203, 213, 225),
                Location = new Point(30, 180),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(30, 205),
                Size = new Size(320, 30),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(30, 245),
                AutoSize = true
            };
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            // Error label
            lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(248, 113, 113),
                Location = new Point(30, 275),
                Size = new Size(320, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Login button
            btnLogin = new Button
            {
                Text = "Sign In",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(320, 45),
                Location = new Point(30, 310),
                BackColor = Color.FromArgb(56, 189, 248),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Default credentials hint
            var lblHint = new Label
            {
                Text = "Default: admin / admin123",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 370),
                Size = new Size(320, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlMain.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, lblUser, txtUsername, lblPass, txtPassword, chkShowPassword, lblError, btnLogin, lblHint });
            this.Controls.Add(pnlMain);

            this.AcceptButton = btnLogin;
            txtUsername.Text = "admin";
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter username and password.";
                return;
            }

            try
            {
                Account? account = _authService.Login(username, password);
                if (account != null)
                {
                    LoggedInAccount = account;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = "Invalid username or password.";
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Connection error. Check SQL Server.";
                MessageBox.Show($"Database Error:\n{ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
