using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _db;
        public ReportController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index() => View(await _db.Reports.Include(r => r.Season).Include(r => r.Fermer).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var report = await _db.Reports.Include(r => r.Season).Include(r => r.Fermer).FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        public IActionResult Create()
        {
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name");
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Report report)
        {
            if (ModelState.IsValid)
            {
                report.CreatedAt = DateTime.UtcNow;
                _db.Add(report);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", report.SeasonId);
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", report.FermerId);
            return View(report);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var report = await _db.Reports.FindAsync(id);
            if (report == null) return NotFound();
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", report.SeasonId);
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", report.FermerId);
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Report report)
        {
            if (id != report.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(report);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", report.SeasonId);
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", report.FermerId);
            return View(report);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var report = await _db.Reports.Include(r => r.Season).Include(r => r.Fermer).FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _db.Reports.FindAsync(id);
            if (report != null) { _db.Reports.Remove(report); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}