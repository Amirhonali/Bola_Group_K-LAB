using DalaProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DalaProject.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        private readonly AppDbContext _db;

        public OwnerController(AppDbContext db)
        {
            _db = db;
        }

        // Кабинет владельца (список компаний и земель)
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var companies = await _db.Companies
                .Include(c => c.Grounds)
                .ThenInclude(g => g.Seasons)
                .Where(c => c.OwnerId == userId)
                .ToListAsync();

            return View(companies);
        }

        // Просмотр сезона и его отчётов
        public async Task<IActionResult> SeasonReports(int seasonId)
        {
            var season = await _db.Seasons
                .Include(s => s.Ground)
                .ThenInclude(g => g.Company)
                .Include(s => s.Reports)
                .ThenInclude(r => r.Fermer)
                .FirstOrDefaultAsync(s => s.Id == seasonId);

            if (season == null) return NotFound();

            // Проверка, принадлежит ли земля текущему владельцу
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (season.Ground.Company.OwnerId != userId)
                return Forbid();

            return View(season);
        }
    }
}