using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("employees");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Address).HasMaxLength(300);
        b.Property(x => x.Role).IsRequired().HasMaxLength(50);
        b.Property(x => x.Salary).HasColumnType("numeric(12,2)");
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(ActiveStatus.Active);
    }
}
