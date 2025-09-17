using DalaProject.DTOs;
using DalaProject.DTOs.User;

namespace DalaProject.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO> RegisterAsync(UserRegisterDTO dto);
    Task<UserDTO?> GetByIdAsync(int id);
    Task<UserDTO?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDTO>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(UserDTO dto);
    Task<bool> VerifyCredentialsAsync(string email, string password);
}