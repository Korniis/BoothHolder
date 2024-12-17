using BoothHolder.Model.Entity;
using System.Linq.Expressions;

namespace BoothHolder.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<List<User>> GetUsers();
        Task<List<Role>> GetRoles();
        Task<int> CreateUser(User user);
        Task<User> SelectOneWithRoleAsync(Expression<Func<User, bool>> value);
    }
}
