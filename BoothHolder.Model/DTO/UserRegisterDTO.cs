namespace BoothHolder.Model.DTO
{
    using System.ComponentModel.DataAnnotations;
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6到100个字符之间")]
        public string Password { get; set; }
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }
        [RegularExpression(@"^(\d+)?$", ErrorMessage = "电话号码必须是数字或为空")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "验证码无效")]
        public string Code { get; set; }
    }
    public enum RegisterStatus
    {
        EmailWrong,
        Success,
        Error,
    }
}
