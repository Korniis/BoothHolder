using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_boothtype")]
    public class BrandType
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        [SugarColumn(IsNullable = false)]

        public string BrandTypeName { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<Booth> Booths { get; set; } // 导航属性

        public string? BrandDescription { get; set; }
    }
}
