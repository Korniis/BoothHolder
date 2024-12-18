using BoothHolder.Common;
using BoothHolder.Common.Configration;
using BoothHolder.Common.Jwt;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using MapsterMapper;
using StackExchange.Redis;
using System.Security.Claims;
using Role = BoothHolder.Model.Entity.Role;
namespace BoothHolder.Service
{
    public class UserService : BaseService<User, UserDTO>, IUserService
    {
        private readonly IUserRepository _userResposity;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<User> _baseRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userroleRepository;
        private readonly IDatabase _redisDatabase;
        private readonly string _redisPrefix = "EmailVerification:";
        private readonly string _redisToken = "TokenVerification:";

        public UserService(IUserRepository userResposity,
            IBaseRepository<User> repository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<UserRole> userroleRepository,
            IMapper mapper,
            IConnectionMultiplexer connectionMultiplexer) : base(repository, mapper) // 调用 BaseService 的构造函数
        {
            _userResposity = userResposity;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _baseRepository = repository;
            _userroleRepository = userroleRepository;
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }
        public async Task<List<RoleDTO>> GetRoles()
        {
            List<Role> roles = await _userResposity.GetRoles();
            return _mapper.Map<List<RoleDTO>>(roles);
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            List<User> users = await _userResposity.GetUsers();
            return _mapper.Map<List<UserDTO>>(users);
        }
        //创建用户
        public async Task<int> CreateUser(UserDTO user)
        {
            User newUser = _mapper.Map<User>(user);
            newUser.PasswordHash = MD5Helper.GetMD5(user.Password);
            var id = await _userResposity.CreateUser(newUser);
            Role role = await _roleRepository.SelectOneAsync(t => t.RoleName == "User");
            newUser.RoleList = [role];
            // 5. 插入多对多关系：将用户与角色关联
            var userRoleRelations = newUser.RoleList.Select(role => new UserRole
            {
                UserID = id,  // 注意：这里假设用户的 Id 已经生成
                RoleID = role.RoleID
            }).ToList();
            await _userroleRepository.CreateAsync(userRoleRelations);
            return id;
        }
        //获取用户token
        public async Task<string> GetToken(UserLoginDTO loginDTO)
        {
            var user = await _userResposity.SelectOneWithRoleAsync(x => x.UserName == loginDTO.Username);
            var password = MD5Helper.GetMD5(loginDTO.Password);
            // 如果用户不存在，抛出异常
            if (user == null || password != user.PasswordHash) return null;
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
            await _redisDatabase.StringSetAsync(_redisToken + user.UserName, token, TimeSpan.FromMinutes(expiresIn));

            return token;
        }
        //用户注册
        public async Task<RegisterStatus> Register(UserRegisterDTO userRegisterDTO)
        {
            User newUser = _mapper.Map<User>(userRegisterDTO);
            string storedCode = await _redisDatabase.StringGetAsync(_redisPrefix + userRegisterDTO.Email);
            if (string.IsNullOrEmpty(storedCode) || !storedCode.Equals(userRegisterDTO.Code))
            {
                return RegisterStatus.EmailWrong;
            }
            try
            {
                newUser.PasswordHash = MD5Helper.GetMD5(userRegisterDTO.Password);
                var id = await _userResposity.CreateUser(newUser);
                Role role = await _roleRepository.SelectOneAsync(t => t.RoleName == "User");
                newUser.RoleList = [role];
                // 5. 插入多对多关系：将用户与角色关联
                var userRoleRelations = newUser.RoleList.Select(role => new UserRole
                {
                    UserID = id,  // 注意：这里假设用户的 Id 已经生成
                    RoleID = role.RoleID
                }).ToList();
                await _userroleRepository.CreateAsync(userRoleRelations);
            }
            catch (Exception ex)
            {
                return RegisterStatus.Error;
            }
            _redisDatabase.KeyDelete(_redisPrefix + userRegisterDTO.Email);
            return RegisterStatus.Success;
        }
        public async Task<bool> SendConfirmCode(string email, bool isRemake)
        {
            // 使用 lambda 表达式检查用户是否已存在
            if (!isRemake && await _userResposity.SelectOneAsync(x => x.Email == email) != null)
                return false;



            // 生成验证码
            var code = new Random().Next(100000, 999999).ToString();



            // 使用 lambda 表达式存储到 Redis 并设置有效期
            return await _redisDatabase.StringSetAsync(_redisPrefix + email, code, TimeSpan.FromMinutes(3)) ? true : false;
            //   return await SMTPHelper.UseSmtpAsync(email, "hajimi", $"验证码:{code}")? true: false;
        }
    }
}
