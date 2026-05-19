using System.Data;
using Microsoft.Data.SqlClient;
using StayEase.DTO;

namespace StayEase.DAO
{
    /// <summary>
    /// Central Data Access class using ADO.NET with SqlConnection.
    /// Provides typed list returns for each entity and generic query methods.
    /// </summary>
    public class DatabaseHelper
    {
        private static DatabaseHelper? _instance;
        private readonly string _connectionString;

        private DatabaseHelper()
        {
            _connectionString = "Server=localhost;Database=StayEase;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public static DatabaseHelper Instance
        {
            get
            {
                _instance ??= new DatabaseHelper();
                return _instance;
            }
        }

        public static void SetConnectionString(string connectionString)
        {
            _instance = null;
            _instance = new DatabaseHelper();
            var field = typeof(DatabaseHelper).GetField("_connectionString",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_instance, connectionString);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ============ GENERIC METHODS ============

        /// <summary>
        /// Execute SELECT query and return DataTable
        /// </summary>
        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Execute INSERT, UPDATE, DELETE queries
        /// </summary>
        public int ExecuteNonQuery(string query)
        {
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute scalar query returning integer (COUNT, MAX, etc.)
        /// </summary>
        public int ExecuteScalar(string query)
        {
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Execute scalar query returning string
        /// </summary>
        public string ExecuteScalarString(string query)
        {
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            object? result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value ? result.ToString()! : string.Empty;
        }

        /// <summary>
        /// Test database connectivity
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using SqlConnection conn = GetConnection();
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ============ TYPED LIST METHODS ============

        public List<Room> GetListRoom(string query)
        {
            List<Room> list = new List<Room>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Room
                {
                    RoomID = reader["RoomID"].ToString()!,
                    RoomName = reader["RoomName"].ToString()!,
                    RoomType = Convert.ToInt16(reader["RoomType"]),
                    Price = Convert.ToInt32(reader["Price"]),
                    RoomDetail = Convert.ToInt32(reader["RoomDetail"]),
                    Status = Convert.ToInt32(reader["Status"]),
                    Condition = Convert.ToInt32(reader["Condition"]),
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Customer> GetListCustomer(string query)
        {
            List<Customer> list = new List<Customer>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Customer
                {
                    CustomerID = reader["CustomerID"].ToString()!,
                    FullName = reader["FullName"].ToString()!,
                    IDCard = reader["IDCard"].ToString()!,
                    Gender = Convert.ToInt16(reader["Gender"]),
                    Phone = reader["Phone"].ToString()!,
                    Address = reader["Address"].ToString()!,
                    Nationality = reader["Nationality"].ToString()!,
                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : null,
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Employee> GetListEmployee(string query)
        {
            List<Employee> list = new List<Employee>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Employee
                {
                    EmployeeID = reader["EmployeeID"].ToString()!,
                    FullName = reader["FullName"].ToString()!,
                    Gender = Convert.ToInt16(reader["Gender"]),
                    LeaveDays = Convert.ToInt16(reader["LeaveDays"]),
                    Position = Convert.ToInt16(reader["Position"]),
                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : null,
                    HireDate = reader["HireDate"] != DBNull.Value ? Convert.ToDateTime(reader["HireDate"]) : null,
                    Email = reader["Email"].ToString()!,
                    DailyWage = Convert.ToInt32(reader["DailyWage"]),
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Account> GetListAccount(string query)
        {
            List<Account> list = new List<Account>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Account
                {
                    Username = reader["Username"].ToString()!,
                    EmployeeID = reader["EmployeeID"].ToString()!,
                    Password = reader["Password"].ToString()!,
                    Status = Convert.ToInt32(reader["Status"]),
                    RoleID = reader["RoleID"].ToString()!,
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Service> GetListService(string query)
        {
            List<Service> list = new List<Service>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Service
                {
                    ServiceID = reader["ServiceID"].ToString()!,
                    ServiceName = reader["ServiceName"].ToString()!,
                    ServiceType = reader["ServiceType"].ToString()!,
                    Price = Convert.ToInt32(reader["Price"]),
                    Image = reader["Image"] != DBNull.Value ? reader["Image"].ToString() : null,
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Amenity> GetListAmenity(string query)
        {
            List<Amenity> list = new List<Amenity>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Amenity
                {
                    AmenityID = reader["AmenityID"].ToString()!,
                    AmenityName = reader["AmenityName"].ToString()!,
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Booking> GetListBooking(string query)
        {
            List<Booking> list = new List<Booking>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Booking
                {
                    BookingID = reader["BookingID"].ToString()!,
                    CustomerID = reader["CustomerID"].ToString()!,
                    EmployeeID = reader["EmployeeID"].ToString()!,
                    BookingDate = Convert.ToDateTime(reader["BookingDate"]),
                    Deposit = Convert.ToInt32(reader["Deposit"]),
                    ProcessStatus = Convert.ToInt32(reader["ProcessStatus"]),
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<BookingRoom> GetListBookingRoom(string query)
        {
            List<BookingRoom> list = new List<BookingRoom>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BookingRoom
                {
                    BookingID = reader["BookingID"].ToString()!,
                    RoomID = reader["RoomID"].ToString()!,
                    CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                    CheckOutDate = reader["CheckOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["CheckOutDate"]) : null,
                    ActualCheckOut = reader["ActualCheckOut"] != DBNull.Value ? Convert.ToDateTime(reader["ActualCheckOut"]) : null,
                    RentalType = Convert.ToInt32(reader["RentalType"]),
                    RentalPrice = Convert.ToInt32(reader["RentalPrice"]),
                    Status = Convert.ToInt32(reader["Status"])
                });
            }
            return list;
        }

        public List<BookingService> GetListBookingService(string query)
        {
            List<BookingService> list = new List<BookingService>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BookingService
                {
                    BookingID = reader["BookingID"].ToString()!,
                    ServiceID = reader["ServiceID"].ToString()!,
                    UsageDate = Convert.ToDateTime(reader["UsageDate"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Price = Convert.ToInt32(reader["Price"])
                });
            }
            return list;
        }

        public List<Invoice> GetListInvoice(string query)
        {
            List<Invoice> list = new List<Invoice>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Invoice
                {
                    InvoiceID = reader["InvoiceID"].ToString()!,
                    BookingID = reader["BookingID"].ToString()!,
                    Discount = Convert.ToInt32(reader["Discount"]),
                    Surcharge = Convert.ToInt32(reader["Surcharge"]),
                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                    PaymentMethod = Convert.ToInt16(reader["PaymentMethod"]),
                    IsDeleted = Convert.ToInt32(reader["IsDeleted"])
                });
            }
            return list;
        }

        public List<Role> GetListRole(string query)
        {
            List<Role> list = new List<Role>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Role
                {
                    RoleID = reader["RoleID"].ToString()!,
                    RoleName = reader["RoleName"].ToString()!
                });
            }
            return list;
        }

        public List<Feature> GetListFeature(string query)
        {
            List<Feature> list = new List<Feature>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Feature
                {
                    FeatureID = reader["FeatureID"].ToString()!,
                    FeatureName = reader["FeatureName"].ToString()!
                });
            }
            return list;
        }

        public List<RoleFeature> GetListRoleFeature(string query)
        {
            List<RoleFeature> list = new List<RoleFeature>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new RoleFeature
                {
                    RoleID = reader["RoleID"].ToString()!,
                    FeatureID = reader["FeatureID"].ToString()!
                });
            }
            return list;
        }

        public List<RoomAmenity> GetListRoomAmenity(string query)
        {
            List<RoomAmenity> list = new List<RoomAmenity>();
            using SqlConnection conn = GetConnection();
            conn.Open();
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new RoomAmenity
                {
                    RoomID = reader["RoomID"].ToString()!,
                    AmenityID = reader["AmenityID"].ToString()!,
                    Quantity = Convert.ToInt32(reader["Quantity"])
                });
            }
            return list;
        }
    }
}
