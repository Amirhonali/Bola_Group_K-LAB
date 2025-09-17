using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Company
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OwnerId { get; set; }
        public string? Description { get; set; }
    }
}