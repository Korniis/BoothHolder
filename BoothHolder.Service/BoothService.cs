using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using BoothHolder.Repository;
using MapsterMapper;
using Newtonsoft.Json;
using SqlSugar;
using System.Linq.Expressions;

namespace BoothHolder.Service
{
    public class BoothService : BaseService<Booth, BoothDTO>, IBoothService
    {
        private readonly IBoothRepository _boothRepository;
        private readonly IBaseRepository<Booth> _baseRepository;
        private readonly IEnterpriseApplicationRepository _enterpriseApplicationRepository;
        private readonly IMapper _mapper;



        public BoothService(IBaseRepository<Booth> baseRepository, IMapper mapper, IBoothRepository boothRepository, IEnterpriseApplicationRepository enterpriseApplicationRepository) : base(baseRepository, mapper)
        {
            _boothRepository = boothRepository;
            _baseRepository = baseRepository;
            _mapper = mapper;
            _enterpriseApplicationRepository = enterpriseApplicationRepository;
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
                .AndIF(!string.IsNullOrEmpty(queryParams.BoothName), it => it.BoothName.Contains(queryParams.BoothName))
                .AndIF(!string.IsNullOrEmpty(queryParams.Location), it => it.Location.Contains(queryParams.Location))
                .AndIF(queryParams.BrandType.HasValue, it => it.BrandTypeId == queryParams.BrandType) // 假设BrandType存储在Description中
                .AndIF(queryParams.MinPrice.HasValue, it => it.DailyRate >= queryParams.MinPrice)
                .AndIF(queryParams.MaxPrice.HasValue, it => it.DailyRate <= queryParams.MaxPrice)
                .AndIF(queryParams.RentalStartDate.HasValue, it => it.AvailableDate >= queryParams.RentalStartDate)
                .AndIF(queryParams.RentalEndDate.HasValue, it => it.AvailableDate <= queryParams.RentalEndDate)
                .AndIF(queryParams.IsAvailable.HasValue, it => it.IsAvailable==queryParams.IsAvailable)
               // .And( it => it.IsDeleted==queryParams.IsDeleted)

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
                if (!upbooth.IsAvailable)
                {
                    upbooth.UserId = null;
                }
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

        public async Task<Booth> GetByIdAsync(long boothId)
        {
            return await _boothRepository.SelectOneByIdAsync(boothId);
        }

   

        public async Task<int> UpdateBoothInfoAsync(long boothId, string v1, string? v2)=> await _boothRepository.UpdateInfoAsync(boothId , v1, v2);

        public string GetAiInfo()
        {
            List<Booth> booths = _boothRepository.SelectFullToAi();
             var  enterprises =  _enterpriseApplicationRepository.SelectAllObj();
            var boothinfo = booths.Select(x => new
            {
                x.Id,
                x.Location,
                x.BoothName,
                x.Description,
                UserDescription = x.User.Description,
                x.User.Email,
                UserId = x.User.Id,
                x.User.UserName,
                EnterpriseDes = enterprises.FirstOrDefault(e => e.UserId == x.User.Id)?.RemarkSupport
            });

            string info =  JsonConvert.SerializeObject(boothinfo);
                return info;
        }
    }
}
