using BoothHolder.IService;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.Status;
using BoothHolder.Repository;
using BoothHolder.Repository.Impl;
using Mapster;
using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
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
        private readonly IBaseRepository<EnterpriseApplication> _repository;
        public EnterpriseApplicationService(IBaseRepository<EnterpriseApplication> repository, IMapper mapper, IEnterpriseApplicationRepository enterpriseApplicationRepository) : base(repository, mapper)
        {
            _mapper = mapper;
            _enterpriseApplicationRepository = enterpriseApplicationRepository;
            _repository = repository;
        }



        public async Task<int> ApplyEnterprise(long userId, EnterpriseApplyDTO enterpriseApplyDTO)
        {

            EnterpriseApplication enterpriseApplication = await _repository.SelectOneAsync(e => e.UserId == userId);
            if (enterpriseApplication == null)
            {
                enterpriseApplication = new EnterpriseApplication()
                {
                    UserId = userId,
                    ContactPhone = enterpriseApplyDTO.ContactPhone,
                    EnterpriseName = enterpriseApplyDTO.EnterpriseName
                     ,
                    RemarkSupport = enterpriseApplyDTO.RemarkSupport
                };
                return await _enterpriseApplicationRepository.ApplyEnterpriseAsync(enterpriseApplication);
            }
            else
            {
                return -1;
            }


        }

        public async Task<int> EnterpriseAgain(int userId, EnterpriseApplyDTO enterpriseApplyDTO)
        {
            EnterpriseApplication enterpriseApplication = await _repository.SelectOneAsync(e => e.UserId == userId);
            if (enterpriseApplication == null||enterpriseApplication.Status== EnterpriseStatus.Passed)
                return -1;
            enterpriseApplication.UserId = userId;
            enterpriseApplication.ContactPhone = enterpriseApplyDTO.ContactPhone;
            enterpriseApplication.EnterpriseName = enterpriseApplyDTO.EnterpriseName;
            enterpriseApplication.RemarkSupport = enterpriseApplyDTO.RemarkSupport;
            enterpriseApplication.Status =EnterpriseStatus.Waiting;
            return await _enterpriseApplicationRepository.UpdateEnterpriseAsync(enterpriseApplication);
            

        }

        public async Task<List<EnterpriseApplication>> SelectByQuery(EnterpriseApplicationParams queryParams)
        {

            var predicate = GetPredicate(queryParams);

            return  await _enterpriseApplicationRepository.SelectByQueryAsync(predicate);

        }
        public Expression<Func<EnterpriseApplication, bool>> GetPredicate(EnterpriseApplicationParams queryParams)
        {
            var predicate = Expressionable.Create<EnterpriseApplication>()
                .AndIF(!string.IsNullOrEmpty(queryParams.EnterpriseName), it => it.EnterpriseName.Contains(queryParams.EnterpriseName))
                .AndIF(!queryParams.Status.IsNullOrEmpty(), it =>  queryParams.Status.Contains(it.Status ))
                .ToExpression();
           
            return predicate;

        }
    }
}
