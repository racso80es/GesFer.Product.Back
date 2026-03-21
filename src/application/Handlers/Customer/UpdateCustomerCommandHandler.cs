using GesFer.Product.Back.Application.Commands.Customer;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Customer;
using GesFer.Product.Back.Domain.ValueObjects;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.Customer;

public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IAdminGeolocationValidationService _geoValidation;

    public UpdateCustomerCommandHandler(ApplicationDbContext context, IAdminGeolocationValidationService geoValidation)
    {
        _context = context;
        _geoValidation = geoValidation;
    }

    public async Task<CustomerDto> HandleAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.DeletedAt == null, cancellationToken);

        if (customer == null)
            throw new InvalidOperationException($"No se encontró el cliente con ID {command.Id}");

        // Validar que no exista otro cliente con el mismo nombre en la misma empresa (excepto el actual)
        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CompanyId == customer.CompanyId && c.Name == command.Dto.Name && c.Id != command.Id && c.DeletedAt == null, cancellationToken);

        if (existingCustomer != null)
            throw new InvalidOperationException($"Ya existe otro cliente con el nombre '{command.Dto.Name}' en esta empresa");

        // Validar tarifa de venta si se proporciona
        if (command.Dto.SellTariffId.HasValue)
        {
            var tariffExists = await _context.Tariffs
                .AnyAsync(t => t.Id == command.Dto.SellTariffId.Value && t.CompanyId == customer.CompanyId && t.DeletedAt == null, cancellationToken);
            if (!tariffExists)
                throw new InvalidOperationException($"No se encontró la tarifa de venta con ID {command.Dto.SellTariffId.Value}");
        }

        await _geoValidation.ValidateGeoHierarchyAsync(
            command.Dto.CountryId,
            command.Dto.StateId,
            command.Dto.CityId,
            command.Dto.PostalCodeId,
            cancellationToken);

        // Validar y convertir TaxId si se proporciona
        TaxId? taxId = null;
        if (!string.IsNullOrWhiteSpace(command.Dto.TaxId))
        {
            taxId = TaxId.Create(command.Dto.TaxId);
        }

        // Validar y convertir Email si se proporciona
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(command.Dto.Email))
        {
            email = Email.Create(command.Dto.Email);
        }

        customer.Name = command.Dto.Name;
        customer.TaxId = taxId;
        customer.Address = command.Dto.Address;
        customer.Phone = command.Dto.Phone;
        customer.Email = email;
        customer.SellTariffId = command.Dto.SellTariffId;
        customer.PostalCodeId = command.Dto.PostalCodeId;
        customer.CityId = command.Dto.CityId;
        customer.StateId = command.Dto.StateId;
        customer.CountryId = command.Dto.CountryId;
        customer.IsActive = command.Dto.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CustomerDto
        {
            Id = customer.Id,
            CompanyId = customer.CompanyId,
            Name = customer.Name,
            TaxId = customer.TaxId.HasValue ? customer.TaxId.Value.Value : null,
            Address = customer.Address,
            Phone = customer.Phone,
            Email = customer.Email.HasValue ? customer.Email.Value.Value : null,
            SellTariffId = customer.SellTariffId,
            PostalCodeId = customer.PostalCodeId,
            CityId = customer.CityId,
            StateId = customer.StateId,
            CountryId = customer.CountryId,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

