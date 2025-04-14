using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
namespace BoothHolder.EnterpriseApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    // [Authorize(Roles ="Enterprise")]
    public class EnterpriseController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly IBoothService _boothService;
        private readonly IReservationService _reservationService;
        public EnterpriseController(IUserService userService, IEnterpriseService enterpriseService, IBoothService boothService, IReservationService reservationService)
        {
            _userService = userService;
            _enterpriseService = enterpriseService;
            _boothService = boothService;
            _reservationService = reservationService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Login(UserLoginDTO loginDTO)
        {
            var token = await _enterpriseService.GetToken(loginDTO);
            Log.Information($"{loginDTO.Username}登录");
            if (token == null)
            {
                Log.Information($"{loginDTO.Username}登录失败");
                return ApiResult.AuthError("用户名或密码错误，请检查您是否是企业");
            }
            Log.Information($"{loginDTO.Username}登录成功");
            return ApiResult.Success(token);
        }
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> UserInfo()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            User user = await _enterpriseService.GetUserInfo(userId);
            if (user == null) return ApiResult.Error("无该用户");
            return ApiResult.Success(new { username = user.UserName, useravatar = user.AvatarUrl });
        }
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> RequestReservation([FromBody] ReservationRequestDTO request)
        {
            //long userId = request.UserId;
            long userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            request.UserId = userId;
            try
            {
                // 1. 基础参数验证
                if (!ModelState.IsValid)
                {
                    return ApiResult.Error("参数验证失败");
                }
                // 2. 验证摊位可用性
                var booth = await _boothService.GetByIdAsync(request.BoothId);
                if (booth == null || !booth.IsAvailable)
                //if (booth == null)
                {
                    return ApiResult.Error("摊位不存在或不可用");
                }
                bool isHasOrder = await _reservationService.ExistsConflictUser(userId
                  );
                // 3. 检查日期冲突
                //var isConflict = await _reservationService.ExistsConflictReservation(
                //    request.BoothId,
                //    request.StartDate,
                //    request.EndDate);
                if (isHasOrder)
                {
                    return ApiResult.Error("您已有摊位");
                }
                await _reservationService.AddAsync(request);
                // 5. 发送预定确认通知
                //   await _notificationService.SendReservationConfirmedAsync(reservation);
                return ApiResult.Success(new
                {
                    // ReservationId = reservation.Id,
                    Message = "预定已直接确认通过"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "预定请求失败");
                return ApiResult.Error("预定请求失败，请稍后重试");
            }
        }
        [HttpGet]
        [Authorize]

        public async Task<ApiResult> CountPayments()
        {
            long userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var pay = await _reservationService.CountPayments(userId);
            return ApiResult.Success(pay);
        }
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> RemoveReservation()
        {
            long userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);





            int stuat = await _reservationService.RemoveReservationAsync(userId);
            if (stuat == 0)
            {
                return ApiResult.Error("您无预定");
            }


            return ApiResult.Success("取消成功");


        }
    }
}
