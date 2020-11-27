using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Sql.Models
{
    public class AudioFile
    {
        public DateTime CreateTime { get; set; }
        public Guid ID { get; set; }
        public string NameFile { get; set; }
        public string ReservedFor { get; set; }
        public Enum.Status Status { get; set; }
        public string Path { get; set; }
        public TimeSpan Length { get; set; }
        public Enum.Gender SpeakerGender { get; set; }
        public int SpeakerAge { get; set; }
    }
}
