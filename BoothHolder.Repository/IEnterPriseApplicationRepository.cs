﻿using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository
{
    public interface IEnterpriseApplicationRepository : IBaseRepository<EnterpriseApplication>
    {
        Task<int> ApplyEnterpriseAsync(EnterpriseApplication application);
        Task<long> CountAsync(Expression<Func<EnterpriseApplication, bool>> predicate);
        Task<List<EnterpriseApplication>> SelectByQueryAsync(Expression<Func<EnterpriseApplication, bool>> predicate);
        Task<int> UpdateEnterpriseAsync(EnterpriseApplication enterpriseApplication);
    }
}
