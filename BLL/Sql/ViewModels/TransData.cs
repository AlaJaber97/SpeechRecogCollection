using BLL.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Sql.ViewModels
{
    public class TransData
    {
        public Guid ID { get; set; }
        public string UserID { get; set; }
        public virtual Models.User User { get; set; }
        public Guid AudioFileID { get; set; }
        public virtual ViewModels.AudioFile AudioFile { get; set; }
        public TypePost TypePost { get; set; }
        public Guid TransTextID { get; set; }
        public virtual Models.TransText TransText { get; set; }
    }
}
