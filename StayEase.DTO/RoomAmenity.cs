namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Room-Amenity junction table.
    /// Maps amenities to rooms with quantity.
    /// </summary>
    public class RoomAmenity
    {
        public string RoomID { get; set; } = string.Empty;
        public string AmenityID { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }
}
