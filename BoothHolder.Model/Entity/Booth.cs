using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_booth")]
    public class Booth
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; } // 摊位ID

        [SugarColumn(Length = 50, IsNullable = false)]
        public string BoothName { get; set; } // 摊位名称

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Location { get; set; } // 摊位位置

        [SugarColumn(IsNullable = false)]
        public decimal DailyRate { get; set; } // 每日租金

        [SugarColumn(IsNullable = false)]
        public bool IsAvailable { get; set; } = true; // 是否可用
        [SugarColumn]
        public string? MediaUrl { get; set; }  // 密码哈希值
        [SugarColumn(IsNullable = false)]

        public long BrandTypeId { get; set; } // 品牌类型
        [SugarColumn(IsNullable = false)]

        public DateTime AvailableDate { get; set; } // 招租日期

        [SugarColumn(IsNullable = true)]
        public string Description { get; set; } // 摊位描述
                                                // 添加 IsDeleted 字段，默认值为 false
        [SugarColumn(IsNullable = false)]
        public bool IsDeleted { get; set; } = false; // 是否已删除，默认 false

        [Navigate(NavigateType.OneToMany, nameof(Reservation.BoothId))]
        public List<Reservation> Reservations { get; set; } // 关联的预定信息
        [Navigate(NavigateType.ManyToOne, nameof(BrandTypeId), nameof(BrandType.Id))]
        public BrandType BrandType { get; set; } // 导航属性
    }
}
