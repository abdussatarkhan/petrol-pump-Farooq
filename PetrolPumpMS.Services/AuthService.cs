using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    public AuthService(AppDbContext db) => _db = db;

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.Active);
        if (user is null) return null;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return ServiceResult.Fail("User not found.");
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return ServiceResult.Fail("Current password is incorrect.");
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return ServiceResult.Fail("New password must be at least 6 characters.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
