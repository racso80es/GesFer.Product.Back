using GesFer.Product.Back.Infrastructure.DTOs;
using GesFer.Product.Back.Infrastructure.DTOs.Geo;
using GesFer.Product.Back.Infrastructure.Services;

namespace GesFer.Product.Back.Api.Services;

public class MockAdminApiClient : IAdminApiClient
{
    internal static readonly Guid MockCountryId = Guid.Parse("10000000-0000-0000-0000-000000000101");
    internal static readonly Guid MockStateId = Guid.Parse("10000000-0000-0000-0000-000000000102");
    internal static readonly Guid MockCityId = Guid.Parse("10000000-0000-0000-0000-000000000103");
    internal static readonly Guid MockPostalCodeId = Guid.Parse("10000000-0000-0000-0000-000000000104");

    private static AdminCompanyDto _mockCompany = new AdminCompanyDto
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111115"),
        Name = "Empresa Demo",
        Address = "Calle Falsa 123",
        Phone = "123456789",
        Email = "demo@empresa.com",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    public Task<AdminCompanyDto?> GetCompanyAsync(Guid id)
    {
        return Task.FromResult<AdminCompanyDto?>(_mockCompany);
    }

    public Task<AdminCompanyDto?> GetCompanyByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Task.FromResult<AdminCompanyDto?>(null);
        var match = string.Equals(_mockCompany.Name, name.Trim(), StringComparison.OrdinalIgnoreCase);
        return Task.FromResult<AdminCompanyDto?>(match ? _mockCompany : null);
    }

    public Task<AdminCompanyDto> UpdateCompanyAsync(Guid id, AdminUpdateCompanyDto dto)
    {
        // Update the mock state
        _mockCompany.Name = dto.Name;
        _mockCompany.Address = dto.Address;
        if (dto.TaxId != null) _mockCompany.TaxId = dto.TaxId;
        if (dto.Phone != null) _mockCompany.Phone = dto.Phone;
        if (dto.Email != null) _mockCompany.Email = dto.Email;

        // Return updated DTO
        var updated = new AdminCompanyDto
        {
            Id = _mockCompany.Id,
            Name = _mockCompany.Name,
            Address = _mockCompany.Address,
            TaxId = _mockCompany.TaxId,
            Phone = _mockCompany.Phone,
            Email = _mockCompany.Email,
            IsActive = _mockCompany.IsActive,
            CreatedAt = _mockCompany.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        return Task.FromResult(updated);
    }

    public Task<IReadOnlyList<CountryGeoReadDto>> GetGeolocationCountriesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CountryGeoReadDto> list = new[]
        {
            new CountryGeoReadDto { Id = MockCountryId, Name = "España (mock)", Code = "ES" }
        };
        return Task.FromResult(list);
    }

    public Task<CountryGeoReadDto?> GetGeolocationCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        if (countryId != MockCountryId)
            return Task.FromResult<CountryGeoReadDto?>(null);
        return Task.FromResult<CountryGeoReadDto?>(new CountryGeoReadDto { Id = MockCountryId, Name = "España (mock)", Code = "ES" });
    }

    public Task<IReadOnlyList<StateGeoReadDto>> GetGeolocationStatesByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        if (countryId != MockCountryId)
            return Task.FromResult<IReadOnlyList<StateGeoReadDto>>(Array.Empty<StateGeoReadDto>());
        IReadOnlyList<StateGeoReadDto> list = new[]
        {
            new StateGeoReadDto { Id = MockStateId, CountryId = MockCountryId, Name = "Provincia mock", Code = "M" }
        };
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<CityGeoReadDto>> GetGeolocationCitiesByStateAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
        if (stateId != MockStateId)
            return Task.FromResult<IReadOnlyList<CityGeoReadDto>>(Array.Empty<CityGeoReadDto>());
        IReadOnlyList<CityGeoReadDto> list = new[]
        {
            new CityGeoReadDto { Id = MockCityId, StateId = MockStateId, Name = "Ciudad mock" }
        };
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<PostalCodeGeoReadDto>> GetGeolocationPostalCodesByCityAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        if (cityId != MockCityId)
            return Task.FromResult<IReadOnlyList<PostalCodeGeoReadDto>>(Array.Empty<PostalCodeGeoReadDto>());
        IReadOnlyList<PostalCodeGeoReadDto> list = new[]
        {
            new PostalCodeGeoReadDto { Id = MockPostalCodeId, CityId = MockCityId, Code = "28001" }
        };
        return Task.FromResult(list);
    }
}
