using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_reservation")]
    public class Reservation
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; } // 预定ID

        [SugarColumn(IsNullable = false)]
        public long BoothId { get; set; } // 摊位ID

        [SugarColumn(IsNullable = false)]
        public long UserId { get; set; } // 用户ID

        [SugarColumn(IsNullable = false)]
        public DateTime StartDate { get; set; } // 租赁开始日期

        [SugarColumn(IsNullable = false)]
        public DateTime? EndDate { get; set; } // 租赁结束日期

        [SugarColumn(Length = 20, IsNullable = false)]
        public int Status { get; set; } = 0; // 预定状态：Pending/Completed/Canceled

        [SugarColumn( IsNullable = false)]
        public string ContactName { get; set; }  // 预定状态：Pending/Paid/Canceled
        [SugarColumn( IsNullable = false)]
        public string Phone { get; set; } 
        [SugarColumn]
        public string? Description { get; set; } // 
        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // 创建时间
        [SugarColumn(IsNullable = true, OldColumnName = "ChangeAt")]
        public DateTime ChangedAt { get; set; } = DateTime.Now; // 创建时间
                                                                // 添加 IsDeleted 字段，默认值为 false
        [SugarColumn(IsNullable = false)]
        public bool IsDeleted { get; set; } = false; // 是否已删除，默认 false

        [Navigate(NavigateType.ManyToOne, nameof(BoothId))]
        public Booth Booth { get; set; } // 关联的摊位

        [Navigate(NavigateType.ManyToOne, nameof(UserId))]
        public User User { get; set; } // 关联的用户
    }
}