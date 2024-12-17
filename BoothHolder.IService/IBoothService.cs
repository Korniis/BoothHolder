using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.IService
{
    public interface IBoothService : IBaseService<Booth, BoothDTO>
    {
        Task<long> Count();
        Task<List<Booth>> SelectByQuery(BoothQueryParams queryParams);
    }
}
