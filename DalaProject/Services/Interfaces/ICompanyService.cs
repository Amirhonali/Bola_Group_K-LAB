using DalaProject.DTOs;
using DalaProject.DTOs.Company;

namespace DalaProject.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyDTO> CreateAsync(CompanyCreateDTO dto);
    Task<CompanyDTO?> GetByIdAsync(int id);
    Task<IEnumerable<CompanyDTO>> GetByOwnerAsync(int ownerId);
    Task<bool> UpdateAsync(CompanyDTO dto);
    Task<bool> DeleteAsync(int id);
}