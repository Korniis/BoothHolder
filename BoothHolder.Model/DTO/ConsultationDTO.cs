using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoothHolder.Model.DTO
{
    public class ConsultationDTO
    {

        public long? SenderId { get; set; }
        public long ReceiveId { get; set; }
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "描述长度不能超过500个字符")]
        public string? Description { get; set; }

    }
}
