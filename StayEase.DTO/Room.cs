namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Room entity.
    /// RoomType: 0 = VIP, 1 = Standard
    /// RoomDetail: 0 = Single, 1 = Double, 2 = Family
    /// Status: 0 = Available, 1 = Occupied, 2 = Not Cleaned, 3 = Under Repair
    /// Condition: 0 = New, 1 = Old
    /// </summary>
    public class Room
    {
        public string RoomID { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public short RoomType { get; set; }     // 0 = VIP, 1 = Standard
        public int Price { get; set; }
        public int RoomDetail { get; set; }     // 0 = Single, 1 = Double, 2 = Family
        public int Status { get; set; }         // 0 = Available, 1 = Occupied, 2 = Not Cleaned, 3 = Under Repair
        public int Condition { get; set; }      // 0 = New, 1 = Old
        public int IsDeleted { get; set; }

        public string RoomTypeDisplay => RoomType == 0 ? "VIP" : "Standard";
        public string RoomDetailDisplay => RoomDetail switch
        {
            0 => "Single",
            1 => "Double",
            2 => "Family",
            _ => "Unknown"
        };
        public string StatusDisplay => Status switch
        {
            0 => "Available",
            1 => "Occupied",
            2 => "Not Cleaned",
            3 => "Under Repair",
            _ => "Unknown"
        };
        public string ConditionDisplay => Condition == 0 ? "New" : "Old";
    }
}
