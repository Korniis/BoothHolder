using BoothHolder.Model.DTO;
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
    }
}
