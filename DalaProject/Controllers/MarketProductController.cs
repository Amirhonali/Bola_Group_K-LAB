using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DalaProject.Data;
using DalaProject.Models;
using System.Security.Claims;

namespace DalaProject.Controllers
{
    [Authorize]
    public class MarketProductController : Controller
    {
        private readonly AppDbContext _context;

        public MarketProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MarketProduct
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder)
        {
            var marketProducts = _context.MarketProducts
                .Include(mp => mp.Product)
                .Include(mp => mp.Fermer)
                .AsQueryable();

            // Сортировка
            marketProducts = sortOrder switch
            {
                "price_asc" => marketProducts.OrderBy(mp => mp.Price),
                "price_desc" => marketProducts.OrderByDescending(mp => mp.Price),
                "category" => marketProducts.OrderBy(mp => mp.Product!.Category),
                "latest" => marketProducts.OrderByDescending(mp => mp.PublishDate),
                _ => marketProducts.OrderByDescending(mp => mp.PublishDate)
            };

            var list = await marketProducts.ToListAsync();
            return View(list);
        }

        // GET: MarketProduct/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var mp = await _context.MarketProducts
                .Include(m => m.Product)
                .Include(m => m.Fermer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mp == null) return NotFound();

            return View(mp);
        }

        // GET: MarketProduct/Create
        [Authorize(Roles = "Fermer")]
        public IActionResult Create()
        {
            var products = _context.Products.ToList();
            ViewBag.Products = products;
            return View();
        }

        // POST: MarketProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Create(MarketProduct marketProduct)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Products = _context.Products.ToList();
                return View(marketProduct);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            marketProduct.FermerId = userId;
            marketProduct.PublishDate = DateTime.UtcNow;

            _context.MarketProducts.Add(marketProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: MarketProduct/Edit/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int id)
        {
            var mp = await _context.MarketProducts.FindAsync(id);
            if (mp == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (mp.FermerId != userId) return Forbid();

            ViewBag.Products = _context.Products.ToList();
            return View(mp);
        }

        // POST: MarketProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int id, MarketProduct marketProduct)
        {
            if (id != marketProduct.Id) return NotFound();

            var existing = await _context.MarketProducts.FindAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (existing == null || existing.FermerId != userId) return Forbid();

            if (ModelState.IsValid)
            {
                existing.Title = marketProduct.Title;
                existing.Price = marketProduct.Price;
                existing.ProductId = marketProduct.ProductId;
                existing.Description = marketProduct.Description;
                existing.Phone = marketProduct.Phone;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = _context.Products.ToList();
            return View(marketProduct);
        }

        // GET: MarketProduct/Delete/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Delete(int id)
        {
            var mp = await _context.MarketProducts
                .Include(m => m.Product)
                .Include(m => m.Fermer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mp == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (mp.FermerId != userId) return Forbid();

            return View(mp);
        }

        // POST: MarketProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mp = await _context.MarketProducts.FindAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (mp == null || mp.FermerId != userId) return Forbid();

            _context.MarketProducts.Remove(mp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}