using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmService : Form
    {
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;
        private ComboBox cboCategory = null!;
        private Button btnAdd = null!, btnEdit = null!, btnDelete = null!;
        private readonly HotelServiceService _svc = new HotelServiceService();

        public frmService() { InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Service Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            txtSearch = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(250, 30), Location = new Point(20, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Search services..." };
            txtSearch.TextChanged += (s, e) => Filter();

            cboCategory = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(180, 30), Location = new Point(290, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cboCategory.Items.AddRange(new object[] { "All Categories", "Food & Beverage", "Beauty Care", "Entertainment", "Party Services" });
            cboCategory.SelectedIndex = 0;
            cboCategory.SelectedIndexChanged += (s, e) => Filter();

            btnAdd = MkBtn("➕ Add", 500, Color.FromArgb(34, 197, 94));
            btnEdit = MkBtn("✏️ Edit", 610, Color.FromArgb(59, 130, 246));
            btnDelete = MkBtn("🗑️ Delete", 720, Color.FromArgb(239, 68, 68));
            btnAdd.Click += (s, e) => { var d = new frmServiceEdit(null); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; var svc = _svc.GetAllServices().First(x => x.ServiceID == dgv.SelectedRows[0].Cells["ServiceID"].Value.ToString()); var d = new frmServiceEdit(svc); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnDelete.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteService(dgv.SelectedRows[0].Cells["ServiceID"].Value.ToString()!); LoadData(); } };

            dgv = CreateGrid(20, 100, 980, 500);
            this.Controls.AddRange(new Control[] { lbl, txtSearch, cboCategory, btnAdd, btnEdit, btnDelete, dgv });
        }

        private void LoadData() => BindGrid(_svc.GetAllServices());

        private void Filter()
        {
            var list = string.IsNullOrEmpty(txtSearch.Text.Trim()) ? _svc.GetAllServices() : _svc.SearchServices(txtSearch.Text.Trim());
            if (cboCategory.SelectedIndex > 0)
                list = list.Where(s => s.ServiceType == cboCategory.Items[cboCategory.SelectedIndex].ToString()).ToList();
            BindGrid(list);
        }

        private void BindGrid(List<Service> data) => dgv.DataSource = data.Select(s => new { s.ServiceID, s.ServiceName, s.ServiceType, s.Price }).ToList();
        private Button MkBtn(string t, int x, Color c) { var b = new Button { Text = t, Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(x, 55), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; return b; }
        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }

    public class frmServiceEdit : Form
    {
        private TextBox txtName = null!, txtPrice = null!;
        private ComboBox cboType = null!;
        private readonly Service? _svc;
        private readonly HotelServiceService _bus = new HotelServiceService();

        public frmServiceEdit(Service? svc) { _svc = svc; InitializeComponent(); if (_svc != null) Populate(); }

        private void InitializeComponent()
        {
            this.Text = _svc == null ? "Add Service" : "Edit Service";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 15;
            MkLbl("Service Name:", y); txtName = MkTxt(y += 25); y += 40;
            MkLbl("Category:", y); cboType = MkCbo(y += 25, new[] { "Food & Beverage", "Beauty Care", "Entertainment", "Party Services" }); y += 40;
            MkLbl("Price:", y); txtPrice = MkTxt(y += 25); y += 50;

            var btnSave = new Button { Text = "Save", Size = new Size(120, 35), Location = new Point(80, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += (s, e) => Save();
            var btnCancel = new Button { Text = "Cancel", Size = new Size(120, 35), Location = new Point(210, y), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnCancel.FlatAppearance.BorderSize = 0; btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void Populate() { txtName.Text = _svc!.ServiceName; txtPrice.Text = _svc.Price.ToString(); int idx = cboType.Items.IndexOf(_svc.ServiceType); if (idx >= 0) cboType.SelectedIndex = idx; }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtPrice.Text, out int price)) { MessageBox.Show("Fill all fields."); return; }
            var svc = new Service { ServiceID = _svc?.ServiceID ?? _bus.GenerateID(), ServiceName = txtName.Text.Trim(), ServiceType = cboType.SelectedItem!.ToString()!, Price = price, Image = _svc?.Image };
            bool ok = _svc == null ? _bus.AddService(svc) : _bus.UpdateService(svc);
            if (ok) { DialogResult = DialogResult.OK; Close(); } else MessageBox.Show("Failed.");
        }

        private void MkLbl(string t, int y) { this.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true }); }
        private TextBox MkTxt(int y) { var t = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }
        private ComboBox MkCbo(int y, string[] items) { var c = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList }; c.Items.AddRange(items); c.SelectedIndex = 0; this.Controls.Add(c); return c; }
    }
}
