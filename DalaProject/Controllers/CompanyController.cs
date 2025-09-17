// Controllers/CompanyController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DalaProject.Models;
using DalaProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DalaProject.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AppDbContext _db;
        public CompanyController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index() => View(await _db.Companies.Include(c => c.Owner).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var company = await _db.Companies.Include(c => c.Grounds).FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();
            return View(company);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["OwnerId"] = new SelectList(await ControllerHelpers.GetOwnersSelectList(_db), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _db.Add(company);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(await ControllerHelpers.GetOwnersSelectList(_db), "Value", "Text", company.OwnerId);
            return View(company);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var company = await _db.Companies.FindAsync(id);
            if (company == null) return NotFound();
            ViewData["OwnerId"] = new SelectList(await ControllerHelpers.GetOwnersSelectList(_db), "Value", "Text", company.OwnerId);
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(company);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(await ControllerHelpers.GetOwnersSelectList(_db), "Value", "Text", company.OwnerId);
            return View(company);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var company = await _db.Companies.Include(c => c.Owner).FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _db.Companies.FindAsync(id);
            if (company != null) { _db.Companies.Remove(company); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}