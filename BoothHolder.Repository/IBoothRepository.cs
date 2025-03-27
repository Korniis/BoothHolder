using BoothHolder.Model.Entity;
using System.Linq.Expressions;

namespace BoothHolder.Repository
{
    public interface IBoothRepository : IBaseRepository<Booth>
    {
        Task<bool> DeleteBoothAsyncById(long id);
        Task<long> GetCount(Expression<Func<Booth, bool>> predicate);
        Task<decimal> GetRevenue();
        Task<decimal> GetFullRevenue();
        Task<List<Booth>> SelectAllWithBrandTypeAsync(Expression<Func<Booth, bool>> predicate, int pageIndex, int pageSize);
        Task<Booth> SelectFullByIdAsync(long id);
        Task<int> UpdateOnReservation(long boothId, long userId);
    }
}
