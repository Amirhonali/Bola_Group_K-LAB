using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.OwnerFermer
{
    public class OwnerFermerDTO
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int FermerId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}