using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Service CRUD with category filtering and image support.
    /// ID Pattern: DV + date(ddMMyy) + sequential number
    /// </summary>
    public class HotelServiceService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Service> GetAllServices()
        {
            return _db.GetListService("SELECT * FROM [Service] WHERE IsDeleted = 0");
        }

        public List<Service> SearchServices(string keyword)
        {
            return _db.GetListService($"SELECT * FROM [Service] WHERE IsDeleted = 0 AND (ServiceName LIKE N'%{keyword}%')");
        }

        public List<Service> FilterByCategory(string category)
        {
            return _db.GetListService($"SELECT * FROM [Service] WHERE IsDeleted = 0 AND ServiceType = N'{category}'");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "DV" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM [Service] WHERE ServiceID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool AddService(Service svc)
        {
            string imgVal = svc.Image != null ? $"N'{svc.Image}'" : "NULL";
            string query = $"INSERT INTO [Service] (ServiceID, ServiceName, ServiceType, Price, Image, IsDeleted) VALUES ('{svc.ServiceID}', N'{svc.ServiceName}', N'{svc.ServiceType}', {svc.Price}, {imgVal}, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateService(Service svc)
        {
            string imgVal = svc.Image != null ? $"N'{svc.Image}'" : "NULL";
            string query = $"UPDATE [Service] SET ServiceName = N'{svc.ServiceName}', ServiceType = N'{svc.ServiceType}', Price = {svc.Price}, Image = {imgVal} WHERE ServiceID = '{svc.ServiceID}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool DeleteService(string serviceId)
        {
            return _db.ExecuteNonQuery($"UPDATE [Service] SET IsDeleted = 1 WHERE ServiceID = '{serviceId}'") > 0;
        }
    }
}
