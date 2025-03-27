using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoothHolder.EnterpriseApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoothController : ControllerBase
    {
        private readonly IBoothService _boothService;
        private readonly IMapper _mapper;

        public BoothController(IBoothService boothService, IMapper mapper)
        {
            _boothService = boothService;
            _mapper = mapper;
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

    }
}
