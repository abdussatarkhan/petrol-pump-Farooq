using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PetrolPumpMS.Data;

/// <summary>
/// Lets `dotnet ef migrations add` / `dotnet ef database update` construct the
/// DbContext at design time without running the full WPF app. It reads the same
/// appsettings.json used by the App project (falling back to a local default).
/// Run EF commands from the repository root, e.g.:
///   dotnet ef migrations add InitialCreate --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
///   dotnet ef database update --project PetrolPumpMS.Data --startup-project PetrolPumpMS.App
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "..", "PetrolPumpMS.App"))
            ? Path.Combine(Directory.GetCurrentDirectory(), "..", "PetrolPumpMS.App")
            : Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connectionString = config.GetConnectionString("Default")
            ?? "Host=localhost;Port=5432;Database=petrol_pump_db;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
