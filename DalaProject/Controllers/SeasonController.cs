using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DalaProject.Controllers
{
    [Route("Season")]
    public class SeasonsController : Controller
    {
        private readonly AppDbContext _db;
        public SeasonsController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _db.Seasons.Include(s => s.Ground).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var season = await _db.Seasons.Include(s => s.Products).Include(s => s.Reports).FirstOrDefaultAsync(s => s.Id == id);
            if (season == null) return NotFound();
            return View(season);
        }
        public IActionResult Create()
        {
            ViewData["GroundId"] = new SelectList(_db.Grounds.Select(g => new { g.Id, g.Location }), "Id", "Location");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Season season)
        {
            if (ModelState.IsValid)
            {
                _db.Add(season);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroundId"] = new SelectList(_db.Grounds.Select(g => new { g.Id, g.Location }), "Id", "Location", season.GroundId);
            return View(season);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var season = await _db.Seasons.FindAsync(id);
            if (season == null) return NotFound();
            ViewData["GroundId"] = new SelectList(_db.Grounds.Select(g => new { g.Id, g.Location }), "Id", "Location", season.GroundId);
            return View(season);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Season season)
        {
            if (id != season.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(season);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroundId"] = new SelectList(_db.Grounds.Select(g => new { g.Id, g.Location }), "Id", "Location", season.GroundId);
            return View(season);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var season = await _db.Seasons.Include(s => s.Ground).FirstOrDefaultAsync(s => s.Id == id);
            if (season == null) return NotFound();
            return View(season);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _db.Seasons.FindAsync(id);
            if (season != null) { _db.Seasons.Remove(season); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}