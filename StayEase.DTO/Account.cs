namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Account entity.
    /// Linked to Employee and Role.
    /// </summary>
    public class Account
    {
        public string Username { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Status { get; set; } = 1;    // 1 = Active
        public string RoleID { get; set; } = string.Empty;
        public int IsDeleted { get; set; }
    }
}
