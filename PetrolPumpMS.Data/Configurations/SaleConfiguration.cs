using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> b)
    {
        b.ToTable("sales");
        b.HasKey(x => x.Id);
        b.Property(x => x.Date).HasColumnType("date").IsRequired();
        b.Property(x => x.Time).HasColumnType("time").IsRequired();
        b.Property(x => x.OpeningReading).HasColumnType("numeric(12,2)");
        b.Property(x => x.ClosingReading).HasColumnType("numeric(12,2)");
        b.Property(x => x.Quantity).HasColumnType("numeric(12,2)");
        b.Property(x => x.Rate).HasColumnType("numeric(10,2)");
        b.Property(x => x.Amount).HasColumnType("numeric(14,2)");
        b.Property(x => x.PaymentMode).HasConversion<string>().HasMaxLength(20).IsRequired();

        b.HasOne(x => x.Nozzle).WithMany(n => n.Sales).HasForeignKey(x => x.NozzleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Employee).WithMany(e => e.Sales).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.FuelType).WithMany().HasForeignKey(x => x.FuelTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Customer).WithMany(c => c.Sales).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Date);
    }
}
