using BoothHolder.Common;
using BoothHolder.Common.Configration;
using BoothHolder.Common.Jwt;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using BoothHolder.Repository.Impl;
using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Role = BoothHolder.Model.Entity.Role;
namespace BoothHolder.Service
{
    public class UserService : BaseService<User, UserDTO>, IUserService
    {
        private readonly IUserRepository _userRespository;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<User> _baseRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userroleRepository;
        private readonly IBaseRepository<EventUser> _eventUserRepository;
        private readonly IDatabase _redisDatabase;
        private readonly string _redisPrefix = "EmailVerification:";
        private readonly string _userredistoken = "usertoken:";

        public UserService(IUserRepository userResposity,
            IBaseRepository<User> repository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<UserRole> userroleRepository,
            IMapper mapper,
            IConnectionMultiplexer connectionMultiplexer,
            IBaseRepository<EventUser> eventUserRepository) : base(repository, mapper) // 调用 BaseService 的构造函数
        {
            _userRespository = userResposity;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _baseRepository = repository;
            _userroleRepository = userroleRepository;
            _redisDatabase = connectionMultiplexer.GetDatabase();
            _eventUserRepository = eventUserRepository;
        }
        public async Task<List<RoleDTO>> GetRoles()
        {
            List<Role> roles = await _userRespository.GetRoles();
            return _mapper.Map<List<RoleDTO>>(roles);
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            List<User> users = await _userRespository.GetUsers();
            return _mapper.Map<List<UserDTO>>(users);
        }
        //创建用户
        public async Task<int> CreateUser(UserDTO user)
        {
            User newUser = _mapper.Map<User>(user);
            newUser.PasswordHash = MD5Helper.GetMD5(user.Password);
            var id = await _userRespository.CreateUser(newUser);
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
            var user = await _userRespository.SelectOneWithRoleAsync(x => x.UserName == loginDTO.Username);
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
            await _redisDatabase.StringSetAsync(_userredistoken + user.UserName, token, TimeSpan.FromMinutes(expiresIn));

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
                var id = await _userRespository.CreateUser(newUser);
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
            if (!isRemake && await _userRespository.SelectOneAsync(x => x.Email == email) != null)
                return false;



            // 生成验证码
            var code = new Random().Next(100000, 999999).ToString();



            // 使用 lambda 表达式存储到 Redis 并设置有效期
            return await _redisDatabase.StringSetAsync(_redisPrefix + email, code, TimeSpan.FromMinutes(3)) ? true : false;
            //   return await SMTPHelper.UseSmtpAsync(email, "hajimi", $"验证码:{code}")? true: false;
        }

        public async Task<bool> SetAvatar(int userId, string avatarurl)
        {
            var user = await _baseRepository.SelectOneByIdAsync(userId);

            user.AvatarUrl = avatarurl;
            user.UpdatedTime = DateTime.Now;
            return await _baseRepository.UpdateAsync(user);

        }

        public async Task<bool> UpdateUserAsync(UserDTO userDTO, long id)
        {
            var user = await _baseRepository.SelectOneByIdAsync(id);

            if (!string.IsNullOrEmpty(userDTO.UserName))
            {
                user.UserName = userDTO.UserName;
            }
            if (!string.IsNullOrEmpty(userDTO.Description))
            {
                user.Description = userDTO.Description;
            }
            if (!string.IsNullOrEmpty(userDTO.Phone))
            {
                user.Phone = userDTO.Phone;
            }
            user.UpdatedTime = DateTime.Now;
            return await _baseRepository.UpdateAsync(user);



        }

        public async Task<bool> AddEvevt(int userId, long evevtId)
        {
            EventUser eventUser = new EventUser();
            eventUser.EventID = evevtId;
            eventUser.UserID = userId;
            return await _eventUserRepository.CreateAsync(eventUser) != 0 ? true : false;

        }

