namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Booking-Room junction entity.
    /// RentalType: 0 = By Day, 1 = By Hour, 2 = Flexible
    /// Status: 0 = Pending, 1 = Checked In, 2 = Checked Out
    /// </summary>
    public class BookingRoom
    {
        public string BookingID { get; set; } = string.Empty;
        public string RoomID { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime? ActualCheckOut { get; set; }
        public int RentalType { get; set; }
        public int RentalPrice { get; set; }
        public int Status { get; set; }

        public string RentalTypeDisplay => RentalType switch
        {
            0 => "By Day", 1 => "By Hour", 2 => "Flexible", _ => "Unknown"
        };
        public string StatusDisplay => Status switch
        {
            0 => "Pending", 1 => "Checked In", 2 => "Checked Out", _ => "Unknown"
        };
    }
}
