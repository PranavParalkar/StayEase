using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Amenity CRUD and room assignment.
    /// ID Pattern: TI + date(ddMMyy) + sequential number
    /// </summary>
    public class AmenityService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Amenity> GetAllAmenities()
        {
            return _db.GetListAmenity("SELECT * FROM Amenity WHERE IsDeleted = 0");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "TI" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Amenity WHERE AmenityID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool AddAmenity(Amenity amenity)
        {
            return _db.ExecuteNonQuery($"INSERT INTO Amenity VALUES ('{amenity.AmenityID}',N'{amenity.AmenityName}',0)") > 0;
        }

        public bool UpdateAmenity(Amenity amenity)
        {
            return _db.ExecuteNonQuery($"UPDATE Amenity SET AmenityName=N'{amenity.AmenityName}' WHERE AmenityID='{amenity.AmenityID}'") > 0;
        }

        public bool DeleteAmenity(string id)
        {
            return _db.ExecuteNonQuery($"UPDATE Amenity SET IsDeleted=1 WHERE AmenityID='{id}'") > 0;
        }

        public List<RoomAmenity> GetRoomAmenities(string roomId)
        {
            return _db.GetListRoomAmenity($"SELECT * FROM RoomAmenity WHERE RoomID='{roomId}'");
        }

        public bool AssignAmenityToRoom(string roomId, string amenityId, int quantity)
        {
            return _db.ExecuteNonQuery($"INSERT INTO RoomAmenity VALUES ('{roomId}','{amenityId}',{quantity})") > 0;
        }

        public bool RemoveAmenityFromRoom(string roomId, string amenityId)
        {
            return _db.ExecuteNonQuery($"DELETE FROM RoomAmenity WHERE RoomID='{roomId}' AND AmenityID='{amenityId}'") > 0;
        }
    }
}
