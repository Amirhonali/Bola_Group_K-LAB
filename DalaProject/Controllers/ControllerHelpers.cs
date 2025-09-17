using System;
using DalaProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DalaProject.Controllers
{
    public static class ControllerHelpers
   {
        public static async Task<List<SelectListItem>> GetOwnersSelectList(AppDbContext db)
    {
        return await Task.FromResult(db.Users.Where(u => u.Role == "Owner").Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.FullName }).ToList());
    }

    public static async Task<List<SelectListItem>> GetFermersSelectList(AppDbContext db)
    {
        return await Task.FromResult(db.Users.Where(u => u.Role == "Fermer").Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.FullName }).ToList());
    }
}
}

