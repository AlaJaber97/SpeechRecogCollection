using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL.Sql.Models
{
    public class TransData
    {
        public DateTime CreateTime { get; set; }
        public Guid ID { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public Guid AudioFileID { get; set; }
        public virtual AudioFile AudioFile { get; set; }
        public Guid TransTextID { get; set; }
        public virtual TransText TransText { get; set; }
        public string ReviewBy { get; set; }
        public bool IsValid { get; set; }
    }
}
