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

        public long GetCount()
        {
            return _db.Queryable<Booth>().Count(it=> !it.IsDeleted);
        }

        public async Task<List<Booth>> SelectAllWithBrandTypeAsync(Expression<Func<Booth, bool>> predicate, int pageIndex, int pageSize)
        {
            return await _db.Queryable<Booth>().Includes(x => x.BrandType).Where(predicate)
                 .Skip(pageIndex * pageSize) // 跳过前面的记录
                  .Take(pageSize) // 获取当前页的记录
                  .ToListAsync();
        }
    }
}
