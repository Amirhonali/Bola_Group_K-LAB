using DalaProject.DTOs;
using DalaProject.DTOs.Ground;

namespace DalaProject.Services.Interfaces;

public interface IGroundService
{
    Task<GroundDTO> CreateAsync(GroundCreateDTO dto);
    Task<GroundDTO?> GetByIdAsync(int id);
    Task<IEnumerable<GroundDTO>> GetByCompanyAsync(int companyId);
    Task<bool> UpdateAsync(GroundDTO dto);
    Task<bool> DeleteAsync(int id);
}