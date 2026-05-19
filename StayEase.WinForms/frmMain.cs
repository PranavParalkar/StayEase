using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    /// <summary>
    /// Main Dashboard MDI Parent form with navigation sidebar.
    /// Menu items visibility controlled by role-based permissions.
    /// </summary>
    public class frmMain : Form
    {
        private Panel pnlSidebar = null!;
        private Panel pnlContent = null!;
        private Panel pnlHeader = null!;
        private Label lblWelcome = null!;
        private Label lblRole = null!;
        private Button btnRoom = null!;
        private Button btnService = null!;
        private Button btnCustomer = null!;
        private Button btnEmployee = null!;
        private Button btnBooking = null!;
        private Button btnInvoice = null!;
        private Button btnStatistics = null!;
        private Button btnRole = null!;
        private Button btnLogout = null!;

        private readonly Account _account;
        private readonly AuthService _authService = new AuthService();
        private readonly List<RoleFeature> _permissions;

        public frmMain(Account account)
        {
            _account = account;
            _permissions = _authService.GetRolePermissions(account.RoleID);
            InitializeComponent();
            ApplyPermissions();
        }

        private void InitializeComponent()
        {
            this.Text = "StayEase — Hotel Management System";
            this.Size = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.MinimumSize = new Size(1100, 600);

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            var lblApp = new Label
            {
                Text = "🏨 StayEase",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            string empName = _authService.GetEmployeeName(_account.EmployeeID);
            string roleName = _authService.GetRoleName(_account.RoleID);

            lblWelcome = new Label
            {
                Text = $"Welcome, {empName}",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(203, 213, 225),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(900, 10)
            };

            lblRole = new Label
            {
                Text = $"Role: {roleName}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(900, 35)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblApp, lblWelcome, lblRole });

            // Sidebar
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(10, 10, 10, 10)
            };

            int btnY = 10;
            int btnH = 45;
            int gap = 5;

            btnRoom = CreateNavButton("🛏  Rooms", btnY); btnY += btnH + gap;
            btnService = CreateNavButton("🍽  Services", btnY); btnY += btnH + gap;
            btnCustomer = CreateNavButton("👤  Customers", btnY); btnY += btnH + gap;
            btnEmployee = CreateNavButton("👨‍💼  Employees", btnY); btnY += btnH + gap;
            btnBooking = CreateNavButton("📋  Bookings", btnY); btnY += btnH + gap;
            btnInvoice = CreateNavButton("💰  Invoices", btnY); btnY += btnH + gap;
            btnStatistics = CreateNavButton("📊  Statistics", btnY); btnY += btnH + gap;
            btnRole = CreateNavButton("🔐  Roles", btnY); btnY += btnH + gap + 20;
            btnLogout = CreateNavButton("🚪  Logout", btnY);
            btnLogout.BackColor = Color.FromArgb(127, 29, 29);

            pnlSidebar.Controls.AddRange(new Control[] { btnRoom, btnService, btnCustomer, btnEmployee, btnBooking, btnInvoice, btnStatistics, btnRole, btnLogout });

            // Content Area
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(20)
            };

            // Welcome content
            var lblDashTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var lblDashSub = new Label
            {
                Text = "Welcome to StayEase Hotel Management System.\nSelect a module from the sidebar to get started.",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(30, 75)
            };

            pnlContent.Controls.AddRange(new Control[] { lblDashTitle, lblDashSub });

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);

            // Event handlers
            btnRoom.Click += (s, e) => LoadForm(new frmRoom());
            btnService.Click += (s, e) => LoadForm(new frmService());
            btnCustomer.Click += (s, e) => LoadForm(new frmCustomer());
            btnEmployee.Click += (s, e) => LoadForm(new frmEmployee());
            btnBooking.Click += (s, e) => LoadForm(new frmBooking(_account));
            btnInvoice.Click += (s, e) => LoadForm(new frmInvoice());
            btnStatistics.Click += (s, e) => LoadForm(new frmStatistics());
            btnRole.Click += (s, e) => LoadForm(new frmRole());
            btnLogout.Click += BtnLogout_Click;
        }

        private Button CreateNavButton(string text, int y)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 45),
                Location = new Point(10, y),
                ForeColor = Color.FromArgb(226, 232, 240),
                BackColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(56, 189, 248);
            btn.MouseLeave += (s, e) =>
            {
                if (btn == btnLogout)
                    btn.BackColor = Color.FromArgb(127, 29, 29);
                else
                    btn.BackColor = Color.FromArgb(51, 65, 85);
            };
            return btn;
        }

        private void LoadForm(Form form)
        {
            pnlContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }

        private void ApplyPermissions()
        {
            // Feature IDs: F001=Room, F002=Service, F003=Customer, F004=Employee, F005=Role, F006=Booking, F007=Invoice, F008=Statistics
            var permIds = _permissions.Select(p => p.FeatureID).ToHashSet();
            btnRoom.Visible = permIds.Contains("F001");
            btnService.Visible = permIds.Contains("F002");
            btnCustomer.Visible = permIds.Contains("F003");
            btnEmployee.Visible = permIds.Contains("F004");
            btnRole.Visible = permIds.Contains("F005");
            btnBooking.Visible = permIds.Contains("F006");
            btnInvoice.Visible = permIds.Contains("F007");
            btnStatistics.Visible = permIds.Contains("F008");
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
        }
    }
}
