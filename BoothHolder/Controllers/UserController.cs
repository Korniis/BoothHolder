using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.RegularExpressions;
namespace BoothHolder.UserApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDatabase _redisDatabase;
        private readonly IMapper _mapper;
        private readonly string _userDataRedis = "UserDataPrefix";
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
        [Authorize(Roles = "User")]
        public async Task<ApiResult> GetRoles()
        {
            Log.Debug("getRoles.......");
            var roles = await _userService.GetRoles();
            var user = User;
            return ApiResult.Success(roles);
        }
        /*      [HttpGet("/token")]
              public ApiResult GetUserByBase(int id) {
                  var user = _baseService.Query();
                  return ApiResult.Success(user);
              }*/
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateUser(UserDTO userDTO)
        {
            await _userService.CreateUser(userDTO);
            return ApiResult.Success("创建成功");
        }
        [HttpPost]
        public async Task<ApiResult> Register(UserRegisterDTO userRegisterDTO)
        {
            Log.Information($"{userRegisterDTO.UserName}+{userRegisterDTO.Email}  注册");
            try
            {
                RegisterStatus reg = await _userService.Register(userRegisterDTO);
                if (reg == RegisterStatus.Success)
                {
                    Log.Information($"{userRegisterDTO.UserName}+{userRegisterDTO.Email}  注册成功");
                    return ApiResult.Success("注册成功");
                }
                if (reg == RegisterStatus.EmailWrong)
                {
                    return ApiResult.Error("验证码错误");
                }
                return ApiResult.Error("邮箱或用户名已存在");
            }
            catch (Exception ex)
            {
                // 记录异常信息并返回通用错误
                return ApiResult.Error("服务器内部错误，请稍后重试");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> Userinfo()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            string cacheKey = $"user:{userId}";
            var cachedUserInfo = await _redisDatabase.StringGetAsync(_userDataRedis + cacheKey);
            User user;
            if (cachedUserInfo.HasValue)
            {
                // 如果缓存中有数据，直接返回
                user = JsonConvert.DeserializeObject<User>(cachedUserInfo);
            }
            else
            {
                // 如果缓存中没有数据，则从数据库中查询
                user = await _userService.SelectOneByIdAsync(userId);
            }
            if (user != null)
            {
                // 将用户数据序列化并存储到 Redis 中，设置过期时间（例如 1小时）
                await _redisDatabase.StringSetAsync(_userDataRedis + cacheKey, JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));
                return ApiResult.Success(new { username = user.UserName, useravatar = user.AvatarUrl });
            }
            else
            {
                return ApiResult.Error("无该用户");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> UserMoreinfo()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            string cacheKey = $"user:{userId}";
            User user = null;
            var cachedUserInfo = await _redisDatabase.StringGetAsync(_userDataRedis + cacheKey);
            if (cachedUserInfo.HasValue)
            {
                // 如果缓存中有数据，直接返回
                user = JsonConvert.DeserializeObject<User>(cachedUserInfo);

            }
            // 如果缓存中没有数据，则从数据库中查询
            else
            {
                user = await _userService.SelectOneByIdAsync(userId);
            }
            if (user != null)
            {
                // 将用户数据序列化并存储到 Redis 中，设置过期时间（例如 1小时）
                await _redisDatabase.StringSetAsync(_userDataRedis + cacheKey, JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));
                _mapper.Map<UserDTO>(user);
                return ApiResult.Success(_mapper.Map<UserDTO>(user));
            }
            else
            {
                return ApiResult.Error("无该用户");
            }

        }
        [HttpPost]
        public async Task<ApiResult> Login(UserLoginDTO loginDTO)
        {
            var token = await _userService.GetToken(loginDTO);
            Log.Information($"{loginDTO.Username}登录");
            if (token == null)
            {
                Log.Information($"{loginDTO.Username}登录失败");
                return ApiResult.AuthError("用户名或密码错误");
            }
            Log.Information($"{loginDTO.Username}登录成功");
            return ApiResult.Success(token);
        }
        /// <summary>
        ///发送验证码
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isRemake"> 是否重新验证 true重新 false 注册</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult> SendConfirmCode(string email)
        {
            bool isSend = await _userService.SendConfirmCode(email, true);
            Log.Information($"{email}发送验证码");
            return isSend
                ? ApiResult.Success("发送成功")
                : ApiResult.Error("发送失败");
        }
        /// <summary>
        ///发送验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> SendRegisterCode(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern)) return ApiResult.Error("请输入正确邮箱");

            bool isSend = await _userService.SendConfirmCode(email, false);
            Log.Information($"{email}注册邮件");
            return isSend
                ? ApiResult.Success("发送成功")
                : ApiResult.Error("用户已存在");
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResult> SetAvatar([FromBody] string avatarurl)
        {

            if (string.IsNullOrEmpty(avatarurl))
                return ApiResult.Error("不能为空");

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (await _userService.SetAvatar(userId, avatarurl))
                return ApiResult.Success("成功");
            else
                return ApiResult.Error("未知错误");


        }
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> UpdateUser(UserDTO userDTO)
        {

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            string cacheKey = $"user:{userId}";

            if (await _userService.UpdateUserAsync(userDTO, userId))
            {
                await _redisDatabase.KeyDeleteAsync(_userDataRedis + cacheKey);
                return ApiResult.Success("成功，大约3分钟后更新");
            }
            else
                return ApiResult.Error("请重试");

        }
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddEvent(long EvevtId)
        {

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _userService.AddEvevt(userId, EvevtId))
            {


                return ApiResult.Success("成功，大约3分钟后更新");
            }
            else
                return ApiResult.Error("请重试");

        }
    }







}
