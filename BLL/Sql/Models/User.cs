using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Sql.Models
{
    public class User : IdentityUser
    {
        public int Age { get; set; }
        public Enum.Gender Gender { get; set; }
    }
}
