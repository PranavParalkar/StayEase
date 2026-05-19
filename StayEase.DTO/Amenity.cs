namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Amenity entity.
    /// Examples: TV, Iron, Hair Dryer, Air Conditioner
    /// </summary>
    public class Amenity
    {
        public string AmenityID { get; set; } = string.Empty;
        public string AmenityName { get; set; } = string.Empty;
        public int IsDeleted { get; set; }
    }
}
