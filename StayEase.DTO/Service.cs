namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Service entity.
    /// ServiceType categories: Food & Beverage, Beauty Care, Entertainment, Party Services
    /// </summary>
    public class Service
    {
        public string ServiceID { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public int Price { get; set; }
        public string? Image { get; set; }      // Base64-encoded image
        public int IsDeleted { get; set; }
    }
}
