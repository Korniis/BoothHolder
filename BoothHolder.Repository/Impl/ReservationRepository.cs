using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoothHolder.Repository;

namespace BoothHolder.Repository.Impl
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        private readonly ISqlSugarClient _db;

        public ReservationRepository(ISqlSugarClient db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// 检查是否存在时间冲突的预约
        /// </summary>
        public async Task<bool> ExistsConflictAsync(long boothId, DateTime startDate, DateTime endDate)
        {
            return await _db.Queryable<Reservation>()
                .Where(r => r.BoothId == boothId)
                .Where(r => r.StartDate < endDate && r.EndDate > startDate)
                .AnyAsync();
        }

        /// <summary>
        /// 获取指定展位的所有预约
        /// </summary>
        public async Task<List<Reservation>> GetByBoothIdAsync(long boothId)
        {
            return await _db.Queryable<Reservation>()
                .Where(r => r.BoothId == boothId)
                .OrderBy(r => r.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// 获取指定时间段内的所有预约
        /// </summary>
        public async Task<List<Reservation>> GetByTimeRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _db.Queryable<Reservation>()
                .Where(r => r.StartDate >= startDate && r.EndDate <= endDate)
                .OrderBy(r => r.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// 检查是否存在时间冲突的预约（排除指定ID）
        /// </summary>
        public async Task<bool> ExistsConflictExcludeSelfAsync(long boothId, DateTime startDate, DateTime endDate, long excludeId)
        {
            return await _db.Queryable<Reservation>()
                .Where(r => r.BoothId == boothId)
                .Where(r => r.Id != excludeId)
                .Where(r => r.StartDate < endDate && r.EndDate > startDate)
                .AnyAsync();
        }

        public async Task<bool> ExistsConflictUserAsync(long userId)
        {
            return await _db.Queryable<Reservation>().Where(r => r.UserId == userId)
                .Where(r=>r.Status==1)
               .AnyAsync();
        }
    }
}