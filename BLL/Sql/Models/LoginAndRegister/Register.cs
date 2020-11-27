using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Sql.Models.LoginAndRegister
{
    public class Register
    {
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public Enum.Gender Gender { get; set; }

    }
}
