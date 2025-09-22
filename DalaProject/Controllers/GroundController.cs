using System.Security.Claims;
using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Controllers
{
    [Authorize(Roles = "Owner, Fermer")]
    public class GroundController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public GroundController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // --- Список всех земель ---
        public async Task<IActionResult> Index()
        {
            var grounds = await _db.Grounds
                                   .Include(g => g.Company)
                                   .Include(g => g.Fermer)
                                   .ToListAsync();
            return View(grounds);
        }

        // --- Детали земли ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ground = await _db.Grounds
                                  .Include(g => g.Company)
                                  .Include(g => g.Fermer)
                                  .Include(g => g.Seasons)
                                  .FirstOrDefaultAsync(g => g.Id == id);

            if (ground == null) return NotFound();

            return View(ground);
        }

        // --- Создание земли (только Owner) ---
        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            ViewBag.Companies = new SelectList(_db.Companies, "Id", "Name");
            ViewBag.Fermers = new SelectList(_db.Users.Where(u => u.Role == "Fermer"), "Id", "FullName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ground ground, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                _db.Add(ground);
                await _db.SaveChangesAsync();

                if (imageFile != null && imageFile.Length > 0)
                {
                    var ext = Path.GetExtension(imageFile.FileName);
                    var fileName = $"{ground.Id}{ext}";
                    var folder = Path.Combine(_env.WebRootPath, "Images/Ground");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var savePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    ground.ImageUrl = $"Images/Ground/{fileName}";
                    _db.Update(ground);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Companies = new SelectList(_db.Companies, "Id", "Name", ground.CompanyId);
            ViewBag.Fermers = new SelectList(_db.Users.Where(u => u.Role == "Fermer"), "Id", "FullName", ground.FermerId);
            return View(ground);
        }

        // --- Редактирование земли (Owner) ---
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ground = await _db.Grounds.FindAsync(id);
            if (ground == null) return NotFound();

            ViewBag.Companies = new SelectList(_db.Companies, "Id", "Name", ground.CompanyId);
            ViewBag.Fermers = new SelectList(_db.Users.Where(u => u.Role == "Fermer"), "Id", "FullName", ground.FermerId);
            return View(ground);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ground updatedGround, IFormFile? imageFile)
        {
            if (id != updatedGround.Id) return NotFound();

            var ground = await _db.Grounds.FindAsync(id);
            if (ground == null) return NotFound();

            ground.Location = updatedGround.Location;
            ground.Area = updatedGround.Area;
            ground.CompanyId = updatedGround.CompanyId;
            ground.FermerId = updatedGround.FermerId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{ground.Id}{ext}";
                var savePath = Path.Combine(_env.WebRootPath, "Images/Ground", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                ground.ImageUrl = $"Images/Ground/{fileName}";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // --- Удаление земли (Owner) ---
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ground = await _db.Grounds
                                  .Include(g => g.Company)
                                  .FirstOrDefaultAsync(g => g.Id == id);
            if (ground == null) return NotFound();

            return View(ground);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ground = await _db.Grounds.FindAsync(id);
            if (ground != null)
            {
                // Удаляем фото из wwwroot/CompanyImages
                if (!string.IsNullOrEmpty(ground.ImageUrl))
                {
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        ground.ImageUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Удаляем саму компанию
                _db.Grounds.Remove(ground);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}