using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.Sql.Models.LoginAndRegister
{
    public class Login
    {
        [Required]
        public string Email { get; set; }
    }
}
