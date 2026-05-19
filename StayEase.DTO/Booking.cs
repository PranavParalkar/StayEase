namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Booking entity.
    /// ProcessStatus: 0 = Pending, 1 = Processed
    /// </summary>
    public class Booking
    {
        public string BookingID { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public int Deposit { get; set; }
        public int ProcessStatus { get; set; }  // 0 = Pending, 1 = Processed
        public int IsDeleted { get; set; }

        public string StatusDisplay => ProcessStatus == 0 ? "Pending" : "Processed";
    }
}
