using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmRole : Form
    {
        private DataGridView dgvRoles = null!;
        private CheckedListBox clbFeatures = null!;
        private Button btnAdd = null!, btnDelete = null!, btnSave = null!;
        private TextBox txtRoleName = null!;
        private readonly RoleService _svc = new RoleService();

        public frmRole() { InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Role & Permission Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };

            dgvRoles = CreateGrid(20, 55, 450, 250);
            dgvRoles.SelectionChanged += DgvRoles_SelectionChanged;

            var lblPerm = new Label { Text = "Assign Permissions", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(500, 55) };

            clbFeatures = new CheckedListBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(460, 250),
                Location = new Point(500, 85),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };

            var features = _svc.GetAllFeatures();
            foreach (var f in features)
                clbFeatures.Items.Add($"{f.FeatureID} - {f.FeatureName}");

            btnSave = new Button { Text = "💾 Save Permissions", Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(200, 35), Location = new Point(500, 345), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSavePerms_Click;

            // Add Role Section
            var lblNew = new Label { Text = "Add New Role", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(20, 325) };
            txtRoleName = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(250, 30), Location = new Point(20, 355), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Role name..." };

            btnAdd = new Button { Text = "➕ Add", Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(280, 355), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtRoleName.Text)) return;
                _svc.AddRole(new Role { RoleID = _svc.GenerateID(), RoleName = txtRoleName.Text.Trim() });
                txtRoleName.Clear(); LoadData();
            };

            btnDelete = new Button { Text = "🗑️ Delete", Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(390, 355), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) =>
            {
                if (dgvRoles.SelectedRows.Count == 0) return;
                string id = dgvRoles.SelectedRows[0].Cells["RoleID"].Value.ToString()!;
                if (id == "R001") { MessageBox.Show("Cannot delete Admin role."); return; }
                if (MessageBox.Show("Delete this role?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteRole(id); LoadData(); }
            };

            this.Controls.AddRange(new Control[] { lbl, dgvRoles, lblPerm, clbFeatures, btnSave, lblNew, txtRoleName, btnAdd, btnDelete });
        }

        private void LoadData()
        {
            var roles = _svc.GetAllRoles();
            dgvRoles.DataSource = roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
        }

        private void DgvRoles_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0) return;
            string roleId = dgvRoles.SelectedRows[0].Cells["RoleID"].Value.ToString()!;
            var perms = _svc.GetRoleFeatures(roleId);
            var permIds = perms.Select(p => p.FeatureID).ToHashSet();
            for (int i = 0; i < clbFeatures.Items.Count; i++)
            {
                string fid = clbFeatures.Items[i].ToString()!.Split(" - ")[0];
                clbFeatures.SetItemChecked(i, permIds.Contains(fid));
            }
        }

        private void BtnSavePerms_Click(object? sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0) return;
            string roleId = dgvRoles.SelectedRows[0].Cells["RoleID"].Value.ToString()!;
            var featureIds = new List<string>();
            for (int i = 0; i < clbFeatures.Items.Count; i++)
                if (clbFeatures.GetItemChecked(i))
                    featureIds.Add(clbFeatures.Items[i].ToString()!.Split(" - ")[0]);
            _svc.SetRoleFeatures(roleId, featureIds);
            MessageBox.Show("Permissions saved!", "Success");
        }

        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }
}
