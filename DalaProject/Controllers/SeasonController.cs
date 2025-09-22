using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DalaProject.Models;
using System.Security.Claims;
using DalaProject.Data;

namespace DalaProject.Controllers
{
    [Authorize]
    public class SeasonController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;


        public SeasonController(AppDbContext db, IWebHostEnvironment environment)
        {
            _environment = environment;
            _db = db;
        }
        
        // --- Просмотр всех сезонов (Owner и Fermer) ---
            [Authorize(Roles = "Owner,Fermer")]
        public async Task<IActionResult> Index()
        {
            var seasons = await _db.Seasons
                                   .Include(s => s.Ground)
                                   .ThenInclude(g => g.Company)
                                   .ToListAsync();
            return View(seasons);
        }

        // --- Детали сезона (Owner и Fermer) ---
        [Authorize(Roles = "Owner,Fermer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var season = await _db.Seasons
                                  .Include(s => s.Ground)
                                  .ThenInclude(g => g.Company)
                                  .FirstOrDefaultAsync(s => s.Id == id);

            if (season == null) return NotFound();
            return View(season);
        }

        // --- Создание сезона (только Fermer) ---
        [Authorize(Roles = "Fermer")]
        public IActionResult Create()
        {
            ViewBag.Grounds = _db.Grounds.Include(g => g.Company).ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Fermer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Season season)
        {
            if (ModelState.IsValid)
            {
                // Приведение дат к UTC
                season.StartDate = DateTime.SpecifyKind(season.StartDate, DateTimeKind.Utc);
                if (season.EndDate.HasValue)
                    season.EndDate = DateTime.SpecifyKind(season.EndDate.Value, DateTimeKind.Utc);

                _db.Seasons.Add(season);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Grounds = _db.Grounds.Include(g => g.Company).ToList();
            return View(season);
        }

        // --- Редактирование сезона (только Fermer) ---
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var season = await _db.Seasons.FindAsync(id);
            if (season == null) return NotFound();

            ViewBag.Grounds = _db.Grounds.Include(g => g.Company).ToList();
            return View(season);
        }

        [HttpPost]
        [Authorize(Roles = "Fermer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Season season)
        {
            if (id != season.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    season.StartDate = DateTime.SpecifyKind(season.StartDate, DateTimeKind.Utc);
                    if (season.EndDate.HasValue)
                        season.EndDate = DateTime.SpecifyKind(season.EndDate.Value, DateTimeKind.Utc);

                    _db.Update(season);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.Seasons.Any(s => s.Id == season.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Grounds = _db.Grounds.Include(g => g.Company).ToList();
            return View(season);
        }

        // --- Удаление сезона (только Fermer) ---
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var season = await _db.Seasons
                                  .Include(s => s.Ground)
                                  .ThenInclude(g => g.Company)
                                  .FirstOrDefaultAsync(s => s.Id == id);

            if (season == null) return NotFound();
            return View(season);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _db.Seasons
                .Include(s => s.Products) // чтобы сразу подтянуть продукты
                .FirstOrDefaultAsync(s => s.Id == id);

            if (season == null) return NotFound();

            // Удаляем фото всех продуктов этого сезона
            foreach (var product in season.Products.ToList())
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, product.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _db.Products.Remove(product);
            }

            // Теперь удаляем сам сезон
            _db.Seasons.Remove(season);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}