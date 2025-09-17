using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Season
{
    public class SeasonDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int GroundId { get; set; }
        public string GroundLocation { get; set; } = null!;
    }
}