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
                StartDate=DateTime.Now,
                CreatedAt = DateTime.Now,
                ChangedAt = DateTime.Now // 记录确认时间
            };
            var resid = await _reservationRepository.CreateAsync(reservation);
            if (resid <= 0)
                return 0;

        return   await _boothRepository.UpdateOnReservation(request.BoothId,request.UserId);


        }

        public async Task<bool> ExistsConflictReservation(long boothId, DateTime startDate, DateTime endDate)
        {
            return await _reservationRepository.ExistsConflictAsync(boothId, startDate, endDate);
        }

        public async Task<bool> ExistsConflictUser(long userId)
        {
            return await _reservationRepository.ExistsConflictUserAsync(userId);
        }


    }
}
