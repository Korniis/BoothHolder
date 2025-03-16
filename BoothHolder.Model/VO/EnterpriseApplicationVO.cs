using BoothHolder.Model.Entity;
using BoothHolder.Model.Status;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BoothHolder.Model.VO
{
    public class EnterpriseApplicationVO
    {
        public long Id { get; set; }
        public string EnterpriseName { get; set; }  // 企业名称
        public string ContactPhone { get; set; }  // 联系电话
        public string Status { get; set; }
        public string? Remark { get; set; }  // 审核备注
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // 申请时间
        public DateTime? ReviewedAt { get; set; }  // 审核时间
        /// <summary>
        /// 审核人附加评论（例如：审核过程中的附加意见）
        /// </summary>
        public string? AuditComment { get; set; }
        /// <summary>
        /// 申请信息
        /// </summary
        public string? RemarkSupport { get; set; }
        public string ApplyUserName { get; set; }  // 申请人用户信息
        public User ReviewedUserName { get; set; }  // 申请人用户信息
    }
}
