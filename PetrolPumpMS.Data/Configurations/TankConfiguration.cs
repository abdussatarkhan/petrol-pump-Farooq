using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class TankConfiguration : IEntityTypeConfiguration<Tank>
{
    public void Configure(EntityTypeBuilder<Tank> b)
    {
        b.ToTable("tanks");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Capacity).HasColumnType("numeric(12,2)");
        b.Property(x => x.CurrentStock).HasColumnType("numeric(12,2)");
        b.Property(x => x.LowThreshold).HasColumnType("numeric(12,2)").HasDefaultValue(500m);

        b.HasOne(x => x.FuelType)
            .WithMany(f => f.Tanks)
            .HasForeignKey(x => x.FuelTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Ignore(x => x.IsLowStock);
    }
}
