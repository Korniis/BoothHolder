using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.Status
{
    public  class EnterpriseStatus
    {//待审核/审核通过/审核拒绝
        public static string Waiting = "待审核";
        public static string Passed = "审核通过";
        public static string Rejected = "审核拒绝";
    }
}
