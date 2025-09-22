using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DalaProject.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        private void PopulateSelectLists(Report? report = null)
        {
            var seasons = _context.Seasons
                .AsNoTracking()
                .Where(s => s != null && s.Name != null)
                .ToList();

            var products = _context.Products
                .AsNoTracking()
                .Where(p => p != null && p.Title != null)
                .ToList();

            ViewBag.Seasons = seasons.Any()
                ? new SelectList(seasons, "Id", "Name", report?.SeasonId)
                : new SelectList(new List<Season>());

            ViewBag.Products = products.Any()
                ? new SelectList(products, "Id", "Title", report?.ProductId)
                : new SelectList(new List<Product>());
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            IQueryable<Report> query = _context.Reports.Include(r => r.Fermer);

            if (role == "Fermer")
            {
                query = query.Where(r => r.FermerId == userId);
            }
            // Owner видит все отчёты

            var reports = await query.ToListAsync();
            return View(reports);
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Fermer)
                .Include(r => r.Season)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (role == "Fermer" && report.FermerId != userId)
                return Forbid();

            return View(report);
        }

        [Authorize(Roles = "Fermer")]
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Create(Report report)
        {
            report.FermerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            report.CreatedAt = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(report);
            return View(report);
        }

        // GET: Reports/Edit/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            PopulateSelectLists(report);
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int id, Report report)
        {
            if (id != report.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingReport = await _context.Reports.FindAsync(id);
                if (existingReport == null) return NotFound();

                existingReport.Title = report.Title;
                existingReport.Description = report.Description;
                existingReport.SeasonId = report.SeasonId;
                existingReport.ProductId = report.ProductId;
                existingReport.Expenses = report.Expenses;
                existingReport.Income = report.Income;
                existingReport.QuantityProduced = report.QuantityProduced;
                existingReport.QuantitySold = report.QuantitySold;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(report);
            return View(report);
        }

        // GET: Reports/Delete/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Fermer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (report.FermerId != userId) return Forbid();

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (report == null || report.FermerId != userId)
                return Forbid();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}