using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.Company;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class CompanyService : ICompanyService
{
    private readonly AppDbContext _db;
    public CompanyService(AppDbContext db) => _db = db;

    public async Task<CompanyDTO> CreateAsync(CompanyCreateDTO dto)
    {
        var entity = new Company
        {
            Name = dto.Name,
            OwnerId = dto.OwnerId
        };
        _db.Companies.Add(entity);
        await _db.SaveChangesAsync();
        return new CompanyDTO { Id = entity.Id, Name = entity.Name, OwnerId = entity.OwnerId, Description = dto.Description };
    }

    public async Task<CompanyDTO?> GetByIdAsync(int id)
    {
        var c = await _db.Companies.FindAsync(id);
        if (c == null) return null;
        return new CompanyDTO { Id = c.Id, Name = c.Name, OwnerId = c.OwnerId, Description = c.Description };
    }

    public async Task<IEnumerable<CompanyDTO>> GetByOwnerAsync(int ownerId)
    {
        return await _db.Companies
            .Where(c => c.OwnerId == ownerId)
            .Select(c => new CompanyDTO { Id = c.Id, Name = c.Name, OwnerId = c.OwnerId, Description = c.Description })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(CompanyDTO dto)
    {
        var c = await _db.Companies.FindAsync(dto.Id);
        if (c == null) return false;
        c.Name = dto.Name;
        c.Description = dto.Description;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _db.Companies.FindAsync(id);
        if (c == null) return false;
        _db.Companies.Remove(c);
        await _db.SaveChangesAsync();
        return true;
    }
}