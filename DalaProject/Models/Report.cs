using DalaProject.Models;

namespace DalaProject.Models
{
    public class Report
{
    public int Id { get; set; }
    public decimal Expenses { get; set; }  // расходы
    public decimal Income { get; set; }    // доход
    public double QuantityProduced { get; set; } // сколько кг произведено
    public double QuantitySold { get; set; }     // сколько кг продано

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public int FermerId { get; set; }
    public User Fermer { get; set; } = null!;
}
}