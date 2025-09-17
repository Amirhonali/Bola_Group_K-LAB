using System.Security.Claims;
using DalaProject.Data;
using DalaProject.DTOs.MarketProduct;
using DalaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Controllers
{
    public class MarketProductsController : Controller
    {
        private readonly AppDbContext _db;
        public MarketProductsController(AppDbContext db) { _db = db; }

        // Index with sorting and filtering support
        public async Task<IActionResult> Index(string? search, string? category, string? sortBy)
        {
            var q = _db.MarketProducts.Include(m => m.Fermer).Include(m => m.Product).AsQueryable();
            if (!string.IsNullOrEmpty(search)) q = q.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
            if (!string.IsNullOrEmpty(category)) q = q.Where(m => m.Category == category);
            q = sortBy switch
            {
                "price_asc" => q.OrderBy(m => m.Price),
                "price_desc" => q.OrderByDescending(m => m.Price),
                "date_asc" => q.OrderBy(m => m.PublishDate),
                _ => q.OrderByDescending(m => m.PublishDate)
            };
            ViewData["Categories"] = new SelectList(await _db.MarketProducts.Select(m => m.Category).Distinct().ToListAsync());
            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var mp = await _db.MarketProducts.Include(m => m.Fermer).Include(m => m.Product).FirstOrDefaultAsync(m => m.Id == id);
            if (mp == null) return NotFound();
            return View(mp);
        }

        public IActionResult Create()
        {
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text");
            ViewData["ProductId"] = new SelectList(_db.Products.Select(p => new { p.Id, p.Title }), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarketProduct mp)
        {
            if (ModelState.IsValid)
            {
                mp.PublishDate = DateTime.UtcNow;
                _db.Add(mp);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", mp.FermerId);
            ViewData["ProductId"] = new SelectList(_db.Products.Select(p => new { p.Id, p.Title }), "Id", "Title", mp.ProductId);
            return View(mp);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var mp = await _db.MarketProducts.FindAsync(id);
            if (mp == null) return NotFound();
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", mp.FermerId);
            ViewData["ProductId"] = new SelectList(_db.Products.Select(p => new { p.Id, p.Title }), "Id", "Title", mp.ProductId);
            return View(mp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarketProduct mp)
        {
            if (id != mp.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(mp);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FermerId"] = new SelectList(ControllerHelpers.GetFermersSelectList(_db).Result, "Value", "Text", mp.FermerId);
            ViewData["ProductId"] = new SelectList(_db.Products.Select(p => new { p.Id, p.Title }), "Id", "Title", mp.ProductId);
            return View(mp);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var mp = await _db.MarketProducts.Include(m => m.Fermer).Include(m => m.Product).FirstOrDefaultAsync(m => m.Id == id);
            if (mp == null) return NotFound();
            return View(mp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mp = await _db.MarketProducts.FindAsync(id);
            if (mp != null) { _db.MarketProducts.Remove(mp); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}