using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository
{
    public interface IBoothRepository : IBaseRepository<Booth>
    {
        long GetCount();
        Task<List<Booth>> SelectAllWithBrandTypeAsync(Expression<Func<Booth, bool>> predicate, int pageIndex, int pageSize);
    }
}
