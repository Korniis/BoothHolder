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
    public class BoothController : ControllerBase
    {
        private readonly IBaseService<Booth, BoothDTO> _baseService;
        private readonly IBoothService _boothService;

        private readonly IMapper _mapper;

        public BoothController(IBaseService<Booth, BoothDTO> baseService, IMapper mapper, IBoothService boothService)
        {
            _baseService = baseService;
            _mapper = mapper;
            _boothService = boothService;
        }





        [HttpGet]
        public async Task<ApiResult> GetList([FromQuery] BoothQueryParams queryParams)
        {


            List<Booth> booths = await _boothService.SelectByQuery(queryParams);
            var total = await _boothService.Count(queryParams);

            var page = _mapper.Map<List<BoothVO>>(booths);
            if (page == null)
                return ApiResult.Error("没有找到");
            return ApiResult.Success(new { total, queryParams.PageIndex, queryParams.PageSize, page });
        }
        [HttpGet("{id}")]
        public async Task<ApiResult> Get(long id)
        {
            var booth = await _boothService.SelectFullByIdAsync(id);

            if (booth == null)
                return ApiResult.Error("没有找到");
            return ApiResult.Success(booth);

        }

        [HttpPost]
        public async Task<ApiResult> CreateBooth([FromBody] BoothDTO booth)
        {
            if (booth == null)
            {
                return ApiResult.Error("");
            }

            await _baseService.CreateAsync(booth);



            return ApiResult.Success("成功创建");
        }


        [HttpPost]
        public async Task<ApiResult> UpdateBooth([FromBody] BoothDTO booth)
        {
            if (booth == null)
            {
                return ApiResult.Error("");
            }

            if (await _boothService.UpdateBoothAsync(booth))
                return ApiResult.Success("成功修改");
            else
                return ApiResult.Error("更新错误");
        }

        [HttpDelete]
        public async Task<ApiResult> DeleteBooth(long id)
        {


            if (await _boothService.DeleteBoothAsync(id))
                return ApiResult.Success("成功创建");
            else
                return ApiResult.Error("更新错误");
        }
        [HttpGet]
        public async Task<ApiResult> CalculateRoi()
        {
            var Revenue = await _boothService.GetRevenue();

            var AllRevenue = await _boothService.GetFullRevenue();


            return ApiResult.Success(new { Revenue, Coi = Revenue / AllRevenue });
        }
    }


}
