using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmEmployee : Form
    {
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;
        private Button btnAdd = null!, btnEdit = null!, btnDelete = null!;
        private readonly EmployeeService _svc = new EmployeeService();

        public frmEmployee() { InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Employee Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            txtSearch = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(300, 30), Location = new Point(20, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Search employees..." };
            txtSearch.TextChanged += (s, e) => { var r = string.IsNullOrEmpty(txtSearch.Text.Trim()) ? _svc.GetAllEmployees() : _svc.SearchEmployees(txtSearch.Text.Trim()); BindGrid(r); };

            btnAdd = MkBtn("➕ Add", 350, Color.FromArgb(34, 197, 94));
            btnEdit = MkBtn("✏️ Edit", 460, Color.FromArgb(59, 130, 246));
            btnDelete = MkBtn("🗑️ Delete", 570, Color.FromArgb(239, 68, 68));
            btnAdd.Click += (s, e) => { var d = new frmEmployeeEdit(null); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; var emp = _svc.GetAllEmployees().First(x => x.EmployeeID == dgv.SelectedRows[0].Cells["EmployeeID"].Value.ToString()); var d = new frmEmployeeEdit(emp); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnDelete.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteEmployee(dgv.SelectedRows[0].Cells["EmployeeID"].Value.ToString()!); LoadData(); } };

            dgv = CreateGrid(20, 100, 980, 500);
            this.Controls.AddRange(new Control[] { lbl, txtSearch, btnAdd, btnEdit, btnDelete, dgv });
        }

        private void LoadData() => BindGrid(_svc.GetAllEmployees());
        private void BindGrid(List<Employee> data) => dgv.DataSource = data.Select(e => new { e.EmployeeID, e.FullName, Gender = e.GenderDisplay, Position = e.PositionDisplay, e.Email, e.DailyWage, HireDate = e.HireDate?.ToString("yyyy-MM-dd") ?? "" }).ToList();
        private Button MkBtn(string t, int x, Color c) { var b = new Button { Text = t, Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(x, 55), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; return b; }
        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }

    public class frmEmployeeEdit : Form
    {
        private TextBox txtName = null!, txtEmail = null!, txtWage = null!, txtLeave = null!;
        private ComboBox cboGender = null!, cboPosition = null!;
        private DateTimePicker dtpDOB = null!, dtpHire = null!;
        private readonly Employee? _emp;
        private readonly EmployeeService _svc = new EmployeeService();

        public frmEmployeeEdit(Employee? emp) { _emp = emp; InitializeComponent(); if (_emp != null) Populate(); }

        private void InitializeComponent()
        {
            this.Text = _emp == null ? "Add Employee" : "Edit Employee";
            this.Size = new Size(420, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 15;
            MkLbl("Full Name:", y); txtName = MkTxt(y += 25); y += 40;
            MkLbl("Gender:", y); cboGender = MkCbo(y += 25, new[] { "Male", "Female" }); y += 40;
            MkLbl("Position:", y); cboPosition = MkCbo(y += 25, new[] { "Manager", "Receptionist" }); y += 40;
            MkLbl("Email:", y); txtEmail = MkTxt(y += 25); y += 40;
            MkLbl("Daily Wage:", y); txtWage = MkTxt(y += 25); y += 40;
            MkLbl("Leave Days:", y); txtLeave = MkTxt(y += 25); txtLeave.Text = "12"; y += 40;
            MkLbl("Date of Birth:", y); dtpDOB = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), Format = DateTimePickerFormat.Short }; this.Controls.Add(dtpDOB); y += 35;
            MkLbl("Hire Date:", y); dtpHire = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), Format = DateTimePickerFormat.Short }; this.Controls.Add(dtpHire); y += 45;

            var btnSave = new Button { Text = "Save", Size = new Size(120, 35), Location = new Point(80, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += (s, e) => Save();
            var btnCancel = new Button { Text = "Cancel", Size = new Size(120, 35), Location = new Point(210, y), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnCancel.FlatAppearance.BorderSize = 0; btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void Populate() { txtName.Text = _emp!.FullName; cboGender.SelectedIndex = _emp.Gender; cboPosition.SelectedIndex = _emp.Position; txtEmail.Text = _emp.Email; txtWage.Text = _emp.DailyWage.ToString(); txtLeave.Text = _emp.LeaveDays.ToString(); if (_emp.DateOfBirth.HasValue) dtpDOB.Value = _emp.DateOfBirth.Value; if (_emp.HireDate.HasValue) dtpHire.Value = _emp.HireDate.Value; }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtWage.Text, out int wage)) { MessageBox.Show("Fill all fields correctly."); return; }
            short.TryParse(txtLeave.Text, out short leave);
            var emp = new Employee { EmployeeID = _emp?.EmployeeID ?? _svc.GenerateID(), FullName = txtName.Text.Trim(), Gender = (short)cboGender.SelectedIndex, Position = (short)cboPosition.SelectedIndex, Email = txtEmail.Text.Trim(), DailyWage = wage, LeaveDays = leave, DateOfBirth = dtpDOB.Value, HireDate = dtpHire.Value };
            bool ok = _emp == null ? _svc.AddEmployee(emp) : _svc.UpdateEmployee(emp);
            if (ok) { DialogResult = DialogResult.OK; Close(); } else MessageBox.Show("Failed.");
        }

        private void MkLbl(string t, int y) { this.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true }); }
        private TextBox MkTxt(int y) { var t = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }
        private ComboBox MkCbo(int y, string[] items) { var c = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList }; c.Items.AddRange(items); c.SelectedIndex = 0; this.Controls.Add(c); return c; }
    }
}
