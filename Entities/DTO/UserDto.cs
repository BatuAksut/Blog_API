using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } 
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
