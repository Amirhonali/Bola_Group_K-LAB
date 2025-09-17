using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DalaProject.Models;

namespace DalaProject.Models
{
    public class Season
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int GroundId { get; set; }
        public Ground Ground { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}