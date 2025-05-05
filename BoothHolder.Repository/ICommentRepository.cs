using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<List<Comment>> SelectAllWithUserAsync(long boothId, long enterpriseId);
    }
}
