using StayEase.DAO;
using StayEase.DTO;

namespace StayEase.BUS
{
    /// <summary>
    /// Role and Permission management.
    /// </summary>
    public class RoleService
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public List<Role> GetAllRoles()
        {
            return _db.GetListRole("SELECT * FROM [Role]");
        }

        public bool AddRole(Role role)
        {
            return _db.ExecuteNonQuery($"INSERT INTO [Role] VALUES ('{role.RoleID}',N'{role.RoleName}')") > 0;
        }

        public bool UpdateRole(Role role)
        {
            return _db.ExecuteNonQuery($"UPDATE [Role] SET RoleName=N'{role.RoleName}' WHERE RoleID='{role.RoleID}'") > 0;
        }

        public bool DeleteRole(string roleId)
        {
            _db.ExecuteNonQuery($"DELETE FROM RoleFeature WHERE RoleID='{roleId}'");
            return _db.ExecuteNonQuery($"DELETE FROM [Role] WHERE RoleID='{roleId}'") > 0;
        }

        public List<Feature> GetAllFeatures()
        {
            return _db.GetListFeature("SELECT * FROM Feature");
        }

        public List<RoleFeature> GetRoleFeatures(string roleId)
        {
            return _db.GetListRoleFeature($"SELECT * FROM RoleFeature WHERE RoleID='{roleId}'");
        }

        public bool AssignFeature(string roleId, string featureId)
        {
            return _db.ExecuteNonQuery($"INSERT INTO RoleFeature VALUES ('{roleId}','{featureId}')") > 0;
        }

        public bool RemoveFeature(string roleId, string featureId)
        {
            return _db.ExecuteNonQuery($"DELETE FROM RoleFeature WHERE RoleID='{roleId}' AND FeatureID='{featureId}'") > 0;
        }

        public bool SetRoleFeatures(string roleId, List<string> featureIds)
        {
            _db.ExecuteNonQuery($"DELETE FROM RoleFeature WHERE RoleID='{roleId}'");
            foreach (var fid in featureIds)
                _db.ExecuteNonQuery($"INSERT INTO RoleFeature VALUES ('{roleId}','{fid}')");
            return true;
        }

        public string GenerateID()
        {
            int count = _db.ExecuteScalar("SELECT COUNT(*) FROM [Role]");
            return "R" + (count + 1).ToString("D3");
        }
    }
}
