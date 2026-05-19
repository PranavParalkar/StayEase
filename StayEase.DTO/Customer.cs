namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Customer entity.
    /// Gender: 0 = Male, 1 = Female
    /// </summary>
    public class Customer
    {
        public string CustomerID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IDCard { get; set; } = string.Empty;
        public short Gender { get; set; }       // 0 = Male, 1 = Female
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Indian";
        public DateTime? DateOfBirth { get; set; }
        public int IsDeleted { get; set; }

        public string GenderDisplay => Gender == 0 ? "Male" : "Female";
    }
}
