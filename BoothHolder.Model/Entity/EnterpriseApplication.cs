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
        public string Status { get; set; } = "待审核";  // 申请状态（待审核/审核通过/审核拒绝）

        [SugarColumn(IsNullable = true, ColumnDataType = "TEXT")]
        public string? Remark { get; set; }  // 审核备注

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // 申请时间

        [SugarColumn(IsNullable = true)]
        public DateTime? ReviewedAt { get; set; }  // 审核时间

        [SugarColumn(IsNullable = true)]
        public long? ReviewedBy { get; set; }  // 审核人 ID（管理员）
    }
}
