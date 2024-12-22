using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using MapsterMapper;
using SqlSugar;
using System.Linq.Expressions;

namespace BoothHolder.Service
{
    public class BoothService : BaseService<Booth, BoothDTO>, IBoothService
    {
        private readonly IBoothRepository _boothRepository;
        private readonly IBaseRepository<Booth> _baseRepository;
        private readonly IMapper _mapper;



        public BoothService(IBaseRepository<Booth> baseRepository, IMapper mapper, IBoothRepository boothRepository) : base(baseRepository, mapper)
        {
            _boothRepository = boothRepository;
            _baseRepository = baseRepository;
            _mapper = mapper;

        }

        public async Task<long> Count(BoothQueryParams queryParams)
        {
            var predicate = GetPredicate(queryParams);

            return await _boothRepository.GetCount(predicate);
        }

        public Task<List<Booth>> SelectByQuery(BoothQueryParams queryParams)
        {
            // 创建基本的表达式
            var predicate = GetPredicate(queryParams);
            return _boothRepository.SelectAllWithBrandTypeAsync(predicate, queryParams.PageIndex, queryParams.PageSize);


        }

        public Task<Booth> SelectFullByIdAsync(long id)
        {
            return _boothRepository.SelectFullByIdAsync(id);
        }


        public Expression<Func<Booth, bool>> GetPredicate(BoothQueryParams queryParams)
        {
            var predicate = Expressionable.Create<Booth>()
                .AndIF(queryParams.IsAvailable, it => !it.IsDeleted)
                .AndIF(!string.IsNullOrEmpty(queryParams.BoothName), it => it.BoothName.Contains(queryParams.BoothName))
                .AndIF(!string.IsNullOrEmpty(queryParams.Location), it => it.Location.Contains(queryParams.Location))
                .AndIF(queryParams.BrandType.HasValue, it => it.BrandTypeId == queryParams.BrandType) // 假设BrandType存储在Description中
                .AndIF(queryParams.MinPrice.HasValue, it => it.DailyRate >= queryParams.MinPrice)
                .AndIF(queryParams.MaxPrice.HasValue, it => it.DailyRate <= queryParams.MaxPrice)
                .AndIF(queryParams.RentalStartDate.HasValue, it => it.AvailableDate >= queryParams.RentalStartDate)
                .AndIF(queryParams.RentalEndDate.HasValue, it => it.AvailableDate <= queryParams.RentalEndDate)
                .AndIF(queryParams.IsAvailable, it => it.IsAvailable)
                .ToExpression(); // 生成最终的表达式
            return predicate;

        }

        public async Task<bool> UpdateBoothAsync(BoothDTO booth)
        {
            if (booth.Id.HasValue)
            {
                // Retrieve the existing booth from the repository using the ID
                var upbooth = await _boothRepository.SelectOneByIdAsync((long)booth.Id);

                if (upbooth == null)
                {
                    // If the booth does not exist, return false
                    return false;
                }

                // Update the properties of the existing booth with the data from the DTO
                upbooth.BoothName = booth.BoothName;
                upbooth.Location = booth.Location;
                upbooth.BrandTypeId = (long)booth.BrandTypeId;
                upbooth.DailyRate = booth.DailyRate;
                upbooth.AvailableDate = booth.AvailableDate;
                upbooth.Description = booth.Description;
                upbooth.IsAvailable = (bool)booth.IsAvailable;
                if (!string.IsNullOrEmpty(booth.MediaUrl))
                    upbooth.MediaUrl = booth.MediaUrl;

                // Save the updated booth to the repository
                var result = await _boothRepository.UpdateAsync(upbooth);

                return result; // Assuming UpdateAsync returns a boolean indicating success
            }
            else
            {
                // If the ID is not provided, return false
                return false;
            }
        }

        public async Task<bool> DeleteBoothAsync(long id)
        {
            var booth = await _boothRepository.SelectOneByIdAsync(id); ;


            booth.IsDeleted = true;
            return await _boothRepository.UpdateAsync(booth);
        }

        public async Task<decimal> GetRevenue()
        {
            return await _boothRepository.GetRevenue();
        }

        public async Task<decimal> GetFullRevenue()
        {
            return await _boothRepository.GetFullRevenue();

        }
    }
}
