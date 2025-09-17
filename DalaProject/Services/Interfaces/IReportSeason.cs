using DalaProject.DTOs;
using DalaProject.DTOs.Report;

namespace DalaProject.Services.Interfaces;

public interface IReportService
{
    Task<ReportDTO> CreateAsync(ReportCreateDTO dto);
    Task<IEnumerable<ReportDTO>> GetByFermerAsync(int fermerId);
    Task<IEnumerable<ReportDTO>> GetBySeasonAsync(int seasonId);
}