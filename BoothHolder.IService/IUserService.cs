﻿using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;

namespace BoothHolder.IService
{
    public interface IUserService : IBaseService<User, UserDTO>
    {
        Task<List<UserDTO>> GetUsers();
        Task<List<RoleDTO>> GetRoles();
        Task<int> CreateUser(UserDTO user);
        Task<string> GetToken(UserLoginDTO userDTO);
        Task<RegisterStatus> Register(UserRegisterDTO userRegisterDTO);
        Task<bool> SendConfirmCode(string email, bool isRemake);
        Task<bool> SetAvatar(int userId, string avatarurl);
        Task<bool> UpdateUserAsync(UserDTO userDTO, long userId);
        Task<bool> AddEvevt(int userId, long evevtId);
        Task<List<User>> SelectByQuery(UserQueryParams queryParams);
        Task<long> Count(UserQueryParams queryParams);
        Task<string> GetUserEnterprise(int userId);

        // Task<RegisterStatus> CreateAdminAsync(UserRegisterDTO userRegisterDTO);
    }
}
