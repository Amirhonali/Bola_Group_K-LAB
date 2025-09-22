using System;
using System.ComponentModel.DataAnnotations.Schema;
using DalaProject.Models;

namespace DalaProject.Models
{

    public class MarketProduct
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        // Link to Product (optional)
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Seller
        public int FermerId { get; set; }
        public User? Fermer { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string Phone { get; set; } = null!; // seller phone to contact
    }
}