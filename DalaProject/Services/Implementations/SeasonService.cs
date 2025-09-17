using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.Season;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class SeasonService : ISeasonService
{
    private readonly AppDbContext _db;
    public SeasonService(AppDbContext db) => _db = db;

    public async Task<SeasonDTO> CreateAsync(SeasonCreateDTO dto)
    {
        var s = new Season
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate ?? dto.StartDate.AddMonths(3),
            GroundId = dto.GroundId
        };
        _db.Seasons.Add(s);
        await _db.SaveChangesAsync();
        return new SeasonDTO { Id = s.Id, Name = s.Name, StartDate = s.StartDate, EndDate = s.EndDate, GroundId = s.GroundId };
    }

    public async Task<SeasonDTO?> GetByIdAsync(int id)
    {
        var s = await _db.Seasons.FindAsync(id);
        if (s == null) return null;
        return new SeasonDTO { Id = s.Id, Name = s.Name, StartDate = s.StartDate, EndDate = s.EndDate, GroundId = s.GroundId };
    }

    public async Task<IEnumerable<SeasonDTO>> GetByGroundAsync(int groundId)
    {
        return await _db.Seasons
            .Where(s => s.GroundId == groundId)
            .Select(s => new SeasonDTO { Id = s.Id, Name = s.Name, StartDate = s.StartDate, EndDate = s.EndDate, GroundId = s.GroundId })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(SeasonDTO dto)
    {
        var s = await _db.Seasons.FindAsync(dto.Id);
        if (s == null) return false;
        s.Name = dto.Name;
        s.StartDate = dto.StartDate;
        s.EndDate = dto.EndDate;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var s = await _db.Seasons.FindAsync(id);
        if (s == null) return false;
        _db.Seasons.Remove(s);
        await _db.SaveChangesAsync();
        return true;
    }
}