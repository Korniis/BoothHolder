using BoothHolder.Model.Status;
using BoothHolder.Model.VO;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_enterpriseapplication")]
    public class EnterpriseApplication
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public long UserId { get; set; }  // 申请人 ID

        [SugarColumn(Length = 255, IsNullable = false)]
        public string EnterpriseName { get; set; }  // 企业名称

        [SugarColumn(Length = 20, IsNullable = false)]
        public string ContactPhone { get; set; }  // 联系电话

        [SugarColumn(Length = 20, IsNullable = false)]
        public string Status { get; set; } = EnterpriseStatus.Waiting;  // 申请状态（待审核/审核通过/审核拒绝）

        [SugarColumn(IsNullable = true, ColumnDataType = "TEXT")]
        public string? Remark { get; set; }  // 审核备注

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // 申请时间

        [SugarColumn(IsNullable = true)]
        public DateTime? ReviewedAt { get; set; }  // 审核时间

        [SugarColumn(IsNullable = true)]
        public long? ReviewedBy { get; set; }  // 审核人 ID（管理员）

     
        
        [SugarColumn]
       
        public string? RemarkSupport { get; set; }
        [Navigate(NavigateType.OneToOne,nameof(UserId))]  // OneToOne 表示一对一关系
        public User ApplyUser { get; set; }  // 申请人用户信息
        [Navigate(NavigateType.OneToOne, nameof(ReviewedBy))]  // OneToOne 表示一对一关系
        public User ReviewedUser { get; set; }  // 申请人用户信息

    }

}
