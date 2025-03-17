using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.VO
{
    public class UserVO
    {
        public long Id { get; set; }
        public string UserName { get; set; }  // 用户名
        public string? Email { get; set; }  // 邮箱
        public string? Phone { get; set; }  // 联系电话
        public string? AvatarUrl { get; set; }  // 密码哈希值
        public string? Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public List<string>? RoleNames { get; set; }
    }
}
