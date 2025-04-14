using BoothHolder.Common.Response;
using BoothHolder.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoothHolder.EnterpriseApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IBoothService _boothService;

        public ReservationController(IReservationService reservationService, IBoothService boothService)
        {
            _reservationService = reservationService;
            _boothService = boothService;
        }
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetReservationInfo()
        {

            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            try
            {
                // 1. Get active reservation for user
                var reservation = await _reservationService.SelectOneAsync(r =>
                    r.UserId == userId && r.Status == 1);

                if (reservation == null)
                {
                    return ApiResult.Success("您没有摊位预定");
                }

                // 2. Get booth information in parallel to optimize performance
                var booth =  await _boothService.SelectFullByIdAsync(reservation.BoothId);

                if (booth == null)
                {
                    return ApiResult.Error("Associated booth not found");
                }
                // 3. Calculate additional useful information
                var daysRented = (DateTime.Now - reservation.StartDate).TotalDays;
                var totalPayment = booth.DailyRate *(int)daysRented;

                // 4. Return optimized response structure
                return ApiResult.Success(new
                {
                    Reservation = new
                    {
                        Id = reservation.Id,
                        StartDate = reservation.StartDate,
                        EndDate = reservation.EndDate,
                        ContactName = reservation.ContactName,
                        Phone = reservation.Phone,
                        ReservationDescription = reservation.Description,
                        DaysRented = Math.Floor(daysRented),
                        TotalPayment = Math.Round(totalPayment, 2)
                    },
                    Booth = new
                    {
                        Id = booth.Id,
                        Name = booth.BoothName,
                        Location = booth.Location,
                        DailyRate = booth.DailyRate,
                        ImageUrl = booth.MediaUrl,
                        BoothDescription = booth.Description,
                        BrandType = booth.BrandType.BrandTypeName
                    }
                });
            }
            catch (Exception ex)
            {
                return ApiResult.Error("Failed to retrieve reservation information");
            }
        }

    }
}