        public async Task<List<User>> SelectByQuery(UserQueryParams queryParams)
        {
            //var predicate = GetPredicate(queryParams);
            var roleNames = queryParams.RoleNames;
            var userName = queryParams.UserName;

            // 构建基础查询条件
            var predicate = Expressionable.Create<User>()
                .AndIF(!string.IsNullOrEmpty(userName), it => it.UserName.Contains(userName))
                .ToExpression();

            // 查询用户
            var users = await _userRespository.SelectAllWithQueryAsync(predicate, queryParams.PageIndex, queryParams.PageSize);

            // 在内存中过滤用户
            if (!roleNames.IsNullOrEmpty())
            {
                users = users.Where(u =>
                  roleNames.All(roleName => u.RoleList.Any(r => r.RoleName == roleName)))
                  .ToList();
            }

            return users;
         
        }

        public async Task<long> Count(UserQueryParams queryParams)
        {
            try
            {
                // 基础 SQL 查询
                var sql = @"
            SELECT COUNT(DISTINCT u.Id)
            FROM t_user u
            LEFT JOIN t_userrole ur ON u.Id = ur.UserId
            LEFT JOIN t_role r ON ur.RoleId = r.RoleId
            WHERE 1 = 1";

                // 动态添加查询条件
                var parameters = new List<SugarParameter>();

                if (!string.IsNullOrEmpty(queryParams.UserName))
                {
                    sql += " AND u.UserName LIKE @UserName";
                    parameters.Add(new SugarParameter("@UserName", $"%{queryParams.UserName}%"));
                }

                if (!queryParams.RoleNames.IsNullOrEmpty())
                {
                    // 将 RoleNames 列表转换为逗号分隔的字符串
                    var roleNamesString = string.Join(",", queryParams.RoleNames.Select(r => $"'{r}'"));
                    sql += $" AND r.RoleName IN ({roleNamesString})";
                }
                //if (!queryParams.RoleNames.IsNullOrEmpty())
                //{
                //    sql += " AND r.RoleName IN @RoleNames";
                //    parameters.Add(new SugarParameter("@RoleNames", queryParams.RoleNames));
                //}

                // 使用 SQLSugar 执行查询
                var totalCount = await _userRespository. GetCountAsync(sql, parameters);

                return totalCount;
            }
            catch (Exception ex)
            {
                // 记录日志并抛出异常
                throw new ApplicationException("An error occurred while counting users.", ex);
            }
        }
        public Expression<Func<User, bool>> GetPredicate(UserQueryParams queryParams)
        {
            var predicate = Expressionable.Create<User>()
                .AndIF(!string.IsNullOrEmpty(queryParams.UserName), it => it.UserName.Contains(queryParams.UserName))
                .AndIF(!queryParams.RoleNames.IsNullOrEmpty(), it => it.RoleList.All(r => queryParams.RoleNames.Contains(r.RoleName)))
                .ToExpression(); // 生成最终的表达式
            return predicate;

        }

   

        //public async  Task<RegisterStatus> CreateAdminAsync(UserRegisterDTO userRegisterDTO)
        //{
        //    User newUser = _mapper.Map<User>(userRegisterDTO);
        //    string storedCode = await _redisDatabase.StringGetAsync(_redisPrefix + userRegisterDTO.Email);
        //    if (string.IsNullOrEmpty(storedCode) || !storedCode.Equals(userRegisterDTO.Code))
        //    {
        //        return RegisterStatus.EmailWrong;
        //    }
        //    try
        //    {
        //        newUser.PasswordHash = MD5Helper.GetMD5(userRegisterDTO.Password);
        //        var id = await _userRespository.CreateUser(newUser);
        //        Role role = await _roleRepository.SelectOneAsync(t => t.RoleName == "User");
        //        newUser.RoleList = [role];
        //        // 5. 插入多对多关系：将用户与角色关联
        //        var userRoleRelations = newUser.RoleList.Select(role => new UserRole
        //        {
        //            UserID = id,  // 注意：这里假设用户的 Id 已经生成
        //            RoleID = role.RoleID
        //        }).ToList();
        //        await _userroleRepository.CreateAsync(userRoleRelations);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RegisterStatus.Error;
        //    }
        //    _redisDatabase.KeyDelete(_redisPrefix + userRegisterDTO.Email);
        //    return RegisterStatus.Success;
        //}
    }

}
