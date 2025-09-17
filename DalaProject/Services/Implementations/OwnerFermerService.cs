using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.OwnerFermer;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class OwnerFermerService : IOwnerFermerService
{
    private readonly AppDbContext _db;
    public OwnerFermerService(AppDbContext db) => _db = db;

    public async Task<OwnerFermerDTO> CreateInvitationAsync(OwnerFermerCreateDTO dto)
    {
        if (!await _db.Users.AnyAsync(u => u.Id == dto.OwnerId))
            throw new InvalidOperationException("Owner not found");
        if (!await _db.Users.AnyAsync(u => u.Id == dto.FermerId))
            throw new InvalidOperationException("Fermer not found");

        var exists = await _db.OwnerFermers.AnyAsync(of => of.OwnerId == dto.OwnerId && of.FermerId == dto.FermerId && of.Status == "Pending");
        if (exists) throw new InvalidOperationException("Pending invitation already exists");

        var ent = new OwnerFermer
        {
            OwnerId = dto.OwnerId,
            FermerId = dto.FermerId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        _db.OwnerFermers.Add(ent);
        await _db.SaveChangesAsync();

        return new OwnerFermerDTO { Id = ent.Id, OwnerId = ent.OwnerId, FermerId = ent.FermerId, Status = ent.Status, CreatedAt = ent.CreatedAt };
    }

    public async Task<IEnumerable<OwnerFermerDTO>> GetInvitationsForFermerAsync(int fermerId)
    {
        return await _db.OwnerFermers
            .Where(of => of.FermerId == fermerId && of.Status == "Pending")
            .Include(of => of.Owner)
            .Select(of => new OwnerFermerDTO
            {
                Id = of.Id,
                OwnerId = of.OwnerId,
                FermerId = of.FermerId,
                Status = of.Status,
                CreatedAt = of.CreatedAt
            }).ToListAsync();
    }

    public async Task AcceptInvitationAsync(int invitationId)
    {
        var inv = await _db.OwnerFermers.FindAsync(invitationId);
        if (inv == null) throw new KeyNotFoundException("Invitation not found");
        if (inv.Status != "Pending") throw new InvalidOperationException("Invitation is not pending");

        inv.Status = "Accepted";
        // ensure no duplicate relation (if using OwnerFermer both for invite and relation; accepted becomes relation)
        await _db.SaveChangesAsync();
    }

    public async Task RejectInvitationAsync(int invitationId)
    {
        var inv = await _db.OwnerFermers.FindAsync(invitationId);
        if (inv == null) throw new KeyNotFoundException("Invitation not found");
        if (inv.Status != "Pending") throw new InvalidOperationException("Invitation is not pending");
        inv.Status = "Rejected";
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<OwnerFermerDTO>> GetOwnersForFermerAsync(int fermerId)
    {
        return await _db.OwnerFermers
            .Where(of => of.FermerId == fermerId && of.Status == "Accepted")
            .Include(of => of.Owner)
            .Select(of => new OwnerFermerDTO
            {
                Id = of.Id,
                OwnerId = of.OwnerId,
                FermerId = of.FermerId,
                Status = of.Status,
                CreatedAt = of.CreatedAt
            }).ToListAsync();
    }
}