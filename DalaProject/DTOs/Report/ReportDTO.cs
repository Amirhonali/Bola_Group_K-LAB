using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Report
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public int FermerId { get; set; }
        public decimal Expenses { get; set; }
        public decimal Income { get; set; }
        public double QuantityProduced { get; set; }
        public double QuantitySold { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}