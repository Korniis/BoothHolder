using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using SqlSugar;
using System.Linq.Expressions;

namespace BoothHolder.Repository.Impl
{
    public class BoothRepository : BaseRepository<Booth>, IBoothRepository
    {
        private readonly ISqlSugarClient _db;
        public BoothRepository(ISqlSugarClient db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> DeleteBoothAsyncById(long id)
        {
            return await _db.Deleteable<Booth>().In(id).ExecuteCommandHasChangeAsync();
        }

        public async Task<long> GetCount(Expression<Func<Booth, bool>> predicate)
        {
            return await _db.Queryable<Booth>().Where(it => !it.IsDeleted).CountAsync(predicate);
        }

        public async Task<decimal> GetRevenue()
        {
            return await _db.Queryable<Booth>().Where(it => !it.IsDeleted && it.IsAvailable).SumAsync(it => it.DailyRate);
        }

        public async Task<decimal> GetFullRevenue()
        {
            return await _db.Queryable<Booth>().Where(it => !it.IsDeleted).SumAsync(it => it.DailyRate);
        }

        public async Task<List<Booth>> SelectAllWithBrandTypeAsync(Expression<Func<Booth, bool>> predicate, int pageIndex, int pageSize)
        {
            var total = await _db.Queryable<Booth>().CountAsync(predicate);


            return await _db.Queryable<Booth>().Includes(u=>u.User).Includes(x => x.BrandType).Where(b=>!b.IsDeleted).Where(predicate)
                .OrderByDescending(b=>b.AvailableDate)
                 .Skip(pageIndex * pageSize) // 跳过前面的记录
                  .Take(pageSize) // 获取当前页的记录
                  .ToListAsync();
        }

        public async Task<Booth> SelectFullByIdAsync(long id)
        {

            return await _db.Queryable<Booth>().Includes(x => x.BrandType).InSingleAsync(id);
        }

        public async Task<int> UpdateOnReservation(long boothId,long userId)
        {
            return await _db.Updateable<Booth>().Where(b => b.Id == boothId).SetColumns(b => new Booth
            {
                IsAvailable = false,
                UserId = userId,

            }).ExecuteCommandAsync();
        }

 

        public async Task<int> UpdateInfoAsync(long boothId, string v1, string? v2)
        {
              return await _db.Updateable<Booth>().Where(b => b.Id == boothId).SetColumns(b => new Booth
            {
                BoothName = v1,
                Description = v2,

            }).ExecuteCommandAsync();
        }
    }
}
