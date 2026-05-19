using System.Data;
using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Invoice creation, revenue calculation, statistics.
    /// ID Pattern: HD + date(ddMMyy) + sequential number
    /// </summary>
    public class InvoiceService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Invoice> GetAllInvoices()
        {
            return _db.GetListInvoice("SELECT * FROM Invoice WHERE IsDeleted = 0");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "HD" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Invoice WHERE InvoiceID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool CreateInvoice(Invoice invoice)
        {
            string q = $"INSERT INTO Invoice VALUES ('{invoice.InvoiceID}','{invoice.BookingID}',{invoice.Discount},{invoice.Surcharge},'{invoice.PaymentDate:yyyy-MM-dd HH:mm:ss}',{invoice.PaymentMethod},0)";
            return _db.ExecuteNonQuery(q) > 0;
        }

        public bool DeleteInvoice(string id)
        {
            return _db.ExecuteNonQuery($"UPDATE Invoice SET IsDeleted=1 WHERE InvoiceID='{id}'") > 0;
        }

        public int GetRoomTotal(string bookingId)
        {
            return _db.ExecuteScalar($"SELECT ISNULL(SUM(RentalPrice),0) FROM BookingRoom WHERE BookingID='{bookingId}'");
        }

        public int GetServiceTotal(string bookingId)
        {
            return _db.ExecuteScalar($"SELECT ISNULL(SUM(Price*Quantity),0) FROM BookingService WHERE BookingID='{bookingId}'");
        }

        public DataTable GetInvoiceReport()
        {
            return _db.GetDataTable(@"SELECT i.InvoiceID,i.BookingID,e.FullName AS EmployeeName,
                ISNULL((SELECT SUM(RentalPrice) FROM BookingRoom WHERE BookingID=i.BookingID),0) AS RoomTotal,
                ISNULL((SELECT SUM(Price*Quantity) FROM BookingService WHERE BookingID=i.BookingID),0) AS ServiceTotal,
                i.Discount,i.Surcharge,i.PaymentDate,i.PaymentMethod
                FROM Invoice i JOIN Booking b ON i.BookingID=b.BookingID 
                JOIN Employee e ON b.EmployeeID=e.EmployeeID WHERE i.IsDeleted=0");
        }

        public DataTable GetRevenueByDateRange(DateTime from, DateTime to)
        {
            return _db.GetDataTable($@"SELECT CAST(i.PaymentDate AS DATE) AS PayDate,
                ISNULL(SUM(br.RentalPrice),0) AS RoomRevenue
                FROM Invoice i JOIN Booking b ON i.BookingID=b.BookingID
                LEFT JOIN BookingRoom br ON b.BookingID=br.BookingID
                WHERE i.IsDeleted=0 AND i.PaymentDate BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd} 23:59:59'
                GROUP BY CAST(i.PaymentDate AS DATE) ORDER BY PayDate");
        }

        public DataTable GetRevenueByRoomType()
        {
            return _db.GetDataTable(@"SELECT CASE r.RoomType WHEN 0 THEN 'VIP' ELSE 'Standard' END AS RoomType,
                SUM(br.RentalPrice) AS Revenue FROM BookingRoom br
                JOIN Room r ON br.RoomID=r.RoomID JOIN Booking b ON br.BookingID=b.BookingID
                WHERE b.IsDeleted=0 GROUP BY r.RoomType");
        }
    }
}
