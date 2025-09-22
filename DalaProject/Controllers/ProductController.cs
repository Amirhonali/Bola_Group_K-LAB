using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DalaProject.Data;
using DalaProject.Models;
using System.Security.Claims;

namespace DalaProject.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Fermer)
                .Include(p => p.Season)
                .ToListAsync();

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Fermer)
                .Include(p => p.Season)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Fermer")]
        public IActionResult Create()
        {
            ViewBag.Seasons = _context.Seasons.ToList();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [Authorize(Roles = "Fermer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var fermerId))
                    return Forbid();

                product.FermerId = fermerId;

                _context.Add(product);
                await _context.SaveChangesAsync();

                // Фото
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{product.Id}{Path.GetExtension(imageFile.FileName)}";
                    var uploadPath = Path.Combine(_environment.WebRootPath, "Images/Product");
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    product.ImageUrl = $"Images/Product/{fileName}";
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Seasons = _context.Seasons.ToList();
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Seasons = _context.Seasons.ToList();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [Authorize(Roles = "Fermer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();

            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null) return NotFound();

            // Берем фермерId из Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var fermerId))
                return Forbid();

            product.FermerId = fermerId;

            // Сохраняем старую картинку если новая не загружена
            product.ImageUrl = existingProduct.ImageUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // удаляем старое фото
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            var oldPath = Path.Combine(_environment.WebRootPath, existingProduct.ImageUrl);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        var fileName = $"{product.Id}{Path.GetExtension(imageFile.FileName)}";
                        var uploadPath = Path.Combine(_environment.WebRootPath, "Images/Product");
                        Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        product.ImageUrl = $"Images/Product/{fileName}";
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Seasons = _context.Seasons.ToList();
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Fermer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Season)
                .Include(p => p.Fermer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Fermer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // удаляем фото
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}