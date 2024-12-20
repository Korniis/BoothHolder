using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoothHolder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        [Authorize(Roles="Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult> UpdateBooth([FromBody] BoothDTO booth)
        {
            if (booth == null)
            {
                return ApiResult.Error("");
            }

        if( await _boothService.UpdateBoothAsync(booth))
             return ApiResult.Success("成功创建");
        else
                return ApiResult.Error("更新错误");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult> DeleteBooth(long id)
        {


            if (await _boothService.DeleteBoothAsync(id))
                return ApiResult.Success("成功创建");
            else
                return ApiResult.Error("更新错误");
        }
    }

        //[HttpGet]
        //[Authorize]
        //public async Task SeedDataAsync()
        //{
        //    // 插入品牌数据
        //    _db.Insertable(new[]
        //    {
        //    new BrandType
        //    {
        //         BrandTypeId = 1,
        //        BrandTypeName = "Applewood Furniture",
        //        BrandDescription = "高端家具品牌，注重细节与品质。"
        //    },
        //    new BrandType
        //    {
        //         BrandTypeId = 2,
        //        BrandTypeName = "Maplecraft",
        //        BrandDescription = "经典的木制家具品牌，以其耐用性而著称。"
        //    },
        //    new BrandType
        //    {
        //         BrandTypeId = 3,
        //        BrandTypeName = "Walnutwood Creations",
        //        BrandDescription = "专业定制现代风格的家具，提供创新设计和舒适体验。"
        //    },
        //    new BrandType
        //    {
        //        BrandTypeId= 4,
        //        BrandTypeName = "Birchcraft Designs",
        //        BrandDescription = "以简洁、实用为设计理念，专注于家庭家具。"
        //    }
        //}).ExecuteCommand();
        //    // 插入家具摊位数据
        //    _db.Insertable(new[]
        //{
        //    new Booth
        //    {
        //        BoothId = 1,
        //        BoothName = "摊位 Applewood 1",
        //        Location = "123 Main St",
        //        DailyRate = 150.75m,
        //        IsAvailable = true,
        //        BrandTypeId = 1,
        //        AvailableDate = DateTime.Parse("2024-12-01"),
        //        Description = "适合展示高档家具的摊位。"
        //    },
        //    new Booth
        //    {
        //        BoothId = 2,
        //        BoothName = "摊位 Maplecraft 2",
        //        Location = "456 Oak St",
        //        DailyRate = 120.00m,
        //        IsAvailable = false,
        //        BrandTypeId = 2,
        //        AvailableDate = DateTime.Parse("2024-12-05"),
        //        Description = "经典木制家具展示区域。"
        //    },
        //    new Booth
        //    {
        //        BoothId = 3,
        //        BoothName = "摊位 Walnutwood 3",
        //        Location = "789 Pine St",
        //        DailyRate = 180.50m,
        //        IsAvailable = true,
        //        BrandTypeId = 4,
        //        AvailableDate = DateTime.Parse("2024-12-10"),
        //        Description = "宽敞的摊位，适合大型家具。"
        //    },
        //    new Booth
        //    {
        //        BoothId = 4,
        //        BoothName = "摊位 Birchcraft 4",
        //        Location = "321 Elm St",
        //        DailyRate = 200.00m,
        //        IsAvailable = true,
        //        BrandTypeId = 2,
        //        AvailableDate = DateTime.Parse("2024-12-15"),
        //        Description = "优越位置，吸引高流量。"
        //    }
        //}).ExecuteCommand();
        //}
    
}
