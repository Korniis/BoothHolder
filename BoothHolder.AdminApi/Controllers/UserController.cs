using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using BoothHolder.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StackExchange.Redis;
using System.Linq;

namespace BoothHolder.AdminApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
   // [Authorize(Roles ="admin")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IDatabase _redisDatabase;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IConnectionMultiplexer connection, IMapper mapper)
        {
            _userService = userService;
            _redisDatabase = connection.GetDatabase();
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<List<UserDTO>> GetUsers()
        {
            Log.Debug("getUser.......");
            var users = await _userService.GetUsers();
            return users;
        }

        [HttpGet]
        public async Task<ApiResult> GetList([FromQuery] UserQueryParams queryParams)
        {


            List<User> users = await _userService.SelectByQuery(queryParams);
            //var total = await _userService.Count(queryParams);
            var total = users.Count();

            var page = _mapper.Map<List<UserVO>>(users);
            if (page == null)
                return ApiResult.Error("没有找到");
            return ApiResult.Success(new { total, queryParams.PageIndex, queryParams.PageSize, page });
        }
      


    }
}
