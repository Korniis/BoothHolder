using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.IService
{
    public interface IEnterpriseService : IBaseService<User, UserDTO>
    {
        Task<string> GetToken(UserLoginDTO loginDTO);
        Task<User> GetUserInfo(int userId);
    }
}
