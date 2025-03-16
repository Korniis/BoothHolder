using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository.Impl
{
    public class EnterpriseApplicationRepository : BaseRepository<EnterpriseApplication>, IEnterpriseApplicationRepository
    {
        private readonly ISqlSugarClient _db;
        public EnterpriseApplicationRepository(ISqlSugarClient db) : base(db)
        {
            _db = db;
        }

        public async Task<int> ApplyEnterpriseAsync(EnterpriseApplication application)
        {
        return await _db.Insertable(application).ExecuteCommandAsync();
            
        }
    }
}
