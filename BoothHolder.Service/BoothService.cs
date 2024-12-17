using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using BoothHolder.Repository.Impl;
using MapsterMapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public  async Task<long> Count()
        {
            return _boothRepository.GetCount();
        }

        public Task<List<Booth>> SelectByQuery(BoothQueryParams queryParams)
        {
            // 创建基本的表达式
            var predicate = Expressionable.Create<Booth>()
                .And(it => !it.IsDeleted)
                .AndIF(!string.IsNullOrEmpty(queryParams.BoothName), it => it.BoothName.Contains(queryParams.BoothName))
                .AndIF(!string.IsNullOrEmpty(queryParams.Location), it => it.Location.Contains(queryParams.Location))
                .AndIF(!string.IsNullOrEmpty(queryParams.BrandType), it => it.BrandType.BrandTypeName.Contains(queryParams.BrandType)) // 假设BrandType存储在Description中
                .AndIF(queryParams.MinPrice.HasValue, it => it.DailyRate >= queryParams.MinPrice)
                .AndIF(queryParams.MaxPrice.HasValue, it => it.DailyRate <= queryParams.MaxPrice)
                .AndIF(queryParams.RentalStartDate.HasValue, it => it.AvailableDate >= queryParams.RentalStartDate)
                .AndIF(queryParams.RentalEndDate.HasValue, it => it.AvailableDate <= queryParams.RentalEndDate)
                .AndIF(queryParams.IsAvailable, it => it.IsAvailable)
                .ToExpression(); // 生成最终的表达式
           return _boothRepository.SelectAllWithBrandTypeAsync(predicate,  queryParams.PageIndex,queryParams.PageSize);
            

        }
    }
}
