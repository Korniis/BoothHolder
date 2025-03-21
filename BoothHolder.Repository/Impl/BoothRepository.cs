﻿using BoothHolder.Model.Entity;
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


            return await _db.Queryable<Booth>().Includes(u=>u.User).Includes(x => x.BrandType).Where(it => !it.IsDeleted).Where(predicate)
                 .Skip(pageIndex * pageSize) // 跳过前面的记录
                  .Take(pageSize) // 获取当前页的记录
                  .ToListAsync();
        }

        public async Task<Booth> SelectFullByIdAsync(long id)
        {

            return await _db.Queryable<Booth>().Includes(x => x.BrandType).InSingleAsync(id);
        }
    }
}
