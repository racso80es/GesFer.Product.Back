using GesFer.Product.Back.Domain.Entities;
using GesFer.Product.Back.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GesFer.Product.Back.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    private static Email? ConvertStringToEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Email.TryCreate(value, out var email) ? email : (Email?)null;
    }

    private static TaxId? ConvertStringToTaxId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return TaxId.TryCreate(value, out var taxId) ? taxId : (TaxId?)null;
    }

    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.TaxId)
            .HasConversion(
                taxId => taxId.HasValue ? taxId.Value.Value : null,
                value => ConvertStringToTaxId(value))
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(200)
            .HasConversion(
                email => email.HasValue ? email.Value.Value : null,
                value => ConvertStringToEmail(value));

        builder.HasOne(c => c.PostalCode)
            .WithMany()
            .HasForeignKey(c => c.PostalCodeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.City)
            .WithMany()
            .HasForeignKey(c => c.CityId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.State)
            .WithMany()
            .HasForeignKey(c => c.StateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Country)
            .WithMany()
            .HasForeignKey(c => c.CountryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Language)
            .WithMany()
            .HasForeignKey(c => c.LanguageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.Name);
    }
}
