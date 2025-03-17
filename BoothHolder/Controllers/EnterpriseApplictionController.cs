using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoothHolder.UserApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EnterpriseApplictionController : ControllerBase
    {
        private readonly IEnterpriseApplicationService _enterpriseApplicationService;

        public EnterpriseApplictionController(IEnterpriseApplicationService enterpriseApplicationService)
        {
            _enterpriseApplicationService = enterpriseApplicationService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ApplyEnterprise(EnterpriseApplyDTO enterpriseApplyDTO)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            int val = await _enterpriseApplicationService.ApplyEnterprise(userId, enterpriseApplyDTO);
            if (val > 0)
                return ApiResult.Success("申请成功，请稍后");
            else if (val == -1)
                return ApiResult.Error("提交后无法更改");
            else
                return ApiResult.Error("申请失败");

        }
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> EnterpriseAgain(EnterpriseApplyDTO enterpriseApplyDTO)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            int val = await _enterpriseApplicationService.EnterpriseAgain(userId, enterpriseApplyDTO);
            if (val > 0)
                return ApiResult.Success("申请成功，请稍后");
            else if (val == -1)
                return ApiResult.Error("未申请或申请已通过");
            else
                return ApiResult.Error("申请失败");

        }
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetApplication()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            EnterpriseApplication    application= await _enterpriseApplicationService.SelectOneByUserIdAsync(userId);
            if (application == null) return ApiResult.NotFound("未找到");
            return ApiResult.Success(new {application.Status,application.EnterpriseName,application.ContactPhone,application.RemarkSupport });
        }
    }
}
