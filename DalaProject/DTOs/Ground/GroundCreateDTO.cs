using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Ground
{
    public class GroundCreateDTO
    {
        public string Location { get; set; } = null!;
        public double Area { get; set; }
        public int CompanyId { get; set; }
        public int? FermerId { get; set; }
    }
}