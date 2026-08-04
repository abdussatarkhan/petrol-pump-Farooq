using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<ServiceResult> CreateAsync(string username, string password, string fullName, UserRole role);
    Task<ServiceResult> SetActiveAsync(int userId, bool active);
    Task<ServiceResult> SetRoleAsync(int userId, UserRole role);
}
