using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DalaProject.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<Ground> Grounds { get; set; } = new List<Ground>();
}