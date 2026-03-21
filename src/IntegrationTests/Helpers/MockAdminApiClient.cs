using System.Collections.Concurrent;
using System.Linq;
using GesFer.Product.Back.Infrastructure.DTOs;
using GesFer.Product.Back.Infrastructure.DTOs.Geo;
using GesFer.Product.Back.Infrastructure.Services;

namespace GesFer.Product.Back.IntegrationTests.Helpers;

/// <summary>
/// Mock de IAdminApiClient para tests de integración (MyCompanyController).
/// Devuelve y actualiza datos en memoria sin llamar a la API de Admin.
/// </summary>
public class MockAdminApiClient : IAdminApiClient
{
    private static readonly Guid MockCountryId = Guid.Parse("10000000-0000-0000-0000-000000000101");
    private static readonly Guid MockStateId = Guid.Parse("10000000-0000-0000-0000-000000000102");
    private static readonly Guid MockCityId = Guid.Parse("10000000-0000-0000-0000-000000000103");
    private static readonly Guid MockPostalCodeId = Guid.Parse("10000000-0000-0000-0000-000000000104");

    private static readonly ConcurrentDictionary<Guid, AdminCompanyDto> Store = new();

    static MockAdminApiClient()
    {
        // Empresa Demo (ID usado por login en tests)
        var demoId = Guid.Parse("11111111-1111-1111-1111-111111111115");
        Store.TryAdd(demoId, new AdminCompanyDto
        {
            Id = demoId,
            Name = "Empresa Demo",
            TaxId = "B87654323",
            Address = "Calle Gran Vía, 1",
            Phone = "912345678",
            Email = "demo@empresa.com",
            LanguageId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = null
        });
    }

    public Task<AdminCompanyDto?> GetCompanyAsync(Guid id)
    {
        return Task.FromResult(Store.TryGetValue(id, out var dto) ? dto : null);
    }

    public Task<AdminCompanyDto?> GetCompanyByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Task.FromResult<AdminCompanyDto?>(null);
        var n = name.Trim();
        var dto = Store.Values.FirstOrDefault(c => string.Equals(c.Name, n, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<AdminCompanyDto?>(dto);
    }

    public Task<AdminCompanyDto> UpdateCompanyAsync(Guid id, AdminUpdateCompanyDto dto)
    {
        if (!Store.TryGetValue(id, out var current))
            throw new InvalidOperationException($"No se encontró la empresa con ID {id}");
        var updated = new AdminCompanyDto
        {
            Id = id,
            Name = dto.Name,
            TaxId = dto.TaxId,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            PostalCodeId = dto.PostalCodeId,
            CityId = dto.CityId,
            StateId = dto.StateId,
            CountryId = dto.CountryId,
            LanguageId = dto.LanguageId,
            IsActive = dto.IsActive,
            CreatedAt = current.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        Store[id] = updated;
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
