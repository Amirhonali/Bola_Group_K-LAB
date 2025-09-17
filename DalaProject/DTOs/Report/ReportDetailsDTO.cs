using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.Report
{
    public class ReportDetailsDTO
{
    public int Id { get; set; }
    public decimal Expenses { get; set; }
    public decimal Income { get; set; }
    public double QuantityProduced { get; set; }
    public double QuantitySold { get; set; }
    public DateTime CreatedAt { get; set; }

    // Для UI
    public string SeasonName { get; set; } = null!;
    public string FermerName { get; set; } = null!;
}
}