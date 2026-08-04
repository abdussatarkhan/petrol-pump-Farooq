using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}
