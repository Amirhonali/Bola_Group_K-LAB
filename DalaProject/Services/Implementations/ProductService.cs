using DalaProject.Data;
using DalaProject.DTOs;
using DalaProject.DTOs.Product;
using DalaProject.Models;
using DalaProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DalaProject.Services.Implementations;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

    public async Task<ProductDTO> CreateAsync(ProductCreateDTO dto)
    {
        var p = new Product
        {
            Title = dto.Title,
            Category = dto.Category,
            Description = dto.Description,
            SeasonId = dto.SeasonId
        };
        _db.Products.Add(p);
        await _db.SaveChangesAsync();
        return new ProductDTO { Id = p.Id, Title = p.Title, Category = p.Category, Description = p.Description, SeasonId = p.SeasonId };
    }

    public async Task<ProductDTO?> GetByIdAsync(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return null;
        return new ProductDTO { Id = p.Id, Title = p.Title, Category = p.Category, Description = p.Description, SeasonId = p.SeasonId };
    }

    public async Task<IEnumerable<ProductDTO>> GetBySeasonAsync(int seasonId)
    {
        return await _db.Products
            .Where(p => p.SeasonId == seasonId)
            .Select(p => new ProductDTO { Id = p.Id, Title = p.Title, Category = p.Category, Description = p.Description, SeasonId = p.SeasonId })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(ProductDTO dto)
    {
        var p = await _db.Products.FindAsync(dto.Id);
        if (p == null) return false;
        p.Title = dto.Title;
        p.Category = dto.Category;
        p.Description = dto.Description;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return false;
        _db.Products.Remove(p);
        await _db.SaveChangesAsync();
        return true;
    }
}