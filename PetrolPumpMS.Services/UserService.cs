using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public Task<List<User>> GetAllAsync() =>
        _db.Users.OrderBy(u => u.Username).ToListAsync();

    public async Task<ServiceResult> CreateAsync(string username, string password, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username)) return ServiceResult.Fail("Username is required.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6) return ServiceResult.Fail("Password must be at least 6 characters.");
        if (await _db.Users.AnyAsync(u => u.Username == username)) return ServiceResult.Fail("That username is already taken.");

        _db.Users.Add(new User
        {
            Username = username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName.Trim(),
            Role = role,
            Active = true
        });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetActiveAsync(int userId, bool active)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return ServiceResult.Fail("User not found.");
        user.Active = active;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetRoleAsync(int userId, UserRole role)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return ServiceResult.Fail("User not found.");
        user.Role = role;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
