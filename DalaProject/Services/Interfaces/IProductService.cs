using DalaProject.DTOs;
using DalaProject.DTOs.Product;

namespace DalaProject.Services.Interfaces;

public interface IProductService
{
    Task<ProductDTO> CreateAsync(ProductCreateDTO dto);
    Task<ProductDTO?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDTO>> GetBySeasonAsync(int seasonId);
    Task<bool> UpdateAsync(ProductDTO dto);
    Task<bool> DeleteAsync(int id);
}