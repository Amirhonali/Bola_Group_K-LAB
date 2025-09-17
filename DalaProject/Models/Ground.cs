using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DalaProject.Models;

namespace DalaProject.Models;

public class Ground
{
    public int Id { get; set; }
    public string Location { get; set; } = null!;
    public double Area { get; set; } // площадь земли в гектарах

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public int? FermerId { get; set; } // кто смотрит за землей
    public User? Fermer { get; set; }

    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}
