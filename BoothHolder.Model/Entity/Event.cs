using SqlSugar;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_event")]
    public class Event
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        [Navigate(typeof(EventUser), nameof(EventUser.EventID), nameof(EventUser.UserID))]//注意顺序
        public List<User> Users { get; set; }

    }
}
