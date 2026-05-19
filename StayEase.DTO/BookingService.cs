namespace StayEase.DTO
{
    public class BookingService
    {
        public string BookingID { get; set; } = string.Empty;
        public string ServiceID { get; set; } = string.Empty;
        public DateTime UsageDate { get; set; }
        public int Quantity { get; set; } = 1;
        public int Price { get; set; }
    }
}
