using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.MarketProduct;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class MarketService : IMarketService
{
    private readonly AppDbContext _db;
    public MarketService(AppDbContext db) => _db = db;

    public async Task<MarketProductDTO> PublishAsync(MarketProductCreateDTO dto)
    {
        // ensure product and fermer exist
        var product = await _db.Products.FindAsync(dto.ProductId) 
            ?? throw new InvalidOperationException("Product not found");
        var fermer = await _db.Users.FindAsync(dto.FermerId)
            ?? throw new InvalidOperationException("Fermer not found");

        var mp = new MarketProduct
        {
            ProductId = dto.ProductId,
            FermerId = dto.FermerId,
            Price = dto.Price,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = "asdf",
            PublishDate = DateTime.UtcNow
        };

        _db.MarketProducts.Add(mp);
        await _db.SaveChangesAsync();

        return new MarketProductDTO
        {
            Id = mp.Id,
            ProductId = mp.ProductId,
            FermerId = mp.FermerId,
            Price = mp.Price,
            Title = mp.Title,
            Description = mp.Description,
            ImageUrl = mp.ImageUrl,
            Phone = dto.Phone,
            PublishDate = mp.PublishDate
        };
    }

    public async Task<IEnumerable<MarketProductDTO>> SearchAsync(string? category = null, string? q = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = _db.MarketProducts
            .Include(mp => mp.Product)
            .Include(mp => mp.Fermer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(mp => mp.Product.Category.ToLower().Contains(category.ToLower()));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qLow = q.ToLower();
            query = query.Where(mp => mp.Title.ToLower().Contains(qLow) || mp.Description.ToLower().Contains(qLow) || mp.Product.Title.ToLower().Contains(qLow));
        }

        if (minPrice.HasValue)
            query = query.Where(mp => mp.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(mp => mp.Price <= maxPrice.Value);

        var list = await query.ToListAsync();
        return list.Select(mp => new MarketProductDTO
        {
            Id = mp.Id,
            ProductId = mp.ProductId,
            FermerId = mp.FermerId,
            Price = mp.Price,
            Title = mp.Title,
            Description = mp.Description,
            ImageUrl = mp.ImageUrl,
            Phone = mp.Fermer.Phone,
            PublishDate = mp.PublishDate
        });
    }

    public async Task<MarketProductDTO?> GetByIdAsync(int id)
    {
        var mp = await _db.MarketProducts.Include(m => m.Product).Include(m => m.Fermer).FirstOrDefaultAsync(m => m.Id == id);
        if (mp == null) return null;
        return new MarketProductDTO
        {
            Id = mp.Id,
            ProductId = mp.ProductId,
            FermerId = mp.FermerId,
            Price = mp.Price,
            Title = mp.Title,
            Description = mp.Description,
            ImageUrl = mp.ImageUrl,
            Phone = mp.Fermer.Phone,
            PublishDate = mp.PublishDate
        };
    }

    public async Task<bool> UpdateAsync(MarketProductDTO dto)
    {
        var mp = await _db.MarketProducts.FindAsync(dto.Id);
        if (mp == null) return false;
        mp.Price = dto.Price;
        mp.Title = dto.Title;
        mp.Description = dto.Description;
        mp.ImageUrl = dto.ImageUrl;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var mp = await _db.MarketProducts.FindAsync(id);
        if (mp == null) return false;
        _db.MarketProducts.Remove(mp);
        await _db.SaveChangesAsync();
        return true;
    }
}