using System.ComponentModel.DataAnnotations;

namespace BoothHolder.Model.DTO
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}
