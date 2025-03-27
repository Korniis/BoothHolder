using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoothHolder.Repository
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        /// <summary>
        /// 检查是否存在时间冲突的预约
        /// </summary>
        Task<bool> ExistsConflictAsync(long boothId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取指定展位的所有预约
        /// </summary>
        Task<List<Reservation>> GetByBoothIdAsync(long boothId);

        /// <summary>
        /// 获取指定时间段内的所有预约
        /// </summary>
        Task<List<Reservation>> GetByTimeRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 检查是否存在时间冲突的预约（排除指定ID）
        /// </summary>
        Task<bool> ExistsConflictExcludeSelfAsync(long boothId, DateTime startDate, DateTime endDate, long excludeId);
        Task<bool> ExistsConflictUserAsync(long userId);
    }
}