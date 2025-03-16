namespace BoothHolder.Model.DTO
{
    public class UserDTO
    {


        public long? Id;
        public string UserName { get; set; }  // 用户名
        public string? Password { get; set; }
        public string? Email { get; set; }  // 邮箱
        public string? Phone { get; set; }  // 联系电话
        public string? AvatarUrl { get; set; }  // 密码哈希值
        public string? Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public List<string>? RoleNames { get; set; }
    }
    public class UserQueryParams
    {
  
        public string? UserName { get; set; }  // 用户名
   
        public string? Description { get; set; }
        public List<string>? RoleNames { get; set; }
        // 分页参数
        public int PageIndex { get; set; } = 0;  // 默认第一页
        public int PageSize { get; set; } = 30;  // 默认每页10条
    }


}
