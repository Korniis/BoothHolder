using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_role")]
    public class Role
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]  // 主键且自增
        public int RoleID { get; set; }  // 角色ID
        [SugarColumn(Length = 50)]
        public string RoleName { get; set; }  // 角色名

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? CreatedAt { get; set; }  // 创建时间

        [Navigate(typeof(UserRole), nameof(UserRole.RoleID), nameof(UserRole.UserID))]//注意顺序
        public List<User> UserList { get; set; }//只能是null不能赋默认值
    }
}
