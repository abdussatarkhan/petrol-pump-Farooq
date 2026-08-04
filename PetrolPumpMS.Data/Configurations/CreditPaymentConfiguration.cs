using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.Data.Configurations;

public class CreditPaymentConfiguration : IEntityTypeConfiguration<CreditPayment>
{
    public void Configure(EntityTypeBuilder<CreditPayment> b)
    {
        b.ToTable("credit_payments");
        b.HasKey(x => x.Id);
        b.Property(x => x.Date).HasColumnType("date").IsRequired();
        b.Property(x => x.Amount).HasColumnType("numeric(12,2)");
        b.Property(x => x.Note).HasMaxLength(500);

        b.HasOne(x => x.Customer)
            .WithMany(c => c.CreditPayments)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
