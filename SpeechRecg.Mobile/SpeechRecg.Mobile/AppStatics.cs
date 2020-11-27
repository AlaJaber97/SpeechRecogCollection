using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechRecg.Mobile
{
    public static class AppStatics
    {
        public static BLL.Sql.Models.User User => !string.IsNullOrEmpty(Token) ? BLL.Services.JWT.GetUser(Token) : null;
        public static string Token { get; set; } = null;
    }
}
