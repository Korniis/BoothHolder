using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoothHolder.AdminApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
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
        public async Task<ApiResult> GetEnterpriseApplycation([FromQuery] EnterpriseApplicationParams queryParams)
        {
            List<EnterpriseApplication> enterpriseApplications = await _enterpriseApplicationService.SelectByQuery(queryParams);
             var total= enterpriseApplications.Count();
             var page = _mapper.Map<List<EnterpriseApplicationVO>>(enterpriseApplications);
            return ApiResult.Success(new { total, queryParams.PageIndex, queryParams.PageSize, page });

        }



    }
}
