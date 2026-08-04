using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> b)
    {
        b.ToTable("purchases");
        b.HasKey(x => x.Id);
        b.Property(x => x.Date).HasColumnType("date").IsRequired();
        b.Property(x => x.Supplier).HasMaxLength(150);
        b.Property(x => x.Quantity).HasColumnType("numeric(12,2)");
        b.Property(x => x.Rate).HasColumnType("numeric(10,2)");
        b.Property(x => x.Total).HasColumnType("numeric(14,2)");

        b.HasOne(x => x.FuelType).WithMany().HasForeignKey(x => x.FuelTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Tank).WithMany(t => t.Purchases).HasForeignKey(x => x.TankId).OnDelete(DeleteBehavior.Restrict);
    }
}
