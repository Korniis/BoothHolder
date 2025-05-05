using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BoothHolder.EnterpriseApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoothController : ControllerBase
    {
        private readonly IBoothService _boothService;
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;

        public BoothController(IBoothService boothService, IMapper mapper, IReservationService reservationService)
        {
            _boothService = boothService;
            _mapper = mapper;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ApiResult> GetList([FromQuery] BoothQueryParams queryParams)
        {
            queryParams.IsAvailable = true;

            List<Booth> booths = await _boothService.SelectByQuery(queryParams);
            var total = await _boothService.Count(queryParams);

            var page = _mapper.Map<List<BoothVO>>(booths);
            if (page == null)
                return ApiResult.Error("没有找到");
            return ApiResult.Success(new { total, queryParams.PageIndex, queryParams.PageSize, page });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResult>> EditBoothInfo([FromBody] EditBoothInfoRequest request)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                // 2. Use more descriptive variable name
                var activeReservation = await _reservationService.SelectOneAsync(r => r.UserId == userId && r.Status == 1);
               // var activeReservation = await _reservationService.sele(userId);
                if (activeReservation == null)
                {
                    return NotFound(ApiResult.Error("未找到有效的租赁信息"));
                }

                // 3. Validate input
                if (string.IsNullOrWhiteSpace(request.BoothName))
                {
                    return BadRequest(ApiResult.Error("摊位名称不能为空"));
                }

                // 4. Use more specific method name
                await _boothService.UpdateBoothInfoAsync(
                    activeReservation.BoothId,
                    request.BoothName.Trim(),
                    request.BoothDescription?.Trim());

                // 5. Return proper API result
                return Ok(ApiResult.Success("摊位信息更新成功"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "修改摊位信息失败，用户ID: {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return StatusCode(500, ApiResult.Error("服务器内部错误"));
            }
        }

    }
    public class EditBoothInfoRequest
    {
        [Required(ErrorMessage = "摊位名称是必填项")]
        [StringLength(50, ErrorMessage = "摊位名称长度不能超过50个字符")]
        public string BoothName { get; set; }

        [StringLength(500, ErrorMessage = "摊位描述长度不能超过500个字符")]
        public string BoothDescription { get; set; }
    }
}
