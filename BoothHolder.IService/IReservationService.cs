using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;

namespace BoothHolder.IService
{
    public interface IReservationService : IBaseService<Reservation, Reservation>
    {
        Task<int> AddAsync( ReservationRequestDTO reservation);
        Task<double >CountPayments(long userId);
        Task<bool> ExistsConflictReservation(long boothId, DateTime startDate, DateTime endDate);
        Task<bool> ExistsConflictUser(long userId);
    }
}