using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Customer CRUD, search, history operations.
    /// ID Pattern: KH + date(ddMMyy) + sequential number
    /// </summary>
    public class CustomerService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Customer> GetAllCustomers()
        {
            return _db.GetListCustomer("SELECT * FROM Customer WHERE IsDeleted = 0");
        }

        public List<Customer> SearchCustomers(string keyword)
        {
            return _db.GetListCustomer($"SELECT * FROM Customer WHERE IsDeleted = 0 AND (FullName LIKE N'%{keyword}%' OR IDCard LIKE '%{keyword}%' OR Phone LIKE '%{keyword}%')");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "KH" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Customer WHERE CustomerID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool AddCustomer(Customer customer)
        {
            string dob = customer.DateOfBirth?.ToString("yyyy-MM-dd") ?? "NULL";
            string dobValue = customer.DateOfBirth.HasValue ? $"'{dob}'" : "NULL";
            string query = $"INSERT INTO Customer (CustomerID, FullName, IDCard, Gender, Phone, Address, Nationality, DateOfBirth, IsDeleted) VALUES ('{customer.CustomerID}', N'{customer.FullName}', '{customer.IDCard}', {customer.Gender}, '{customer.Phone}', N'{customer.Address}', N'{customer.Nationality}', {dobValue}, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateCustomer(Customer customer)
        {
            string dob = customer.DateOfBirth?.ToString("yyyy-MM-dd") ?? "NULL";
            string dobValue = customer.DateOfBirth.HasValue ? $"'{dob}'" : "NULL";
            string query = $"UPDATE Customer SET FullName = N'{customer.FullName}', IDCard = '{customer.IDCard}', Gender = {customer.Gender}, Phone = '{customer.Phone}', Address = N'{customer.Address}', Nationality = N'{customer.Nationality}', DateOfBirth = {dobValue} WHERE CustomerID = '{customer.CustomerID}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool DeleteCustomer(string customerId)
        {
            return _db.ExecuteNonQuery($"UPDATE Customer SET IsDeleted = 1 WHERE CustomerID = '{customerId}'") > 0;
        }
    }
}
