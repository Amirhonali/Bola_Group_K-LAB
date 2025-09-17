using DalaProject.DTOs;
using DalaProject.DTOs.Season;

namespace DalaProject.Services.Interfaces;

public interface ISeasonService
{
    Task<SeasonDTO> CreateAsync(SeasonCreateDTO dto);
    Task<SeasonDTO?> GetByIdAsync(int id);
    Task<IEnumerable<SeasonDTO>> GetByGroundAsync(int groundId);
    Task<bool> UpdateAsync(SeasonDTO dto);
    Task<bool> DeleteAsync(int id);
}