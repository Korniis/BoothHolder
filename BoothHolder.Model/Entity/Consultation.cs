using BoothHolder.Model.Status;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.Entity
{
    [SugarTable("t_consultation")]
    public class Consultation
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        [SugarColumn(IsNullable = false)]
        public long SenderId { get; set; }
        [SugarColumn(IsNullable = false)]
        public long ReceiveId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string? Recall { get; set; }
        public int  Status { get; set; }= ((int)ConsultationStatus.Pending);

        public DateTime UpdateTime { get; set; } = DateTime.Now;

    }
 
}
