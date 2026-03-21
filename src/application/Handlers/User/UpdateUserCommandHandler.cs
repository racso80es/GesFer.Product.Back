using GesFer.Product.Back.Application.Commands.User;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.User;
using GesFer.Product.Back.Domain.ValueObjects;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.User;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IAdminApiClient _adminApiClient;
    private readonly IAdminGeolocationValidationService _geoValidation;

    public UpdateUserCommandHandler(
        ApplicationDbContext context,
        IAdminApiClient adminApiClient,
        IAdminGeolocationValidationService geoValidation)
    {
        _context = context;
        _adminApiClient = adminApiClient;
        _geoValidation = geoValidation;
    }

    public async Task<UserDto> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id && u.DeletedAt == null, cancellationToken);

        if (user == null)
            throw new InvalidOperationException($"No se encontró el usuario con ID {command.Id}");

        // Validar que no exista otro usuario con el mismo username en la misma empresa (excepto el actual)
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == command.Dto.Username
                && u.CompanyId == user.CompanyId
                && u.Id != command.Id
                && u.DeletedAt == null, cancellationToken);

        if (existingUser != null)
            throw new InvalidOperationException($"Ya existe otro usuario con el nombre '{command.Dto.Username}' en esta empresa");

        await _geoValidation.ValidateGeoHierarchyAsync(
            command.Dto.CountryId,
            command.Dto.StateId,
            command.Dto.CityId,
            command.Dto.PostalCodeId,
            cancellationToken);

        // Validar LanguageId si se proporciona
        if (command.Dto.LanguageId.HasValue)
        {
            var languageExists = await _context.Languages
                .AnyAsync(l => l.Id == command.Dto.LanguageId.Value && l.DeletedAt == null, cancellationToken);
            if (!languageExists)
                throw new InvalidOperationException($"No se encontró el idioma con ID {command.Dto.LanguageId.Value}");
        }

        // Validar y convertir Email si se proporciona
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(command.Dto.Email))
        {
            email = Email.Create(command.Dto.Email);
        }

        user.Username = command.Dto.Username;
        user.FirstName = command.Dto.FirstName;
        user.LastName = command.Dto.LastName;
        user.Email = email;
        user.Phone = command.Dto.Phone;
        user.Address = command.Dto.Address;
        user.PostalCodeId = command.Dto.PostalCodeId;
        user.CityId = command.Dto.CityId;
        user.StateId = command.Dto.StateId;
        user.CountryId = command.Dto.CountryId;
        user.LanguageId = command.Dto.LanguageId;
        user.IsActive = command.Dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Actualizar contraseña solo si se proporciona
        if (!string.IsNullOrWhiteSpace(command.Dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Dto.Password, BCrypt.Net.BCrypt.GenerateSalt(11));
        }

        await _context.SaveChangesAsync(cancellationToken);

        var company = await _adminApiClient.GetCompanyAsync(user.CompanyId);
        return new UserDto
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            CompanyName = company?.Name ?? string.Empty,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email.HasValue ? user.Email.Value.Value : null,
            Phone = user.Phone,
            Address = user.Address,
            PostalCodeId = user.PostalCodeId,
            CityId = user.CityId,
            StateId = user.StateId,
            CountryId = user.CountryId,
            LanguageId = user.LanguageId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

