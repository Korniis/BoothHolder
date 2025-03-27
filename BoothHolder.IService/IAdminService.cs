 using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;

namespace BoothHolder.IService
{
    public interface IAdminService : IBaseService<User, UserDTO>
    {

        Task<string> GetToken(UserLoginDTO loginDTO);
        Task<User> GetUserInfo(int userId); 
    }
}
