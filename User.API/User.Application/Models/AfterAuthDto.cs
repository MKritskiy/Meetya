using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Models
{
    public class AfterAuthDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
