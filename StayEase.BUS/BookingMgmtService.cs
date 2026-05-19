using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Booking master record management, room booking, service booking.
    /// ID Pattern: CTT + date(ddMMyy) + sequential number
    /// </summary>
    public class BookingMgmtService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        // ===== BOOKING MASTER =====
        public List<Booking> GetAllBookings()
        {
            return _db.GetListBooking("SELECT * FROM Booking WHERE IsDeleted = 0");
        }

        public List<Booking> GetPendingBookings()
        {
            return _db.GetListBooking("SELECT * FROM Booking WHERE IsDeleted = 0 AND ProcessStatus = 0");
        }

        public string GenerateBookingID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "CTT" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Booking WHERE BookingID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool CreateBooking(Booking booking)
        {
            string query = $"INSERT INTO Booking (BookingID, CustomerID, EmployeeID, BookingDate, Deposit, ProcessStatus, IsDeleted) VALUES ('{booking.BookingID}', '{booking.CustomerID}', '{booking.EmployeeID}', '{booking.BookingDate:yyyy-MM-dd HH:mm:ss}', {booking.Deposit}, 0, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateBookingStatus(string bookingId, int status)
        {
            return _db.ExecuteNonQuery($"UPDATE Booking SET ProcessStatus = {status} WHERE BookingID = '{bookingId}'") > 0;
        }

        public bool DeleteBooking(string bookingId)
        {
            return _db.ExecuteNonQuery($"UPDATE Booking SET IsDeleted = 1 WHERE BookingID = '{bookingId}'") > 0;
        }

        // ===== BOOKING ROOMS =====
        public List<BookingRoom> GetBookingRooms(string bookingId)
        {
            return _db.GetListBookingRoom($"SELECT * FROM BookingRoom WHERE BookingID = '{bookingId}'");
        }

        public bool AddBookingRoom(BookingRoom br)
        {
            string checkOut = br.CheckOutDate.HasValue ? $"'{br.CheckOutDate:yyyy-MM-dd HH:mm:ss}'" : "NULL";
            string query = $"INSERT INTO BookingRoom (BookingID, RoomID, CheckInDate, CheckOutDate, ActualCheckOut, RentalType, RentalPrice, Status) VALUES ('{br.BookingID}', '{br.RoomID}', '{br.CheckInDate:yyyy-MM-dd HH:mm:ss}', {checkOut}, NULL, {br.RentalType}, {br.RentalPrice}, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool CheckIn(string bookingId, string roomId, DateTime checkInDate)
        {
            string query = $"UPDATE BookingRoom SET Status = 1 WHERE BookingID = '{bookingId}' AND RoomID = '{roomId}' AND CheckInDate = '{checkInDate:yyyy-MM-dd HH:mm:ss}'";
            _db.ExecuteNonQuery($"UPDATE Room SET Status = 1 WHERE RoomID = '{roomId}'");
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool CheckOut(string bookingId, string roomId, DateTime checkInDate)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string query = $"UPDATE BookingRoom SET Status = 2, ActualCheckOut = '{now}' WHERE BookingID = '{bookingId}' AND RoomID = '{roomId}' AND CheckInDate = '{checkInDate:yyyy-MM-dd HH:mm:ss}'";
            _db.ExecuteNonQuery($"UPDATE Room SET Status = 2 WHERE RoomID = '{roomId}'");
            return _db.ExecuteNonQuery(query) > 0;
        }

        // ===== BOOKING SERVICES =====
        public List<BookingService> GetBookingServices(string bookingId)
        {
            return _db.GetListBookingService($"SELECT * FROM BookingService WHERE BookingID = '{bookingId}'");
        }

        public bool AddBookingService(BookingService bs)
        {
            string query = $"INSERT INTO BookingService (BookingID, ServiceID, UsageDate, Quantity, Price) VALUES ('{bs.BookingID}', '{bs.ServiceID}', '{bs.UsageDate:yyyy-MM-dd}', {bs.Quantity}, {bs.Price})";
            return _db.ExecuteNonQuery(query) > 0;
        }
    }
}
