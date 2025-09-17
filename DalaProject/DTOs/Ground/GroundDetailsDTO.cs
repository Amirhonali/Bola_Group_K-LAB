using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Ground
{
    public class GroundDetailsDTO
    {
        public int Id { get; set; }
        public string Location { get; set; } = null!;
        public double Area { get; set; }
        public int CompanyId { get; set; }
        public int? FermerId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? FermerName { get; set; }
    }
}