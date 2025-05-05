using BoothHolder.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.IService
{
    public interface ICommentService : IBaseService<Comment, Comment>
    {
        Task<List<Comment>> SelectAllWithUserAsync(long boothId, long enterpriseId);
    }
}
