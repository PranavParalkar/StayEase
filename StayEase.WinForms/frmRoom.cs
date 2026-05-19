using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmRoom : Form
    {
        private DataGridView dgvRooms = null!;
        private TextBox txtSearch = null!;
        private Button btnAdd = null!, btnEdit = null!, btnDelete = null!, btnRefresh = null!;
        private ComboBox cboFilterStatus = null!, cboFilterType = null!;
        private readonly RoomService _roomService = new RoomService();

        public frmRoom()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.Padding = new Padding(20);

            var lblTitle = new Label { Text = "Room Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };

            // Search bar
            txtSearch = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(250, 30), Location = new Point(20, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Search rooms..." };
            txtSearch.TextChanged += (s, e) => SearchRooms();

            // Filters
            cboFilterStatus = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(150, 30), Location = new Point(290, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterStatus.Items.AddRange(new object[] { "All Status", "Available", "Occupied", "Not Cleaned", "Under Repair" });
            cboFilterStatus.SelectedIndex = 0;
            cboFilterStatus.SelectedIndexChanged += (s, e) => FilterData();

            cboFilterType = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(130, 30), Location = new Point(460, 55), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterType.Items.AddRange(new object[] { "All Types", "VIP", "Standard" });
            cboFilterType.SelectedIndex = 0;
            cboFilterType.SelectedIndexChanged += (s, e) => FilterData();

            // Buttons
            btnAdd = CreateButton("➕ Add", 620, Color.FromArgb(34, 197, 94));
            btnEdit = CreateButton("✏️ Edit", 730, Color.FromArgb(59, 130, 246));
            btnDelete = CreateButton("🗑️ Delete", 840, Color.FromArgb(239, 68, 68));
            btnRefresh = CreateButton("🔄", 950, Color.FromArgb(100, 116, 139));
            btnRefresh.Size = new Size(40, 35);

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadData();

            // DataGridView
            dgvRooms = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(980, 500),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(51, 65, 85),
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgvRooms.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvRooms.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvRooms.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85);
            dgvRooms.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225);
            dgvRooms.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRooms.EnableHeadersVisualStyles = false;

            this.Controls.AddRange(new Control[] { lblTitle, txtSearch, cboFilterStatus, cboFilterType, btnAdd, btnEdit, btnDelete, btnRefresh, dgvRooms });
        }

        private Button CreateButton(string text, int x, Color color)
        {
            var btn = new Button { Text = text, Font = new Font("Segoe UI", 10), Size = new Size(100, 35), Location = new Point(x, 55), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData()
        {
            var rooms = _roomService.GetAllRooms();
            dgvRooms.DataSource = rooms.Select(r => new { r.RoomID, r.RoomName, Type = r.RoomTypeDisplay, Detail = r.RoomDetailDisplay, r.Price, Status = r.StatusDisplay, Condition = r.ConditionDisplay }).ToList();
        }

        private void SearchRooms()
        {
            string kw = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(kw)) { LoadData(); return; }
            var rooms = _roomService.SearchRooms(kw);
            dgvRooms.DataSource = rooms.Select(r => new { r.RoomID, r.RoomName, Type = r.RoomTypeDisplay, Detail = r.RoomDetailDisplay, r.Price, Status = r.StatusDisplay, Condition = r.ConditionDisplay }).ToList();
        }

        private void FilterData()
        {
            var rooms = _roomService.GetAllRooms();
            if (cboFilterStatus.SelectedIndex > 0)
                rooms = rooms.Where(r => r.Status == cboFilterStatus.SelectedIndex - 1).ToList();
            if (cboFilterType.SelectedIndex > 0)
                rooms = rooms.Where(r => r.RoomType == cboFilterType.SelectedIndex - 1).ToList();
            dgvRooms.DataSource = rooms.Select(r => new { r.RoomID, r.RoomName, Type = r.RoomTypeDisplay, Detail = r.RoomDetailDisplay, r.Price, Status = r.StatusDisplay, Condition = r.ConditionDisplay }).ToList();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new frmRoomEdit(null);
            if (dlg.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0) { MessageBox.Show("Select a room to edit."); return; }
            string id = dgvRooms.SelectedRows[0].Cells["RoomID"].Value.ToString()!;
            var room = _roomService.GetAllRooms().First(r => r.RoomID == id);
            var dlg = new frmRoomEdit(room);
            if (dlg.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0) { MessageBox.Show("Select a room to delete."); return; }
            string id = dgvRooms.SelectedRows[0].Cells["RoomID"].Value.ToString()!;
            if (MessageBox.Show($"Delete room {id}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _roomService.DeleteRoom(id);
                LoadData();
            }
        }
    }

    // ===== ROOM ADD/EDIT DIALOG =====
    public class frmRoomEdit : Form
    {
        private TextBox txtName = null!, txtPrice = null!;
        private ComboBox cboType = null!, cboDetail = null!, cboStatus = null!, cboCondition = null!;
        private Button btnSave = null!, btnCancel = null!;
        private readonly Room? _room;
        private readonly RoomService _svc = new RoomService();

        public frmRoomEdit(Room? room)
        {
            _room = room;
            InitializeComponent();
            if (_room != null) PopulateFields();
        }

        private void InitializeComponent()
        {
            this.Text = _room == null ? "Add Room" : "Edit Room";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 15;
            AddLabel("Room Name:", y); txtName = AddTextBox(y += 25); y += 40;
            AddLabel("Type:", y); cboType = AddCombo(y += 25, new[] { "VIP", "Standard" }); y += 40;
            AddLabel("Detail:", y); cboDetail = AddCombo(y += 25, new[] { "Single", "Double", "Family" }); y += 40;
            AddLabel("Price:", y); txtPrice = AddTextBox(y += 25); y += 40;
            AddLabel("Status:", y); cboStatus = AddCombo(y += 25, new[] { "Available", "Occupied", "Not Cleaned", "Under Repair" }); y += 40;
            AddLabel("Condition:", y); cboCondition = AddCombo(y += 25, new[] { "New", "Old" }); y += 45;

            btnSave = new Button { Text = "Save", Size = new Size(120, 35), Location = new Point(80, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnCancel = new Button { Text = "Cancel", Size = new Size(120, 35), Location = new Point(210, y), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private Label AddLabel(string text, int y)
        {
            var lbl = new Label { Text = text, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true };
            this.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(int y)
        {
            var txt = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txt);
            return txt;
        }

        private ComboBox AddCombo(int y, string[] items)
        {
            var cbo = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            this.Controls.Add(cbo);
            return cbo;
        }

        private void PopulateFields()
        {
            txtName.Text = _room!.RoomName;
            cboType.SelectedIndex = _room.RoomType;
            cboDetail.SelectedIndex = _room.RoomDetail;
            txtPrice.Text = _room.Price.ToString();
            cboStatus.SelectedIndex = _room.Status;
            cboCondition.SelectedIndex = _room.Condition;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtPrice.Text, out int price))
            { MessageBox.Show("Please fill all fields correctly."); return; }

            var room = new Room
            {
                RoomID = _room?.RoomID ?? _svc.GenerateID(),
                RoomName = txtName.Text.Trim(),
                RoomType = (short)cboType.SelectedIndex,
                RoomDetail = cboDetail.SelectedIndex,
                Price = price,
                Status = cboStatus.SelectedIndex,
                Condition = cboCondition.SelectedIndex
            };

            bool ok = _room == null ? _svc.AddRoom(room) : _svc.UpdateRoom(room);
            if (ok) { this.DialogResult = DialogResult.OK; this.Close(); }
            else MessageBox.Show("Operation failed.");
        }
    }
}
