using DalaProject.Models;

namespace DalaProject.Models;

public class OwnerFermer
{
    public int Id { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public int FermerId { get; set; }
    public User Fermer { get; set; } = null!;

    public string Status { get; set; } = "Pending"; // Pending / Accepted / Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
