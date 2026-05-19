namespace StayEase.DTO
{
    /// <summary>
    /// Data Transfer Object for Employee entity.
    /// Gender: 0 = Male, 1 = Female
    /// Position: 0 = Manager, 1 = Receptionist
    /// </summary>
    public class Employee
    {
        public string EmployeeID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public short Gender { get; set; }       // 0 = Male, 1 = Female
        public short LeaveDays { get; set; }
        public short Position { get; set; }     // 0 = Manager, 1 = Receptionist
        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }
        public string Email { get; set; } = string.Empty;
        public int DailyWage { get; set; }
        public int IsDeleted { get; set; }

        public string GenderDisplay => Gender == 0 ? "Male" : "Female";
        public string PositionDisplay => Position == 0 ? "Manager" : "Receptionist";
    }
}
