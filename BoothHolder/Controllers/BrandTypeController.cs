using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace BoothHolder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BrandTypeController : ControllerBase
    {
        private readonly IBaseService<BrandType,BrandType> _baseService;

        public BrandTypeController(IBaseService<BrandType, BrandType> baseService)
        {
            _baseService = baseService;
        }
        [HttpGet]
        public async Task<ApiResult> Get()
        {

            var types= await _baseService.SelectAllAsync();
            return ApiResult.Success(types.Select(x=>new {x.Id,x.BrandTypeName }) );

        }
    }
}
