using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data;

/// <summary>
/// Idempotent runtime seeding, run once after migrations are applied on startup.
/// Kept out of HasData/migrations because the admin password must be BCrypt-hashed
/// with a fresh salt rather than baked into a migration as a fixed string.
/// </summary>
public static class DbSeeder
{
    public const string DefaultAdminPassword = "Admin@123";

    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultAdminPassword),
                FullName = "System Administrator",
                Role = UserRole.Admin,
                Active = true
            });
        }

        if (!await db.FuelTypes.AnyAsync())
        {
            db.FuelTypes.AddRange(
                new FuelType { Name = "Petrol", Unit = "Liter", CurrentPrice = 106.50m },
                new FuelType { Name = "Diesel", Unit = "Liter", CurrentPrice = 92.75m }
            );
        }

        await db.SaveChangesAsync();
    }
}
