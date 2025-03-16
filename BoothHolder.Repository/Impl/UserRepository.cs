using BoothHolder.Model.Entity;
using SqlSugar;
using System.Linq.Expressions;

namespace BoothHolder.Repository.Impl
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ISqlSugarClient _db;


        public UserRepository(ISqlSugarClient db) : base(db)
        {

            _db = db;
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _db.Queryable<Role>().Includes(x => x.UserList, u => u.RoleList).ToListAsync();
        }

        public async Task<List<User>> GetUsers()
        {

            return await _db.Queryable<User>().Includes(x => x.RoleList).ToListAsync();
        }
        public async Task<int> CreateUser(User user)
        {
            return
                      await _db.Insertable(user)
                          // 包括 RoleList
                          .ExecuteReturnIdentityAsync();
        }

        public async Task<User> SelectOneWithRoleAsync(Expression<Func<User, bool>> value)
        {
            return await _db.Queryable<User>()
                  .Includes(x => x.RoleList)
                 .SingleAsync(value);
        }

        public async Task<List<User>> SelectAllWithQueryAsync(Expression<Func<User, bool>> predicate, int pageIndex, int pageSize)
        {
         var total = await _db.Queryable<User>().CountAsync(predicate);
            return await _db.Queryable<User>().Includes(c=>c.RoleList).Where(predicate)
          .Skip(pageIndex * pageSize) // 跳过前面的记录
           .Take(pageSize) // 获取当前页的记录
           .ToListAsync();
        }

        public async Task<long> GetCountAsync(Expression<Func<User, bool>> predicate)
        {
            return await _db.Queryable<User>().CountAsync(predicate);
        }
    }
}
