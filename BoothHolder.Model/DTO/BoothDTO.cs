namespace BoothHolder.Model.DTO
{
    public class BoothDTO
    {
        public long? Id { get; set; } // 摊位ID
        public string BoothName { get; set; } // 摊位名称
        public string Location { get; set; } // 摊位位置
        public decimal DailyRate { get; set; } // 每日租金
        public bool? IsAvailable { get; set; } // 是否可用
        public string? MediaUrl { get; set; }
        public DateTime AvailableDate { get; set; } // 招租日期
        public string? Description { get; set; } // 摊位描述
        public long? BrandTypeId { get; set; } // 品牌类型名称（可以用 BrandType 的名称来简化信息

    }
    public class BoothQueryParams
    {
        public string? BoothName { get; set; }  // 可为空
        public string? Location { get; set; }    // 可为空
        public int? BrandType { get; set; }   // 可为空

        public bool IsAvailable { get; set; } = false;
        public bool IsDesc { get; set; } = false;

        public decimal? MinPrice { get; set; }  // 可为空
        public decimal? MaxPrice { get; set; }  // 可为空
        public DateTime? RentalStartDate { get; set; }  // 可为空
        public DateTime? RentalEndDate { get; set; }    // 可为空
        // 分页参数
        public int PageIndex { get; set; } = 0;  // 默认第一页
        public int PageSize { get; set; } = 30;  // 默认每页10条
    }
}
