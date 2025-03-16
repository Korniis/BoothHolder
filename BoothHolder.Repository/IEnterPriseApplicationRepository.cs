using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository
{
    public interface IEnterpriseApplicationRepository
    {
        Task<int> ApplyEnterpriseAsync(EnterpriseApplication application);
    }
}
