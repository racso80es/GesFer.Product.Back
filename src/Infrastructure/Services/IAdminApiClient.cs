using GesFer.Product.Back.Infrastructure.DTOs;
using GesFer.Product.Back.Infrastructure.DTOs.Geo;

namespace GesFer.Product.Back.Infrastructure.Services;

/// <summary>
/// Interfaz para comunicarse con el API de Admin
/// </summary>
/// <summary>
/// Product solo necesita conocer una empresa (la que le corresponde). Admin no expone listado de todas.
/// </summary>
public interface IAdminApiClient
{
    Task<AdminCompanyDto?> GetCompanyAsync(Guid id);
    /// <summary>Obtiene una empresa por nombre (Admin API). Para login.</summary>
    Task<AdminCompanyDto?> GetCompanyByNameAsync(string name);
    Task<AdminCompanyDto> UpdateCompanyAsync(Guid id, AdminUpdateCompanyDto dto);

    /// <summary>GET /api/geolocation/countries</summary>
    Task<IReadOnlyList<CountryGeoReadDto>> GetGeolocationCountriesAsync(CancellationToken cancellationToken = default);

    /// <summary>GET /api/geolocation/countries/{countryId} — 404 devuelve null.</summary>
    Task<CountryGeoReadDto?> GetGeolocationCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default);

    /// <summary>GET /api/geolocation/countries/{countryId}/states</summary>
    Task<IReadOnlyList<StateGeoReadDto>> GetGeolocationStatesByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);

    /// <summary>GET /api/geolocation/states/{stateId}/cities</summary>
    Task<IReadOnlyList<CityGeoReadDto>> GetGeolocationCitiesByStateAsync(Guid stateId, CancellationToken cancellationToken = default);

    /// <summary>GET /api/geolocation/cities/{cityId}/postal-codes</summary>
    Task<IReadOnlyList<PostalCodeGeoReadDto>> GetGeolocationPostalCodesByCityAsync(Guid cityId, CancellationToken cancellationToken = default);
}
