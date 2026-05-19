using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmBooking : Form
    {
        private DataGridView dgvBookings = null!, dgvRooms = null!, dgvServices = null!;
        private Button btnCreate = null!, btnCheckIn = null!, btnCheckOut = null!, btnAddService = null!, btnDelete = null!;
        private readonly BookingMgmtService _svc = new BookingMgmtService();
        private readonly Account _account;

        public frmBooking(Account account) { _account = account; InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Booking Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };

            btnCreate = MkBtn("➕ New Booking", 20, Color.FromArgb(34, 197, 94), 140);
            btnCheckIn = MkBtn("📥 Check-In", 170, Color.FromArgb(59, 130, 246), 120);
            btnCheckOut = MkBtn("📤 Check-Out", 300, Color.FromArgb(168, 85, 247), 130);
            btnAddService = MkBtn("🍽 Add Service", 440, Color.FromArgb(245, 158, 11), 130);
            btnDelete = MkBtn("🗑️ Delete", 580, Color.FromArgb(239, 68, 68), 100);

            btnCreate.Click += BtnCreate_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;
            btnAddService.Click += BtnAddService_Click;
            btnDelete.Click += (s, e) => { if (dgvBookings.SelectedRows.Count == 0) return; if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteBooking(dgvBookings.SelectedRows[0].Cells["BookingID"].Value.ToString()!); LoadData(); } };

            dgvBookings = CreateGrid(20, 100, 980, 200);
            dgvBookings.SelectionChanged += DgvBookings_SelectionChanged;

            var lblRooms = new Label { Text = "Booked Rooms", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(20, 310) };
            dgvRooms = CreateGrid(20, 335, 480, 250);

            var lblSvc = new Label { Text = "Booked Services", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(520, 310) };
            dgvServices = CreateGrid(520, 335, 480, 250);

            this.Controls.AddRange(new Control[] { lbl, btnCreate, btnCheckIn, btnCheckOut, btnAddService, btnDelete, dgvBookings, lblRooms, dgvRooms, lblSvc, dgvServices });
        }

        private void LoadData()
        {
            var bookings = _svc.GetAllBookings();
            dgvBookings.DataSource = bookings.Select(b => new { b.BookingID, b.CustomerID, b.EmployeeID, BookingDate = b.BookingDate.ToString("yyyy-MM-dd HH:mm"), b.Deposit, Status = b.StatusDisplay }).ToList();
        }

        private void DgvBookings_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0) return;
            string id = dgvBookings.SelectedRows[0].Cells["BookingID"].Value.ToString()!;
            var rooms = _svc.GetBookingRooms(id);
            dgvRooms.DataSource = rooms.Select(r => new { r.RoomID, CheckIn = r.CheckInDate.ToString("yyyy-MM-dd"), CheckOut = r.CheckOutDate?.ToString("yyyy-MM-dd") ?? "-", r.RentalPrice, Status = r.StatusDisplay }).ToList();
            var svcs = _svc.GetBookingServices(id);
            dgvServices.DataSource = svcs.Select(s => new { s.ServiceID, UsageDate = s.UsageDate.ToString("yyyy-MM-dd"), s.Quantity, s.Price }).ToList();
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            var dlg = new frmBookingCreate(_account, _svc);
            if (dlg.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0) { MessageBox.Show("Select a room."); return; }
            string bookingId = dgvBookings.SelectedRows[0].Cells["BookingID"].Value.ToString()!;
            string roomId = dgvRooms.SelectedRows[0].Cells["RoomID"].Value.ToString()!;
            var rooms = _svc.GetBookingRooms(bookingId);
            var room = rooms.First(r => r.RoomID == roomId);
            if (room.Status != 0) { MessageBox.Show("Room already checked in or out."); return; }
            _svc.CheckIn(bookingId, roomId, room.CheckInDate);
            DgvBookings_SelectionChanged(null, EventArgs.Empty);
            MessageBox.Show("Checked in successfully!", "Success");
        }

        private void BtnCheckOut_Click(object? sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0) { MessageBox.Show("Select a room."); return; }
            string bookingId = dgvBookings.SelectedRows[0].Cells["BookingID"].Value.ToString()!;
            string roomId = dgvRooms.SelectedRows[0].Cells["RoomID"].Value.ToString()!;
            var rooms = _svc.GetBookingRooms(bookingId);
            var room = rooms.First(r => r.RoomID == roomId);
            if (room.Status != 1) { MessageBox.Show("Room must be checked in first."); return; }
            _svc.CheckOut(bookingId, roomId, room.CheckInDate);
            DgvBookings_SelectionChanged(null, EventArgs.Empty);
            MessageBox.Show("Checked out successfully!", "Success");
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0) { MessageBox.Show("Select a booking."); return; }
            string bookingId = dgvBookings.SelectedRows[0].Cells["BookingID"].Value.ToString()!;
            var dlg = new frmAddBookingService(bookingId, _svc);
            if (dlg.ShowDialog() == DialogResult.OK) DgvBookings_SelectionChanged(null, EventArgs.Empty);
        }

        private Button MkBtn(string t, int x, Color c, int w = 100) { var b = new Button { Text = t, Font = new Font("Segoe UI", 10), Size = new Size(w, 35), Location = new Point(x, 55), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; return b; }
        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), Anchor = AnchorStyles.Top | AnchorStyles.Left, BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }

    public class frmBookingCreate : Form
    {
        private ComboBox cboCustomer = null!, cboRoom = null!, cboRentalType = null!;
        private DateTimePicker dtpCheckIn = null!, dtpCheckOut = null!;
        private TextBox txtDeposit = null!, txtPrice = null!;
        private readonly Account _account;
        private readonly BookingMgmtService _svc;
        private readonly CustomerService _custSvc = new CustomerService();
        private readonly RoomService _roomSvc = new RoomService();

        public frmBookingCreate(Account account, BookingMgmtService svc) { _account = account; _svc = svc; InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "Create Booking";
            this.Size = new Size(420, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            int y = 15;
            MkLbl("Customer:", y);
            cboCustomer = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in _custSvc.GetAllCustomers()) cboCustomer.Items.Add($"{c.CustomerID} - {c.FullName}");
            if (cboCustomer.Items.Count > 0) cboCustomer.SelectedIndex = 0;
            this.Controls.Add(cboCustomer); y += 40;

            MkLbl("Room:", y);
            cboRoom = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var r in _roomSvc.GetAvailableRooms()) cboRoom.Items.Add($"{r.RoomID} - {r.RoomName} ({r.RoomTypeDisplay}) ₹{r.Price}");
            if (cboRoom.Items.Count > 0) cboRoom.SelectedIndex = 0;
            this.Controls.Add(cboRoom); y += 40;

            MkLbl("Rental Type:", y);
            cboRentalType = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRentalType.Items.AddRange(new[] { "By Day", "By Hour", "Flexible" });
            cboRentalType.SelectedIndex = 0;
            this.Controls.Add(cboRentalType); y += 40;

            MkLbl("Check-In:", y);
            dtpCheckIn = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };
            this.Controls.Add(dtpCheckIn); y += 35;

            MkLbl("Check-Out:", y);
            dtpCheckOut = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", Value = DateTime.Now.AddDays(1) };
            this.Controls.Add(dtpCheckOut); y += 40;

            MkLbl("Rental Price:", y); txtPrice = MkTxt(y += 25); y += 40;
            MkLbl("Deposit:", y); txtDeposit = MkTxt(y += 25); txtDeposit.Text = "0"; y += 50;

            var btnSave = new Button { Text = "Create Booking", Size = new Size(320, 40), Location = new Point(30, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cboCustomer.SelectedIndex < 0 || cboRoom.SelectedIndex < 0) { MessageBox.Show("Select customer and room."); return; }
            if (!int.TryParse(txtPrice.Text, out int price) || !int.TryParse(txtDeposit.Text, out int deposit)) { MessageBox.Show("Enter valid price/deposit."); return; }

            string custId = cboCustomer.SelectedItem!.ToString()!.Split(" - ")[0];
            string roomId = cboRoom.SelectedItem!.ToString()!.Split(" - ")[0];
            string bookingId = _svc.GenerateBookingID();

            var booking = new Booking { BookingID = bookingId, CustomerID = custId, EmployeeID = _account.EmployeeID, BookingDate = DateTime.Now, Deposit = deposit };
            if (!_svc.CreateBooking(booking)) { MessageBox.Show("Failed to create booking."); return; }

            var br = new BookingRoom { BookingID = bookingId, RoomID = roomId, CheckInDate = dtpCheckIn.Value, CheckOutDate = dtpCheckOut.Value, RentalType = cboRentalType.SelectedIndex, RentalPrice = price };
            _svc.AddBookingRoom(br);

            MessageBox.Show($"Booking {bookingId} created!", "Success");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void MkLbl(string t, int y) { this.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true }); }
        private TextBox MkTxt(int y) { var t = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }
    }

    public class frmAddBookingService : Form
    {
        private ComboBox cboService = null!;
        private TextBox txtQty = null!;
        private readonly string _bookingId;
        private readonly BookingMgmtService _svc;
        private readonly HotelServiceService _hotelSvc = new HotelServiceService();

        public frmAddBookingService(string bookingId, BookingMgmtService svc) { _bookingId = bookingId; _svc = svc; InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "Add Service to Booking";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            int y = 20;
            this.Controls.Add(new Label { Text = "Service:", Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true });
            cboService = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var s in _hotelSvc.GetAllServices()) cboService.Items.Add($"{s.ServiceID} - {s.ServiceName} (₹{s.Price})");
            if (cboService.Items.Count > 0) cboService.SelectedIndex = 0;
            this.Controls.Add(cboService); y += 45;

            this.Controls.Add(new Label { Text = "Quantity:", Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true });
            txtQty = new TextBox { Text = "1", Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtQty); y += 50;

            var btn = new Button { Text = "Add", Size = new Size(320, 35), Location = new Point(30, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                if (cboService.SelectedIndex < 0 || !int.TryParse(txtQty.Text, out int qty)) { MessageBox.Show("Invalid input."); return; }
                string svcId = cboService.SelectedItem!.ToString()!.Split(" - ")[0];
                var allSvcs = _hotelSvc.GetAllServices();
                int price = allSvcs.First(x => x.ServiceID == svcId).Price;
                _svc.AddBookingService(new BookingService { BookingID = _bookingId, ServiceID = svcId, UsageDate = DateTime.Today, Quantity = qty, Price = price });
                MessageBox.Show("Service added!"); DialogResult = DialogResult.OK; Close();
            };
            this.Controls.Add(btn);
        }
    }
}
