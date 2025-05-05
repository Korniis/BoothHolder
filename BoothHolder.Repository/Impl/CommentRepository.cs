using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Repository.Impl
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly ISqlSugarClient _db;
        public CommentRepository(ISqlSugarClient db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Comment>> SelectAllWithUserAsync(long boothId, long enterpriseId)
        {
            return await  _db.Queryable<Comment> ().Where(c=>c.IsDeleted==false).Includes(c=>c.CommentUser)
                .Where(c=>c.BoothId==boothId&&c.EnterpriseId==enterpriseId).ToListAsync();
        }
    }
}
