using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.MarketProduct
{
public class MarketProductDetailsDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishDate { get; set; }

    // Доп. для UI
    public string FermerName { get; set; } = null!;
    public string FermerPhone { get; set; } = null!;
    public string ProductTitle { get; set; } = null!;
    public string ProductCategory { get; set; } = null!;
}
}