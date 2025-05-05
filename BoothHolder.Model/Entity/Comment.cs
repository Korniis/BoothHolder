using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;

namespace BoothHolder.Model.Entity
{
    /// <summary>
    /// 评论信息表
    /// </summary>
    [SugarTable("t_comment")]
    public class Comment
    {
        /// <summary>
        /// 评论ID（主键，自增）
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        [SugarColumn]
        public long EnterpriseId { get; set; }

        /// <summary>
        /// 展位ID
        /// </summary>
        [SugarColumn]
        public long BoothId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn]
        public long UserId { get; set; }



        /// <summary>
        /// 评论内容
        /// </summary>
        [SugarColumn(Length = 500)]
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        /// <summary>
        /// 是否删除（软删除标识）
        /// </summary>
        [SugarColumn]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 创建时间（UTC时间）
        /// </summary>
        [SugarColumn]
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        [Navigate(NavigateType.ManyToOne, nameof(UserId))]
        public User CommentUser  { get; set; }
    }
}