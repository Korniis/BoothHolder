using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

     

        public async Task<int> UpdateEnterpriseAsync(EnterpriseApplication enterpriseApplication)
        {
            return await  _db.Updateable(enterpriseApplication).ExecuteCommandAsync();
        }
        public async Task<List<EnterpriseApplication>> SelectByQueryAsync(Expression<Func<EnterpriseApplication, bool>> predicate)
        {
            return  await _db.Queryable<EnterpriseApplication>().Includes(it=>it.ReviewedUser).Includes(ea=>ea.ApplyUser).Where(predicate).ToListAsync();
        }

        public async Task<long> CountAsync(Expression<Func<EnterpriseApplication, bool>> predicate)
        {
            return await _db.Queryable<EnterpriseApplication>().Where(predicate).CountAsync();
        }
    }
}
