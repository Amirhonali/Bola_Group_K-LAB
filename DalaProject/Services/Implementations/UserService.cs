using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using DalaProject.DTOs.User;

namespace DalaProject.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<UserDTO> RegisterAsync(UserRegisterDTO dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email.ToLower() == email))
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            FullName = dto.FullName,
            Email = email,
            Phone = dto.Phone,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Map(user);
    }

    public async Task<UserDTO?> GetByIdAsync(int id)
    {
        var u = await _db.Users.FindAsync(id);
        return u == null ? null : Map(u);
    }

    public async Task<UserDTO?> GetByEmailAsync(string email)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.Trim().ToLowerInvariant());
        return u == null ? null : Map(u);
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        var users = await _db.Users.ToListAsync();
        return users.Select(Map);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var u = await _db.Users.FindAsync(id);
        if (u == null) return false;
        _db.Users.Remove(u);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(UserDTO dto)
    {
        var u = await _db.Users.FindAsync(dto.Id);
        if (u == null) return false;
        u.FullName = dto.FullName;
        u.Phone = dto.Phone;
        u.Email = dto.Email;
        // role change left out intentionally (handle with care)
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyCredentialsAsync(string email, string password)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.Trim().ToLowerInvariant());
        if (u == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, u.PasswordHash);
    }

    private static UserDTO Map(User u) =>
        new UserDTO { Id = u.Id, FullName = u.FullName, Email = u.Email, Phone = u.Phone, Role = u.Role };
}