namespace StayEase.DTO
{
    /// <summary>
    /// Invoice DTO. PaymentMethod: 0 = Cash, 1 = Bank Transfer
    /// </summary>
    public class Invoice
    {
        public string InvoiceID { get; set; } = string.Empty;
        public string BookingID { get; set; } = string.Empty;
        public int Discount { get; set; }
        public int Surcharge { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public short PaymentMethod { get; set; }
        public int IsDeleted { get; set; }

        public string PaymentMethodDisplay => PaymentMethod == 0 ? "Cash" : "Bank Transfer";
    }
}
