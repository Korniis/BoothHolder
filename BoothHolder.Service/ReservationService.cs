using BoothHolder.Common.Response;
using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using BoothHolder.Repository;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BoothHolder.Service
{
    public class ReservationService : BaseService<Reservation, Reservation>, IReservationService
    {
        IBaseRepository<Reservation> _repository;
        IBoothRepository _boothRepository;
        IReservationRepository _reservationRepository;
        IMapper _mapper;
        public ReservationService(IBaseRepository<Reservation> repository, IReservationRepository reservationRepository, IMapper mapper, IBoothRepository boothRepository) : base(repository, mapper)
        {
            _repository = repository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _boothRepository = boothRepository;
        }
        public async Task<int> AddAsync(ReservationRequestDTO request)
        {
            var reservation = new Reservation
            {
                BoothId = request.BoothId,
                UserId = request.UserId,
                Status = (int)ReservationStatus.Completed, // 直接设为已确认
                ContactName = request.ContactName,
                Phone = request.Phone,
                Description = request.Description,
                StartDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                ChangedAt = DateTime.Now // 记录确认时间
            };
            var resid = await _reservationRepository.CreateAsync(reservation);
            if (resid <= 0)
                return 0;
            return await _boothRepository.UpdateOnReservation(request.BoothId, request.UserId);
        }
        public async Task<decimal> CountPayments(long userId)
        {
            var reservation = await _reservationRepository.SelectOneAsync(r => r.UserId == userId && r.Status == (int)ReservationStatus.Completed);
            if (reservation == null)
            {
                throw new Exception("该用户没有已完成的预定记录");
            }
            var days = DateTime.Now -  reservation.StartDate ;
            if (days.Days<=30) return 0;
            var booth = await _boothRepository.SelectOneByIdAsync(reservation.BoothId);
            if (booth == null)
            {
                throw new KeyNotFoundException("关联的摊位信息不存在");
            }
            // 5. 计算超出30天的天数
            int excessDays = Math.Max(0, days.Days-30); // 确保天数不为负

            // 6. 计算应付金额（每日租金 × 超额天数）
            decimal paymentDue = booth.DailyRate * excessDays;
            return paymentDue;
        }
        public async Task<bool> ExistsConflictReservation(long boothId, DateTime startDate, DateTime endDate)
        {
            return await _reservationRepository.ExistsConflictAsync(boothId, startDate, endDate);
        }
        public async Task<bool> ExistsConflictUser(long userId)
        {
            return await _reservationRepository.ExistsConflictUserAsync(userId);
        }

        //public async Task<Reservation> GetByUserIdAsync(long userId)
        //{
        //    return 
        //}

        public async Task<int> RemoveReservationAsync(long userId)
        {
            Reservation reservation = await _reservationRepository.SelectOneAsync(r => r.UserId == userId&&r.Status==1);
            if (reservation == null || reservation.UserId != userId)
            {
                return 0;
            }
            try
            {
                reservation.Status = (int)ReservationStatus.Canceled;
                reservation.EndDate = DateTime.Now;
                reservation.ChangedAt = DateTime.Now;

                await _reservationRepository.UpdateAsync(reservation);

                var booth = await _boothRepository.SelectOneAsync(c => c.Id == reservation.BoothId);
                booth.IsAvailable = true;
                booth.UserId = null;
                booth.AvailableDate = DateTime.Now;

                await _boothRepository.UpdateAsync(booth);
            }
            catch (Exception ex) {
            
            }
            return 1;
        }
    }
}
