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
    }
}
