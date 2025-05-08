using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_complaint")]
    public class Complaint
    {
        
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Title { get; set; }

        [SugarColumn(ColumnDataType = "text", IsNullable = false)]
        public string Content { get; set; }

        [SugarColumn(IsNullable = true)]
        public int? UserId { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Navigate(NavigateType.ManyToOne, nameof(UserId))]
        public User Reporter { get; set; }

    }
}

