using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Sql.Models
{
    public class TransText
    {
        public DateTime CreateTime { get; set; }
        public Guid ID { get; set; }
        public string Text { get; set; }
        public Enum.Dialect Dialect { get; set; }
        public Enum.Status Status { get; set; }
    }
}
