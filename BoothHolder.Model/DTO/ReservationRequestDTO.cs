using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.DTO
{
    public class ReservationRequestDTO
    {
        [Required(ErrorMessage = "摊位ID不能为空")]
        public long BoothId { get; set; }

        [Required(ErrorMessage = "用户ID不能为空")]
        public long UserId { get; set; }

        //[Required(ErrorMessage = "开始日期不能为空")]
        //public DateTime StartDate { get; set; }

        //[Required(ErrorMessage = "结束日期不能为空")]
        //public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "联系人姓名不能为空")]
        [StringLength(50, ErrorMessage = "联系人姓名长度不能超过50个字符")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "联系人方式不能为空")]
        public string Phone { get; set; }

        [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
        public string Description { get; set; }
    }
    public enum ReservationStatus
    {
        Pending,    // 待确认
        Completed ,  // 已完成
        Canceled,   // 已取消
     
    }

    public enum PaymentStatus
    {
        Unpaid,     // 未支付
        Paid,       // 已支付
        Refunded    // 已退款
    }
}
