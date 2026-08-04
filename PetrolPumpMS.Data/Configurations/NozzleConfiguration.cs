using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class NozzleConfiguration : IEntityTypeConfiguration<Nozzle>
{
    public void Configure(EntityTypeBuilder<Nozzle> b)
    {
        b.ToTable("nozzles");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(ActiveStatus.Active);
        b.Property(x => x.LastReading).HasColumnType("numeric(12,2)");

        b.HasOne(x => x.Tank)
            .WithMany(t => t.Nozzles)
            .HasForeignKey(x => x.TankId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
