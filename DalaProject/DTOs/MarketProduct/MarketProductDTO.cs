using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.MarketProduct
{
    public class MarketProductDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int FermerId { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime PublishDate { get; set; }
    }
}