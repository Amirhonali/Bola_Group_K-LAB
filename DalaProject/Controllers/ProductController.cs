using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace DalaProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        public ProductController(AppDbContext db) { _db = db; }

        // Index with market-like features: sorting and search
        public async Task<IActionResult> Index(string? search, string? category, string? sort)
        {
            var q = _db.Products.Include(p => p.Fermer).Include(p => p.Season).AsQueryable();
            if (!string.IsNullOrEmpty(search)) q = q.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            if (!string.IsNullOrEmpty(category)) q = q.Where(p => p.Category == category);
            q = sort switch
            {
                "date_asc" => q.OrderBy(p => p.Season!.StartDate),
                "date_desc" => q.OrderByDescending(p => p.Season!.StartDate),
                _ => q.OrderByDescending(p => p.Id)
            };
            ViewData["Categories"] = new SelectList(await _db.Products.Select(p => p.Category).Distinct().ToListAsync());
            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _db.Products.Include(p => p.Fermer).Include(p => p.Season).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text");
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", product.FermerId);
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", product.SeasonId);
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", product.FermerId);
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", product.SeasonId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", product.FermerId);
            ViewData["SeasonId"] = new SelectList(_db.Seasons.Select(s => new { s.Id, s.Name }), "Id", "Name", product.SeasonId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _db.Products.Include(p => p.Fermer).Include(p => p.Season).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null) { _db.Products.Remove(product); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
