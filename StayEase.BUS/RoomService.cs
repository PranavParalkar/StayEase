using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Room CRUD, status management, search/filter operations.
    /// ID Pattern: P + date(ddMMyy) + sequential number
    /// </summary>
    public class RoomService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Room> GetAllRooms()
        {
            return _db.GetListRoom("SELECT * FROM Room WHERE IsDeleted = 0");
        }

        public List<Room> SearchRooms(string keyword)
        {
            return _db.GetListRoom($"SELECT * FROM Room WHERE IsDeleted = 0 AND (RoomName LIKE N'%{keyword}%' OR RoomID LIKE '%{keyword}%')");
        }

        public List<Room> FilterByStatus(int status)
        {
            return _db.GetListRoom($"SELECT * FROM Room WHERE IsDeleted = 0 AND Status = {status}");
        }

        public List<Room> FilterByType(int roomType)
        {
            return _db.GetListRoom($"SELECT * FROM Room WHERE IsDeleted = 0 AND RoomType = {roomType}");
        }

        public string GenerateID()
        {
            string dateStr = DateTime.Now.ToString("ddMMyy");
            string prefix = "P" + dateStr;
            int count = _db.ExecuteScalar($"SELECT COUNT(*) FROM Room WHERE RoomID LIKE '{prefix}%'");
            return prefix + (count + 1).ToString("D4");
        }

        public bool AddRoom(Room room)
        {
            string query = $"INSERT INTO Room (RoomID, RoomName, RoomType, Price, RoomDetail, Status, Condition, IsDeleted) VALUES ('{room.RoomID}', N'{room.RoomName}', {room.RoomType}, {room.Price}, {room.RoomDetail}, {room.Status}, {room.Condition}, 0)";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateRoom(Room room)
        {
            string query = $"UPDATE Room SET RoomName = N'{room.RoomName}', RoomType = {room.RoomType}, Price = {room.Price}, RoomDetail = {room.RoomDetail}, Status = {room.Status}, Condition = {room.Condition} WHERE RoomID = '{room.RoomID}'";
            return _db.ExecuteNonQuery(query) > 0;
        }

        public bool DeleteRoom(string roomId)
        {
            return _db.ExecuteNonQuery($"UPDATE Room SET IsDeleted = 1 WHERE RoomID = '{roomId}'") > 0;
        }

        public bool UpdateRoomStatus(string roomId, int status)
        {
            return _db.ExecuteNonQuery($"UPDATE Room SET Status = {status} WHERE RoomID = '{roomId}'") > 0;
        }

        public List<Room> GetAvailableRooms()
        {
            return _db.GetListRoom("SELECT * FROM Room WHERE IsDeleted = 0 AND Status = 0");
        }
    }
}
