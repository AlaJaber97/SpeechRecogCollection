using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Sql.ViewModels
{
    public class AudioFile
    {
        public Guid ID { get; set; }
        public Enum.Status Status { get; set; }
        public string Path { get; set; }
        public TimeSpan Length { get; set; }
        public Enum.Gender SpeakerGender { get; set; }
        public int SpeakerAge { get; set; }
    }
}
