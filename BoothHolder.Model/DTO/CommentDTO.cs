using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.DTO
{
    public class CommentDTO
    {
        /// <summary>
        /// 企业ID（必填）
        /// </summary>
        [Required]
        public long EnterpriseId { get; set; }

        /// <summary>
        /// 展位ID（必填）
        /// </summary>
        [Required]
        public long BoothId { get; set; }

        ///// <summary>
        ///// 用户ID（必填）
        ///// </summary>
        //[Required]
        //public long UserId { get; set; }

        /// <summary>
        /// 评论内容（必填）
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }
    }
}
