using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.IService
{
    public interface IEnterpriseApplicationService : IBaseService<EnterpriseApplication, EnterpriseApplyDTO>
    {
        Task<int> ApplyEnterprise(long userId, EnterpriseApplyDTO enterpriseApplyDTO);
        Task<int> EnterpriseAgain(int userId, EnterpriseApplyDTO enterpriseApplyDTO);
        Task<List<EnterpriseApplication>> SelectByQuery(EnterpriseApplicationParams queryParams);
    }
}
