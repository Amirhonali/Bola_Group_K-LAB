using System.Collections.Generic;

namespace DalaProject.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!; // "Owner" или "Fermer"
    public string? ImageUrl { get; set; }
    public ICollection<OwnerFermer> Owners { get; set; } = new List<OwnerFermer>();
    public ICollection<OwnerFermer> Fermers { get; set; } = new List<OwnerFermer>();
    public ICollection<MarketProduct> MarketProducts { get; set; } = new List<MarketProduct>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}