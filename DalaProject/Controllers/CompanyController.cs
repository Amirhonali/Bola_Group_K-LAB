using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DalaProject.Models;
using DalaProject.Data;

namespace DalaProject.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _db;
        public CompanyController(AppDbContext db) { _db = db; }

        // --- Просмотр списка компаний ---
        [Authorize(Roles = "Owner, Fermer")]
        public async Task<IActionResult> Index()
        {
            var companies = await _db.Companies
                                     .Include(c => c.Owner)
                                     .ToListAsync();
            return View(companies);
        }

        // --- Детали компании ---
        [Authorize(Roles = "Owner, Fermer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var company = await _db.Companies
                                    .Include(c => c.Grounds)
                                    .ThenInclude(g => g.Fermer)   // <-- вот это добавь
                                    .Include(c => c.Owner)
                                    .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        // --- Создание компании ---
        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company, IFormFile? imageFile)
        {
            ModelState.Remove("OwnerId");
            ModelState.Remove("Owner");
            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
                return View(company);

            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            company.OwnerId = ownerId;

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            // --- сохраняем картинку ---
            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{company.Id}{ext}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Company", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                company.ImageUrl = $"Images/Company/{fileName}";
                _db.Update(company);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // --- Редактирование компании ---
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var company = await _db.Companies.FindAsync(id);
            if (company == null) return NotFound();

            return View(company);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company updatedCompany, IFormFile? imageFile)
        {
            if (id != updatedCompany.Id) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var ownerId))
                return Forbid();

            var company = await _db.Companies.FindAsync(id);
            if (company == null) return NotFound();

            if (company.OwnerId != ownerId)
                return Forbid();

            // обновляем поля
            company.Name = updatedCompany.Name;
            company.Description = updatedCompany.Description;

            // если загружен новый файл
            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{company.Id}{ext}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Company", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                company.ImageUrl = $"/Images/Company/{fileName}";
            }

            if (ModelState.IsValid)
            {
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedCompany);
        }

        // --- Удаление компании ---
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = await _db.Companies
                                   .Include(c => c.Owner)
                                   .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Grounds) // Загружаем земли
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company != null)
            {
                // --- удаляем фото компании ---
                if (!string.IsNullOrEmpty(company.ImageUrl))
                {
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        company.ImageUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // --- удаляем фото всех земель этой компании ---
                foreach (var ground in company.Grounds)
                {
                    if (!string.IsNullOrEmpty(ground.ImageUrl))
                    {
                        var groundFilePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            ground.ImageUrl.TrimStart('/')
                        );

                        if (System.IO.File.Exists(groundFilePath))
                        {
                            System.IO.File.Delete(groundFilePath);
                        }
                    }
                }

                // Удаляем саму компанию (EF удалит Grounds по каскаду, если FK настроен)
                _db.Companies.Remove(company);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}