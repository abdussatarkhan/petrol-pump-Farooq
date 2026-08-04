using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Username).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.Username).IsUnique();
        b.Property(x => x.PasswordHash).IsRequired();
        b.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.Active).HasDefaultValue(true);
    }
}
