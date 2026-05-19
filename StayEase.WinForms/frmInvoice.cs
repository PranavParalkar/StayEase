using StayEase.BUS;
using StayEase.DTO;

namespace StayEase.WinForms
{
    public class frmInvoice : Form
    {
        private DataGridView dgv = null!;
        private Button btnCreate = null!, btnDelete = null!;
        private readonly InvoiceService _svc = new InvoiceService();
        private readonly BookingMgmtService _bookingSvc = new BookingMgmtService();

        public frmInvoice() { InitializeComponent(); LoadData(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Invoice Management", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };

            btnCreate = MkBtn("➕ Create Invoice", 20, Color.FromArgb(34, 197, 94), 160);
            btnDelete = MkBtn("🗑️ Delete", 190, Color.FromArgb(239, 68, 68), 100);

            btnCreate.Click += BtnCreate_Click;
            btnDelete.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) return; if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _svc.DeleteInvoice(dgv.SelectedRows[0].Cells["InvoiceID"].Value.ToString()!); LoadData(); } };

            dgv = CreateGrid(20, 100, 980, 500);
            this.Controls.AddRange(new Control[] { lbl, btnCreate, btnDelete, dgv });
        }

        private void LoadData()
        {
            var dt = _svc.GetInvoiceReport();
            dgv.DataSource = dt;
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            var dlg = new frmInvoiceCreate(_svc, _bookingSvc);
            if (dlg.ShowDialog() == DialogResult.OK) LoadData();
        }

        private Button MkBtn(string t, int x, Color c, int w = 100) { var b = new Button { Text = t, Font = new Font("Segoe UI", 10), Size = new Size(w, 35), Location = new Point(x, 55), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; return b; }
        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }

    public class frmInvoiceCreate : Form
    {
        private ComboBox cboBooking = null!;
        private TextBox txtDiscount = null!, txtSurcharge = null!;
        private ComboBox cboPayment = null!;
        private Label lblRoomTotal = null!, lblServiceTotal = null!, lblGrandTotal = null!;
        private readonly InvoiceService _svc;
        private readonly BookingMgmtService _bookingSvc;

        public frmInvoiceCreate(InvoiceService svc, BookingMgmtService bookingSvc) { _svc = svc; _bookingSvc = bookingSvc; InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "Create Invoice";
            this.Size = new Size(420, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 41, 59);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            int y = 15;
            MkLbl("Booking:", y);
            cboBooking = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var b in _bookingSvc.GetAllBookings()) cboBooking.Items.Add($"{b.BookingID} - {b.CustomerID}");
            if (cboBooking.Items.Count > 0) cboBooking.SelectedIndex = 0;
            cboBooking.SelectedIndexChanged += (s, e) => CalcTotals();
            this.Controls.Add(cboBooking); y += 45;

            lblRoomTotal = new Label { Text = "Room Total: ₹0", Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(34, 197, 94), Location = new Point(30, y), AutoSize = true }; this.Controls.Add(lblRoomTotal); y += 25;
            lblServiceTotal = new Label { Text = "Service Total: ₹0", Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(56, 189, 248), Location = new Point(30, y), AutoSize = true }; this.Controls.Add(lblServiceTotal); y += 35;

            MkLbl("Discount %:", y); txtDiscount = MkTxt(y += 25); txtDiscount.Text = "0"; txtDiscount.TextChanged += (s, e) => CalcTotals(); y += 40;
            MkLbl("Surcharge %:", y); txtSurcharge = MkTxt(y += 25); txtSurcharge.Text = "0"; txtSurcharge.TextChanged += (s, e) => CalcTotals(); y += 40;
            MkLbl("Payment:", y);
            cboPayment = new ComboBox { Font = new Font("Segoe UI", 10), Size = new Size(320, 30), Location = new Point(30, y += 25), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPayment.Items.AddRange(new[] { "Cash", "Bank Transfer" });
            cboPayment.SelectedIndex = 0;
            this.Controls.Add(cboPayment); y += 40;

            lblGrandTotal = new Label { Text = "Grand Total: ₹0", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(250, 204, 21), Location = new Point(30, y), AutoSize = true }; this.Controls.Add(lblGrandTotal); y += 45;

            var btn = new Button { Text = "Create Invoice", Size = new Size(320, 40), Location = new Point(30, y), BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                if (cboBooking.SelectedIndex < 0) return;
                string bId = cboBooking.SelectedItem!.ToString()!.Split(" - ")[0];
                int.TryParse(txtDiscount.Text, out int disc);
                int.TryParse(txtSurcharge.Text, out int sur);
                var inv = new Invoice { InvoiceID = _svc.GenerateID(), BookingID = bId, Discount = disc, Surcharge = sur, PaymentDate = DateTime.Now, PaymentMethod = (short)cboPayment.SelectedIndex };
                if (_svc.CreateInvoice(inv)) { _bookingSvc.UpdateBookingStatus(bId, 1); MessageBox.Show("Invoice created!"); DialogResult = DialogResult.OK; Close(); }
            };
            this.Controls.Add(btn);

            if (cboBooking.Items.Count > 0) CalcTotals();
        }

        private void CalcTotals()
        {
            if (cboBooking.SelectedIndex < 0) return;
            string bId = cboBooking.SelectedItem!.ToString()!.Split(" - ")[0];
            int roomT = _svc.GetRoomTotal(bId);
            int svcT = _svc.GetServiceTotal(bId);
            lblRoomTotal.Text = $"Room Total: ₹{roomT:N0}";
            lblServiceTotal.Text = $"Service Total: ₹{svcT:N0}";
            int.TryParse(txtDiscount.Text, out int disc);
            int.TryParse(txtSurcharge.Text, out int sur);
            int subtotal = roomT + svcT;
            int grand = subtotal + (subtotal * sur / 100) - (subtotal * disc / 100);
            lblGrandTotal.Text = $"Grand Total: ₹{grand:N0}";
        }

        private void MkLbl(string t, int y) { this.Controls.Add(new Label { Text = t, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(30, y), AutoSize = true }); }
        private TextBox MkTxt(int y) { var t = new TextBox { Font = new Font("Segoe UI", 11), Size = new Size(320, 30), Location = new Point(30, y), BackColor = Color.FromArgb(51, 65, 85), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }
    }
}
