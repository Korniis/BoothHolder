﻿namespace BoothHolder.Model.VO
{
    public class BoothVO
    {
        public long Id { get; set; } // 摊位ID
        public string BoothName { get; set; } // 摊位名称
        public string Location { get; set; } // 摊位位置
        public string? MediaUrl { get; set; }  // 密码哈希值

        public decimal DailyRate { get; set; } // 每日租金
        public DateTime AvailableDate { get; set; } // 招租日期
        public string Description { get; set; } // 摊位描述
        public string BrandTypeName { get; set; } // 品牌类型名称（可以用 BrandType 的名称来简化信息）

    }
}