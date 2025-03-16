namespace BoothHolder.Model.VO
{
    public class BoothVO
    {
        public long Id { get; set; } // 摊位ID
        public string BoothName { get; set; } // 摊位名称
        public string Location { get; set; } // 摊位位置
        public string? MediaUrl { get; set; }  // 密码哈希值

        public decimal DailyRate { get; set; } // 每日租金
        public bool IsAvailable { get; set; }  // 是否可用
        public DateTime AvailableDate { get; set; } // 招租日期
        public string Description { get; set; } // 摊位描述
        public long BrandTypeId { get; set; } // 品牌类型
        public string BrandTypeName { get; set; } // 品牌类型名称（可以用 BrandType 的名称来简化信息）
        public string UserName {  get; set; }

    }
}
