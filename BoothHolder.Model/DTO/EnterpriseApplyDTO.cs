using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.DTO
{
    public class EnterpriseApplyDTO
    { /// <summary>
      /// 企业名称（必填）
      /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 企业联系电话（必填）
        /// </summary>
        public string ContactPhone { get; set; }
        public string RemarkSupport { get; set; }
    }
    public class EnterpriseApplicationParams
    {
        public string? EnterpriseName { get; set; }
        public  string? Status { get; set; }

        public int PageIndex { get; set; } = 0;  // 默认第一页
        public int PageSize { get; set; } = 30;  // 默认每页10条
    }

    public record RemarkQuery(long Id, int Status, string? Remark);

}
