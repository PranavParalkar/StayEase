using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmCustomer : Form
    {
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;
        private Button btnAdd = null!, btnEdit = null!, btnDelete = null!;
        private readonly CustomerService _svc = new CustomerService();

        public frmCustomer() { InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Customer Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            txtSearch = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(300, 30), Location = new Point(20, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Search by name, ID card, phone..." };
            txtSearch.TextChanged += (s, e) => { var r = string.IsNullOrEmpty(txtSearch.Text.Trim()) ? _svc.GetAllCustomers() : _svc.SearchCustomers(txtSearch.Text.Trim()); BindGrid(r); };

            btnAdd = MkBtn("➕ Add", 350, Color.FromArgb(34, 197, 94));
            btnEdit = MkBtn("✏️ Edit", 460, Color.FromArgb(59, 130, 246));
            btnDelete = MkBtn("🗑️ Delete", 570, Color.FromArgb(239, 68, 68));
            btnAdd.Click += (s, e) => { var d = new frmCustomerEdit(null); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; var c = _svc.GetAllCustomers().First(x => x.CustomerID == dgv.SelectedRows[0].Cells["CustomerID"].Value.ToString()); var d = new frmCustomerEdit(c); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnDelete.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteCustomer(dgv.SelectedRows[0].Cells["CustomerID"].Value.ToString()!); LoadData(); } };

            dgv = CreateGrid(20, 100, 980, 500);
            this.Controls.AddRange(new Control[] { lbl, txtSearch, btnAdd, btnEdit, btnDelete, dgv });
        }

        private void LoadData() => BindGrid(_svc.GetAllCustomers());
        private void BindGrid(List<Customer> data) => dgv.DataSource = data.Select(c => new { c.CustomerID, c.FullName, c.IDCard, Gender = c.GenderDisplay, c.Phone, c.Nationality, DOB = c.DateOfBirth?.ToString("yyyy-MM-dd") ?? "" }).ToList();
        private Button MkBtn(string t, int x, Color c) { var b = new Button { Text = t, Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(x, 55), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; return b; }
        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }

    public class frmCustomerEdit : Form
    {
        private TextBox txtName = null!, txtIDCard = null!, txtPhone = null!, txtAddress = null!, txtNationality = null!;
        private ComboBox cboGender = null!;
        private DateTimePicker dtpDOB = null!;
        private readonly Customer? _cust;
        private readonly CustomerService _svc = new CustomerService();

        public frmCustomerEdit(Customer? cust) { _cust = cust; InitializeComponent(); if (_cust != null) Populate(); }

        private void InitializeComponent()
        {
            this.Text = _cust == null ? "Add Customer" : "Edit Customer";
            this.Size = new Size(420, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 15;
            MkLabel("Full Name:", y); txtName = MkText(y += 25); y += 40;
            MkLabel("ID Card:", y); txtIDCard = MkText(y += 25); y += 40;
            MkLabel("Gender:", y); cboGender = MkCombo(y += 25, new[] { "Male", "Female" }); y += 40;
            MkLabel("Phone:", y); txtPhone = MkText(y += 25); y += 40;
            MkLabel("Address:", y); txtAddress = MkText(y += 25); y += 40;
            MkLabel("Nationality:", y); txtNationality = MkText(y += 25); txtNationality.Text = "Indian"; y += 40;
            MkLabel("Date of Birth:", y);
            dtpDOB = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpDOB); y += 45;

            var btnSave = new Button { Text = "Save", Size = new Size(120, 35), Location = new Point(80, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => Save();
            var btnCancel = new Button { Text = "Cancel", Size = new Size(120, 35), Location = new Point(210, y), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void Populate() { txtName.Text = _cust!.FullName; txtIDCard.Text = _cust.IDCard; cboGender.SelectedIndex = _cust.Gender; txtPhone.Text = _cust.Phone; txtAddress.Text = _cust.Address; txtNationality.Text = _cust.Nationality; if (_cust.DateOfBirth.HasValue) dtpDOB.Value = _cust.DateOfBirth.Value; }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name is required."); return; }
            var c = new Customer { CustomerID = _cust?.CustomerID ?? _svc.GenerateID(), FullName = txtName.Text.Trim(), IDCard = txtIDCard.Text.Trim(), Gender = (short)cboGender.SelectedIndex, Phone = txtPhone.Text.Trim(), Address = txtAddress.Text.Trim(), Nationality = txtNationality.Text.Trim(), DateOfBirth = dtpDOB.Value };
            bool ok = _cust == null ? _svc.AddCustomer(c) : _svc.UpdateCustomer(c);
            if (ok) { DialogResult = DialogResult.OK; Close(); } else MessageBox.Show("Failed.");
        }

        private void MkLabel(string t, int y) { this.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true }); }
        private TextBox MkText(int y) { var t = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }
        private ComboBox MkCombo(int y, string[] items) { var c = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList }; c.Items.AddRange(items); c.SelectedIndex = 0; this.Controls.Add(c); return c; }
    }
}
