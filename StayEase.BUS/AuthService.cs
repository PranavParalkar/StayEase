using System.Security.Cryptography;
using System.Text;
using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Authentication and Account management business logic.
    /// Handles login, password hashing, change password, account CRUD.
    /// </summary>
    public class AuthService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        /// <summary>
        /// Hash password using SHA256
        /// </summary>
        public string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        public Account? Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            string query = $"SELECT * FROM Account WHERE Username = '{username}' AND Password = '{hashedPassword}' AND IsDeleted = 0 AND Status = 1";
            List<Account> accounts = _db.GetListAccount(query);
            return accounts.Count > 0 ? accounts[0] : null;
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Account? account = Login(username, oldPassword);
            if (account == null) return false;

            string hashedNew = HashPassword(newPassword);
            string query = $"UPDATE Account SET Password = '{hashedNew}' WHERE Username = '{username}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        /// <summary>
        /// Get all accounts
        /// </summary>
        public List<Account> GetAllAccounts()
        {
            return _db.GetListAccount("SELECT * FROM Account WHERE IsDeleted = 0");
        }

        /// <summary>
        /// Create new account linked to employee
        /// </summary>
        public bool CreateAccount(string username, string employeeId, string password, string roleId)
        {
            string hashedPwd = HashPassword(password);
            string query = $"INSERT INTO Account (Username, EmployeeID, Password, Status, RoleID, IsDeleted) VALUES ('{username}', '{employeeId}', '{hashedPwd}', 1, '{roleId}', 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        /// <summary>
        /// Soft delete account
        /// </summary>
        public bool DeleteAccount(string username)
        {
            string query = $"UPDATE Account SET IsDeleted = 1 WHERE Username = '{username}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        /// <summary>
        /// Get employee name for logged in account
        /// </summary>
        public string GetEmployeeName(string employeeId)
        {
            return _db.ExecuteScalarString($"SELECT FullName FROM Employee WHERE EmployeeID = '{employeeId}'");
        }

        /// <summary>
        /// Get role name for account
        /// </summary>
        public string GetRoleName(string roleId)
        {
            return _db.ExecuteScalarString($"SELECT RoleName FROM [Role] WHERE RoleID = '{roleId}'");
        }

        /// <summary>
        /// Get permissions for a role
        /// </summary>
        public List<RoleFeature> GetRolePermissions(string roleId)
        {
            return _db.GetListRoleFeature($"SELECT * FROM RoleFeature WHERE RoleID = '{roleId}'");
        }
    }
}
