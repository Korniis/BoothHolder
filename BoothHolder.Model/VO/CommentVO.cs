using BoothHolder.Model.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BoothHolder.Model.VO
{
    public class CommentVO
    {
        /// <summary>
        /// 评论ID（主键，自增）
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
 
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}
