using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Repository;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Service
{
    public class EnterpriseApplicationService : BaseService<EnterpriseApplication, EnterpriseApplyDTO>
        , IEnterpriseApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IEnterpriseApplicationRepository _enterpriseApplicationRepository;
        public EnterpriseApplicationService(IBaseRepository<EnterpriseApplication> repository, IMapper mapper, IEnterpriseApplicationRepository enterpriseApplicationRepository) : base(repository, mapper)
        {
            _mapper = mapper;
            _enterpriseApplicationRepository = enterpriseApplicationRepository;
        }



        public  async Task<int>  ApplyEnterprise(long userId, EnterpriseApplyDTO enterpriseApplyDTO)
        {

            


            EnterpriseApplication enterpriseApplication= new EnterpriseApplication() {UserId=userId,
                ContactPhone=enterpriseApplyDTO.ContactPhone,
                EnterpriseName= enterpriseApplyDTO.EnterpriseName};
            return await _enterpriseApplicationRepository.ApplyEnterpriseAsync(enterpriseApplication);
        }
    }
}
