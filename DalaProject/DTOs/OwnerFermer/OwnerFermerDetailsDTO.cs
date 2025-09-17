using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DalaProject.DTOs.OwnerFermer
{
    public class OwnerFermerDetailsDTO
{
    public int Id { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Для UI
    public string OwnerName { get; set; } = null!;
    public string FermerName { get; set; } = null!;
}
}