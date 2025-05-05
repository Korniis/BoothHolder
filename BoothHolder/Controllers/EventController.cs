using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;

namespace BoothHolder.UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {

        public readonly IBaseService<Event, Event> _baseService;
        private readonly ISqlSugarClient _sqlSugarClient;

        public EventController(IBaseService<Event, Event> baseService, ISqlSugarClient sqlSugarClient)
        {
            _baseService = baseService;
            _sqlSugarClient = sqlSugarClient;
        }

        [HttpGet]
        public async Task<ApiResult> GetList()
        {

            var events = await _baseService.SelectAllAsync();

            return ApiResult.Success(events);
        }
        [HttpPost]
        [Authorize]

        public async Task<ApiResult> GetMyEvent()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var user = await _sqlSugarClient.Queryable<User>().Includes(x => x.EventList).SingleAsync(x => x.Id == userId);

            return ApiResult.Success(user.EventList);

        }

    }
}
