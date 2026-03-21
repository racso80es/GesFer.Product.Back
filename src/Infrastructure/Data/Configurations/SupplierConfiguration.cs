using GesFer.Product.Back.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesFer.Product.Back.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.TaxId)
            .HasMaxLength(50);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.Phone)
            .HasMaxLength(50);

        builder.Property(s => s.Email)
            .HasMaxLength(200);

        // CompanyId: FK a Companies (Admin); sin navegación
        builder.HasOne(s => s.BuyTariff)
            .WithMany(t => t.Suppliers)
            .HasForeignKey(s => s.BuyTariffId)
            .OnDelete(DeleteBehavior.SetNull);

        // Columnas geo: sin FK local (SSOT Admin)

        // Índices
        builder.HasIndex(s => new { s.CompanyId, s.Name });
        builder.HasIndex(s => s.PostalCodeId);
        builder.HasIndex(s => s.CityId);
        builder.HasIndex(s => s.StateId);
        builder.HasIndex(s => s.CountryId);
    }
}

