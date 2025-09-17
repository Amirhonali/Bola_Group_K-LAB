using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}