using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository.Impl
{
    public class BoothRepository : BaseRepository<Booth>, IBoothRepository
    {
        private readonly ISqlSugarClient _db;
        public BoothRepository(ISqlSugarClient db) : base(db)
        {
            _db = db;
        }

        public async Task< long> GetCount(Expression<Func<Booth, bool>> predicate)
        {
            return await _db.Queryable<Booth>().Where(it=> !it.IsDeleted).CountAsync(predicate);
        }

        public async Task<List<Booth>> SelectAllWithBrandTypeAsync(Expression<Func<Booth, bool>> predicate, int pageIndex, int pageSize)
        {
            var total = await _db.Queryable<Booth>().CountAsync(predicate);


            return await _db.Queryable<Booth>().Includes(x => x.BrandType).Where(it => !it.IsDeleted).Where(predicate)
                 .Skip(pageIndex * pageSize) // 跳过前面的记录
                  .Take(pageSize) // 获取当前页的记录
                  .ToListAsync();
        }

        public  async Task<Booth> SelectFullByIdAsync(long id)
        {
                  
            return await _db.Queryable<Booth>().Includes(x=>x.BrandType).InSingleAsync(id);
        }
    }
}
