using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class FuelTypeConfiguration : IEntityTypeConfiguration<FuelType>
{
    public void Configure(EntityTypeBuilder<FuelType> b)
    {
        b.ToTable("fuel_types");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.Unit).IsRequired().HasMaxLength(20).HasDefaultValue("Liter");
        b.Property(x => x.CurrentPrice).HasColumnType("numeric(10,2)");
    }
}
