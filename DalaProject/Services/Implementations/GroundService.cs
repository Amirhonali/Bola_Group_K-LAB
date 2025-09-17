using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.Ground;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class GroundService : IGroundService
{
    private readonly AppDbContext _db;
    public GroundService(AppDbContext db) => _db = db;

    public async Task<GroundDTO> CreateAsync(GroundCreateDTO dto)
    {
        var entity = new Ground
        {
            Location = dto.Location,
            Area = dto.Area,
            CompanyId = dto.CompanyId,
            FermerId = dto.FermerId
        };
        _db.Grounds.Add(entity);
        await _db.SaveChangesAsync();
        return new GroundDTO { Id = entity.Id, Location = entity.Location, Area = entity.Area, CompanyId = entity.CompanyId, FermerId = entity.FermerId };
    }

    public async Task<GroundDTO?> GetByIdAsync(int id)
    {
        var g = await _db.Grounds.FindAsync(id);
        if (g == null) return null;
        return new GroundDTO { Id = g.Id, Location = g.Location, Area = g.Area, CompanyId = g.CompanyId, FermerId = g.FermerId };
    }

    public async Task<IEnumerable<GroundDTO>> GetByCompanyAsync(int companyId)
    {
        return await _db.Grounds
            .Where(g => g.CompanyId == companyId)
            .Select(g => new GroundDTO { Id = g.Id, Location = g.Location, Area = g.Area, CompanyId = g.CompanyId, FermerId = g.FermerId })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(GroundDTO dto)
    {
        var g = await _db.Grounds.FindAsync(dto.Id);
        if (g == null) return false;
        g.Location = dto.Location;
        g.Area = dto.Area;
        g.FermerId = dto.FermerId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var g = await _db.Grounds.FindAsync(id);
        if (g == null) return false;
        _db.Grounds.Remove(g);
        await _db.SaveChangesAsync();
        return true;
    }
}