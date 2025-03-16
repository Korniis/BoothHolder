using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;

namespace BoothHolder.AdminApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;


       

        public AdminController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<ApiResult> Login(UserLoginDTO loginDTO)
        {

            var token = await _adminService.GetToken(loginDTO);
            Log.Information($"{loginDTO.Username}登录");
            if (token == null)
            {
                Log.Information($"{loginDTO.Username}登录失败");
                return ApiResult.AuthError("用户名或密码错误，请检查您是否是管理员");
            }
            Log.Information($"{loginDTO.Username}登录成功");
            return ApiResult.Success(token);


        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult> UserInfo()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
           

            User user= await _adminService.GetUserInfo(userId);
            if (user==null) return ApiResult.Error("无该用户");
            return ApiResult.Success(new { username = user.UserName, useravatar = user.AvatarUrl });
        }

    }
}
