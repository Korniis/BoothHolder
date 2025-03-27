using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;

namespace BoothHolder.IService
{
    public interface IBoothService : IBaseService<Booth, BoothDTO>
    {
        Task<long> Count(BoothQueryParams queryParams);
        Task<bool> DeleteBoothAsync(long id);
        Task<decimal> GetRevenue();
        Task<decimal> GetFullRevenue();
        Task<List<Booth>> SelectByQuery(BoothQueryParams queryParams);
        Task<Booth> SelectFullByIdAsync(long id);
        Task<bool> UpdateBoothAsync(BoothDTO booth);
        Task<Booth> GetByIdAsync(long boothId);
    }
}
