using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.User
{
    public class UserRegisterDTO
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; // сырой пароль, сервис должен хешировать
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = "Fermer";
    }
}