using DalaProject.Data;
using DalaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DalaProject.Controllers
{
    public class GroundController : Controller
    {
        private readonly AppDbContext _db;
        public GroundController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index() => View(await _db.Grounds.Include(g => g.Company).Include(g => g.Fermer).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var ground = await _db.Grounds.Include(g => g.Seasons).Include(g => g.Fermer).FirstOrDefaultAsync(g => g.Id == id);
            if (ground == null) return NotFound();
            return View(ground);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CompanyId"] = new SelectList(_db.Companies.Select(c => new { c.Id, c.Name }), "Id", "Name");
            ViewData["FermerId"] = new SelectList(await ControllerHelpers.GetFermersSelectList(_db), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ground ground)
        {
            if (ModelState.IsValid)
            {
                _db.Add(ground);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_db.Companies.Select(c => new { c.Id, c.Name }), "Id", "Name", ground.CompanyId);
            ViewData["FermerId"] = new SelectList(await ControllerHelpers.GetFermersSelectList(_db), "Value", "Text", ground.FermerId);
            return View(ground);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ground = await _db.Grounds.FindAsync(id);
            if (ground == null) return NotFound();
            ViewData["CompanyId"] = new SelectList(_db.Companies.Select(c => new { c.Id, c.Name }), "Id", "Name", ground.CompanyId);
            ViewData["FermerId"] = new SelectList(await ControllerHelpers.GetFermersSelectList(_db), "Value", "Text", ground.FermerId);
            return View(ground);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ground ground)
        {
            if (id != ground.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _db.Update(ground);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_db.Companies.Select(c => new { c.Id, c.Name }), "Id", "Name", ground.CompanyId);
            ViewData["FermerId"] = new SelectList(await ControllerHelpers.GetFermersSelectList(_db), "Value", "Text", ground.FermerId);
            return View(ground);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ground = await _db.Grounds.Include(g => g.Company).FirstOrDefaultAsync(g => g.Id == id);
            if (ground == null) return NotFound();
            return View(ground);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ground = await _db.Grounds.FindAsync(id);
            if (ground != null) { _db.Grounds.Remove(ground); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}