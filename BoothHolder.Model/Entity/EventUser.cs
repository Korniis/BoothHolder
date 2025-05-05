using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_eventuser")]
    public class EventUser
    {

        [SugarColumn(IsPrimaryKey = true)]
        public long UserID { get; set; }  // 用户ID

        [SugarColumn(IsPrimaryKey = true)]
        public long EventID { get; set; }  // 角色ID
    }
}
