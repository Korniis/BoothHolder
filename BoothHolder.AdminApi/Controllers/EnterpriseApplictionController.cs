using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BoothHolder.AdminApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EnterpriseApplictionController : ControllerBase
    {
        private readonly IEnterpriseApplicationService _enterpriseApplicationService;
        private readonly IMapper _mapper;
        public EnterpriseApplictionController(IEnterpriseApplicationService enterpriseApplicationService, IMapper mapper)
        {
            _enterpriseApplicationService = enterpriseApplicationService;
            _mapper = mapper;
        }
        [HttpGet]
          [AllowAnonymous]
        public async Task<ApiResult> GetEnterpriseApplication([FromQuery] EnterpriseApplicationParams queryParams)
        {
            List<EnterpriseApplication> enterpriseApplications = await _enterpriseApplicationService.SelectByQuery(queryParams);
            long total = await _enterpriseApplicationService.CountTotal(queryParams);
            var page = _mapper.Map<List<EnterpriseApplicationVO>>(enterpriseApplications);
            return ApiResult.Success(new { total, queryParams.PageIndex, queryParams.PageSize, page });
        }
        [HttpPost]
        public async Task<ApiResult> RemarkApplication(RemarkQuery remarkQuery)
        {
            var RemarkById = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int result = await _enterpriseApplicationService.RemarkApplication(RemarkById, remarkQuery);
            if (result == 0) return ApiResult.Error("未知错误");
            return ApiResult.Success("审批成功");
        }
    }
}
