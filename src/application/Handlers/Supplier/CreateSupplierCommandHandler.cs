using GesFer.Product.Back.Application.Commands.Supplier;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Supplier;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.Supplier;

public class CreateSupplierCommandHandler : ICommandHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IAdminApiClient _adminApiClient;
    private readonly IAdminGeolocationValidationService _geoValidation;

    public CreateSupplierCommandHandler(
        ApplicationDbContext context,
        IAdminApiClient adminApiClient,
        IAdminGeolocationValidationService geoValidation)
    {
        _context = context;
        _adminApiClient = adminApiClient;
        _geoValidation = geoValidation;
    }

    public async Task<SupplierDto> HandleAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _adminApiClient.GetCompanyAsync(command.Dto.CompanyId);
        if (company == null)
            throw new InvalidOperationException($"No se encontró la empresa con ID {command.Dto.CompanyId}");

        // Validar que no exista un proveedor con el mismo nombre en la misma empresa
        var existingSupplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.CompanyId == command.Dto.CompanyId && s.Name == command.Dto.Name && s.DeletedAt == null, cancellationToken);

        if (existingSupplier != null)
            throw new InvalidOperationException($"Ya existe un proveedor con el nombre '{command.Dto.Name}' en esta empresa");

        // Validar tarifa de compra si se proporciona
        if (command.Dto.BuyTariffId.HasValue)
        {
            var tariffExists = await _context.Tariffs
                .AnyAsync(t => t.Id == command.Dto.BuyTariffId.Value && t.CompanyId == command.Dto.CompanyId && t.DeletedAt == null, cancellationToken);
            if (!tariffExists)
                throw new InvalidOperationException($"No se encontró la tarifa de compra con ID {command.Dto.BuyTariffId.Value}");
        }

        await _geoValidation.ValidateGeoHierarchyAsync(
            command.Dto.CountryId,
            command.Dto.StateId,
            command.Dto.CityId,
            command.Dto.PostalCodeId,
            cancellationToken);

        var supplier = new Product.Back.Domain.Entities.Supplier
        {
            CompanyId = command.Dto.CompanyId,
            Name = command.Dto.Name,
            TaxId = command.Dto.TaxId,
            Address = command.Dto.Address,
            Phone = command.Dto.Phone,
            Email = command.Dto.Email,
            BuyTariffId = command.Dto.BuyTariffId,
            PostalCodeId = command.Dto.PostalCodeId,
            CityId = command.Dto.CityId,
            StateId = command.Dto.StateId,
            CountryId = command.Dto.CountryId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return new SupplierDto
        {
            Id = supplier.Id,
            CompanyId = supplier.CompanyId,
            Name = supplier.Name,
            TaxId = supplier.TaxId,
            Address = supplier.Address,
            Phone = supplier.Phone,
            Email = supplier.Email,
            BuyTariffId = supplier.BuyTariffId,
            PostalCodeId = supplier.PostalCodeId,
            CityId = supplier.CityId,
            StateId = supplier.StateId,
            CountryId = supplier.CountryId,
            IsActive = supplier.IsActive,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}

