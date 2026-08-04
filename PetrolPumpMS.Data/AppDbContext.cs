using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<FuelType> FuelTypes => Set<FuelType>();
    public DbSet<Tank> Tanks => Set<Tank>();
    public DbSet<Nozzle> Nozzles => Set<Nozzle>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<CreditPayment> CreditPayments => Set<CreditPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
