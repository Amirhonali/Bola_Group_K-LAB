// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DalaProject.Models;
using Microsoft.EntityFrameworkCore;
using DalaProject.Data;
using DalaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DalaProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string role)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email уже существует");
                    return View(model);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    PasswordHash = hashedPassword,
                    Role = role
                };

                // Сначала добавляем пользователя без картинки
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Теперь user.Id доступен

                // Теперь можно сохранить файл
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var ext = Path.GetExtension(model.ImageFile.FileName);
                    var fileName = $"{user.Id}{ext}";
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", fileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    user.ImageUrl = $"Images/User/{fileName}";
                    await _context.SaveChangesAsync(); // обновляем запись
                }

                // Автовход
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return user.Role switch
                {
                    "Owner" => RedirectToAction("OwnerDashboard", "Home"),
                    "Fermer" => RedirectToAction("FermerDashboard", "Home"),
                    _ => RedirectToAction("Index", "Home"),
                };
            }

            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (user.Role == "Owner")
                {
                    return RedirectToAction("OwnerDashboard", "Home");
                }
                else if (user.Role == "Fermer")
                {
                    return RedirectToAction("FermerDashboard", "Home");
                }
                else
                {
                    // fallback
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Неверный email или пароль");
            return View(model);
        }
        // GET: /Account/Edit
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /Account/Edit
        // POST: /Account/Edit
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User updatedUser, IFormFile? imageFile)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // обновляем поля
            user.FullName = updatedUser.FullName;
            user.Phone = updatedUser.Phone;
            user.Email = updatedUser.Email;

            // обновляем картинку только если загружен новый файл
            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{user.Id}{ext}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                user.ImageUrl = $"Images/User/{fileName}";
            }

            await _context.SaveChangesAsync();

            return user.Role switch
            {
                "Owner" => RedirectToAction("OwnerDashboard", "Home"),
                "Fermer" => RedirectToAction("FermerDashboard", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            if (!int.TryParse(userIdString, out int userId))
                return BadRequest("Invalid user ID");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return View(user);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}