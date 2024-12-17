using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_payment")]
    public class Payment
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        public long Id { get; set; } // 支付记录ID

        [SugarColumn(IsNullable = false)]
        public long ReservationId { get; set; } // 预定ID

        [SugarColumn(IsNullable = false)]
        public decimal Amount { get; set; } // 支付金额

        [SugarColumn(IsNullable = false)]
        public DateTime PaymentDate { get; set; } = DateTime.Now; // 支付时间

        [SugarColumn(Length = 20, IsNullable = false)]
        public string Status { get; set; } = "Paid"; // 支付状态：Paid/Failed

        [Navigate(NavigateType.ManyToOne, nameof(ReservationId))]
        public Reservation Reservation { get; set; } // 关联的预定记录
                                                     // 添加 IsDeleted 字段，默认值为 false
        [SugarColumn(IsNullable = false)]
        public bool IsDeleted { get; set; } = false; // 是否已删除，默认 false
    }

}
