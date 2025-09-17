// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DalaProject.Models;
using DalaProject.Data;

namespace DalaProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.MarketProducts
                .Include(p => p.Fermer)
                .OrderByDescending(p => p.PublishDate)
                .Take(8)
                .ToListAsync();

            return View(products);
        }
    }
}