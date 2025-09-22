using System.Collections.Generic;

namespace DalaProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!; // напр. "Клубника"
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int FermerId { get; set; }
        public User? Fermer { get; set; } = null!;

        public int SeasonId { get; set; }
        public Season? Season { get; set; } = null!;
    }
}