using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_userrole")]
    public class UserRole
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserID { get; set; }  // 用户ID

        [SugarColumn(IsPrimaryKey = true)]
        public int RoleID { get; set; }  // 角色ID
    }
}
