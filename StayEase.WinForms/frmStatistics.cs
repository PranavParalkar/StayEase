using StayEase.BUS;

namespace StayEase.WinForms
{
    public class frmStatistics : Form
    {
        private DateTimePicker dtpFrom = null!, dtpTo = null!;
        private Button btnFilter = null!;
        private DataGridView dgv = null!;
        private Label lblTotal = null!;
        private readonly InvoiceService _svc = new InvoiceService();

        public frmStatistics() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            var lbl = new Label { Text = "Statistics & Reports", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };

            var lblFrom = new Label { Text = "From:", Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(20, 60), AutoSize = true };
            dtpFrom = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(180, 30), Location = new Point(70, 57), Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddMonths(-1) };

            var lblTo = new Label { Text = "To:", Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(270, 60), AutoSize = true };
            dtpTo = new DateTimePicker { Font = new Font("Segoe UI", 10), Size = new Size(180, 30), Location = new Point(300, 57), Format = DateTimePickerFormat.Short };

            btnFilter = new Button { Text = "📊 Generate Report", Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(180, 35), Location = new Point(500, 55), BackColor = Color.FromArgb(56, 189, 248), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += (s, e) => LoadReport();

            // Revenue by Room Type
            var lblType = new Label { Text = "Revenue by Room Type", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(20, 105) };

            dgv = CreateGrid(20, 135, 980, 200);

            // Daily Revenue
            var lblDaily = new Label { Text = "Daily Revenue", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(56, 189, 248), AutoSize = true, Location = new Point(20, 350) };

            lblTotal = new Label { Text = "", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(250, 204, 21), AutoSize = true, Location = new Point(20, 560) };

            this.Controls.AddRange(new Control[] { lbl, lblFrom, dtpFrom, lblTo, dtpTo, btnFilter, lblType, dgv, lblDaily, lblTotal });
        }

        private void LoadReport()
        {
            var dtType = _svc.GetRevenueByRoomType();
            dgv.DataSource = dtType;

            var dtDaily = _svc.GetRevenueByDateRange(dtpFrom.Value, dtpTo.Value);
            int totalRevenue = 0;
            foreach (System.Data.DataRow row in dtDaily.Rows)
                totalRevenue += Convert.ToInt32(row["RoomRevenue"]);
            lblTotal.Text = $"Total Revenue (Selected Period): ₹{totalRevenue:N0}";
        }

        private DataGridView CreateGrid(int x, int y, int w, int h) { var g = new DataGridView { Location = new Point(x, y), Size = new Size(w, h), BackgroundColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, GridColor = Color.FromArgb(51, 65, 85), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false }; g.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59); g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248); g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225); g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); g.EnableHeadersVisualStyles = false; return g; }
    }
}
