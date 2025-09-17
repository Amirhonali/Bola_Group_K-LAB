using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.Report;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<ReportDTO> CreateAsync(ReportCreateDTO dto)
    {
        // Verify season and fermer
        var season = await _db.Seasons.FindAsync(dto.SeasonId)
            ?? throw new InvalidOperationException("Season not found");
        var fermer = await _db.Users.FindAsync(dto.FermerId)
            ?? throw new InvalidOperationException("Fermer not found");

        var r = new Report
        {
            SeasonId = dto.SeasonId,
            FermerId = dto.FermerId,
            Expenses = dto.Expenses,
            Income = dto.Income,
            QuantityProduced = dto.QuantityProduced,
            QuantitySold = dto.QuantitySold,
            CreatedAt = DateTime.UtcNow
        };
        _db.Reports.Add(r);
        await _db.SaveChangesAsync();

        return new ReportDTO
        {
            Id = r.Id,
            SeasonId = r.SeasonId,
            FermerId = r.FermerId,
            Expenses = r.Expenses,
            Income = r.Income,
            QuantityProduced = r.QuantityProduced,
            QuantitySold = r.QuantitySold,
            CreatedAt = r.CreatedAt
        };
    }

    public async Task<IEnumerable<ReportDTO>> GetByFermerAsync(int fermerId)
    {
        return await _db.Reports
            .Where(r => r.FermerId == fermerId)
            .Select(r => new ReportDTO
            {
                Id = r.Id,
                SeasonId = r.SeasonId,
                FermerId = r.FermerId,
                Expenses = r.Expenses,
                Income = r.Income,
                QuantityProduced = r.QuantityProduced,
                QuantitySold = r.QuantitySold,
                CreatedAt = r.CreatedAt
            }).ToListAsync();
    }

    public async Task<IEnumerable<ReportDTO>> GetBySeasonAsync(int seasonId)
    {
        return await _db.Reports
            .Where(r => r.SeasonId == seasonId)
            .Select(r => new ReportDTO
            {
                Id = r.Id,
                SeasonId = r.SeasonId,
                FermerId = r.FermerId,
                Expenses = r.Expenses,
                Income = r.Income,
                QuantityProduced = r.QuantityProduced,
                QuantitySold = r.QuantitySold,
                CreatedAt = r.CreatedAt
            }).ToListAsync();
    }
}