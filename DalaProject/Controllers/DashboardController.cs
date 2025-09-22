using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DalaProject.Data;

namespace DalaProject.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        // --- Главная страница личного кабинета ---
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Owner")
            {
                var companies = await _db.Companies
                    .Include(c => c.Grounds)
                    .Where(c => c.OwnerId == userId)
                    .ToListAsync();

                return View("OwnerDashboard", companies);
            }
            else if (role == "Fermer")
            {
                var grounds = await _db.Grounds
                    .Include(g => g.Company)
                    .Where(g => g.FermerId == userId)
                    .ToListAsync();

                return View("FermerDashboard", grounds);
            }

            return RedirectToAction("AccessDenied", "Account");
        }
    }
}