using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Employee CRUD, search, Excel import/export support.
    /// ID Pattern: NV + date(ddMMyy) + sequential number
    /// </summary>
    public class EmployeeService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Employee> GetAllEmployees()
        {
            return _db.GetListEmployee("SELECT * FROM Employee WHERE IsDeleted = 0");
        }

        public List<Employee> SearchEmployees(string keyword)
        {
            return _db.GetListEmployee($"SELECT * FROM Employee WHERE IsDeleted = 0 AND (FullName LIKE N'%{keyword}%' OR EmployeeID LIKE '%{keyword}%')");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "NV" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Employee WHERE EmployeeID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool AddEmployee(Employee emp)
        {
            string dob = emp.DateOfBirth?.ToString("yyyy-MM-dd") ?? "NULL";
            string dobVal = emp.DateOfBirth.HasValue ? $"'{dob}'" : "NULL";
            string hire = emp.HireDate?.ToString("yyyy-MM-dd") ?? "NULL";
            string hireVal = emp.HireDate.HasValue ? $"'{hire}'" : "NULL";
            string query = $"INSERT INTO Employee (EmployeeID, FullName, Gender, LeaveDays, Position, DateOfBirth, HireDate, Email, DailyWage, IsDeleted) VALUES ('{emp.EmployeeID}', N'{emp.FullName}', {emp.Gender}, {emp.LeaveDays}, {emp.Position}, {dobVal}, {hireVal}, '{emp.Email}', {emp.DailyWage}, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateEmployee(Employee emp)
        {
            string dob = emp.DateOfBirth?.ToString("yyyy-MM-dd") ?? "NULL";
            string dobVal = emp.DateOfBirth.HasValue ? $"'{dob}'" : "NULL";
            string hire = emp.HireDate?.ToString("yyyy-MM-dd") ?? "NULL";
            string hireVal = emp.HireDate.HasValue ? $"'{hire}'" : "NULL";
            string query = $"UPDATE Employee SET FullName = N'{emp.FullName}', Gender = {emp.Gender}, LeaveDays = {emp.LeaveDays}, Position = {emp.Position}, DateOfBirth = {dobVal}, HireDate = {hireVal}, Email = '{emp.Email}', DailyWage = {emp.DailyWage} WHERE EmployeeID = '{emp.EmployeeID}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool DeleteEmployee(string employeeId)
        {
            return _db.ExecuteNonQuery($"UPDATE Employee SET IsDeleted = 1 WHERE EmployeeID = '{employeeId}'") > 0;
        }
    }
}
