using BoothHolder.Common;
using BoothHolder.Common.Configration;
using BoothHolder.Common.Jwt;
using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using MapsterMapper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Claims;

namespace BoothHolder.Service
{
    public class AdminService : BaseService<User, UserDTO>, IAdminService
    {

        private readonly IMapper _mapper;
        private readonly IUserRepository _userResposity;
        private readonly IBaseRepository<User> _baseRepository;
        private readonly string _redisPrefix = "EmailVerification:";
        private readonly string __adminredistoken = "admintoken:";
        private readonly string _adminDataRedis = "AdminDataPrefix";

        private readonly string _redisToken = "TokenVerification:";
        private readonly IDatabase _redisDatabase;

        public AdminService(IConnectionMultiplexer connectionMultiplexer, IBaseRepository<User> repository, IMapper mapper, IUserRepository userResposity) : base(repository, mapper)
        {
            _userResposity = userResposity;
            _mapper = mapper;
            _baseRepository = repository;
            _redisDatabase = connectionMultiplexer.GetDatabase();

        }

     

        public async Task<string> GetToken(UserLoginDTO loginDTO)
        {
            var user = await _userResposity.SelectOneWithRoleAsync(x => x.UserName == loginDTO.Username);
            var password = MD5Helper.GetMD5(loginDTO.Password);
            // 如果用户不存在，抛出异常
            if (user == null || password != user.PasswordHash || !user.RoleList.Any(r => r.RoleID == 1)) return null;

            // 创建用户声明（Claims），包含用户名、邮箱和角色
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // 可选：用户 ID
            };
            // 将角色添加到 Claims 中
            foreach (var role in user.RoleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }
            var token = JwtHelper.CreateToken(claims);
            var expiresIn = Convert.ToInt32(AppSettings.app("JwtSettings:ClockSkewMinutes"));
            await _redisDatabase.StringSetAsync(__adminredistoken + user.UserName, token, TimeSpan.FromMinutes(expiresIn));

            return token;
        }

        public async Task<User> GetUserInfo(int userId)
        {
            User user;
            string cacheKey = $"user:{userId}";

            var cachedUserInfo = await _redisDatabase.StringGetAsync(_adminDataRedis + cacheKey);


            if (cachedUserInfo.HasValue)
            {
                // 如果缓存中有数据，直接返回
                user = JsonConvert.DeserializeObject<User>(cachedUserInfo);
            }
            else
            {
                // 如果缓存中没有数据，则从数据库中查询
                user = await _userResposity.SelectOneByIdAsync(userId);
            }
            // 将用户数据序列化并存储到 Redis 中，设置过期时间（例如 1小时）
            if (user != null)
                await _redisDatabase.StringSetAsync(_adminDataRedis + cacheKey,
                                                    JsonConvert.SerializeObject(user),
                                                    TimeSpan.FromHours(1));
              
            
          return user;
        }
    }
}

