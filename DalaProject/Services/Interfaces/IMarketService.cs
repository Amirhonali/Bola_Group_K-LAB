using DalaProject.DTOs;
using DalaProject.DTOs.MarketProduct;

namespace DalaProject.Services.Interfaces;

public interface IMarketService
{
    Task<MarketProductDTO> PublishAsync(MarketProductCreateDTO dto);
    Task<IEnumerable<MarketProductDTO>> SearchAsync(string? category = null, string? q = null, decimal? minPrice = null, decimal? maxPrice = null);
    Task<MarketProductDTO?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(MarketProductDTO dto);
    Task<bool> RemoveAsync(int id);
}