using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_user")]
    [SugarIndex("unique_userEmail", nameof(Email), OrderByType.Asc, true)]
    [SugarIndex("unique_userName", nameof(UserName), OrderByType.Asc, true)]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        [SugarColumn(Length = 50, IsNullable = false)]
        public string UserName { get; set; }  // 用户名
        [SugarColumn(Length = 100, IsNullable = false)]
        public string Email { get; set; }  // 邮箱
        [SugarColumn(Length = 200, IsNullable = false)]
        public string PasswordHash { get; set; }  // 密码哈希值
        [SugarColumn]
        public string? AvatarUrl { get; set; }  // 密码哈希值
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; } // 摊位描述
        [SugarColumn(Length = 20)]
        public string? Phone { get; set; }  // 联系电话
        [SugarColumn(IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatedTime { get; set; } = DateTime.Now; // 创建时间

        [SugarColumn(IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now; // 更新时间
        [Navigate(typeof(UserRole), nameof(UserRole.UserID), nameof(UserRole.RoleID))]//注意顺序
        public List<Role> RoleList { get; set; }//只能是null不能赋默认值
        [Navigate(NavigateType.OneToMany, nameof(Reservation.UserId))]
        public List<Reservation> Reservations { get; set; } // 关联的预定信息
        [Navigate(typeof(EventUser), nameof(EventUser.UserID), nameof(EventUser.EventID))]//注意顺序
        public List<Event> EventList { get; set; }//只能是null不能赋默认值
        [Navigate(NavigateType.OneToMany, nameof(Booth.UserId))] // 一对多导航
        public List<Booth> Booths { get; set; }
    }



}
