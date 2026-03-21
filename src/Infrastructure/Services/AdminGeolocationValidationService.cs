namespace GesFer.Product.Back.Infrastructure.Services;

/// <inheritdoc cref="IAdminGeolocationValidationService" />
public sealed class AdminGeolocationValidationService : IAdminGeolocationValidationService
{
    private readonly IAdminApiClient _adminApi;

    public AdminGeolocationValidationService(IAdminApiClient adminApi)
    {
        _adminApi = adminApi;
    }

    /// <inheritdoc />
    public async Task ValidateGeoHierarchyAsync(
        Guid? countryId,
        Guid? stateId,
        Guid? cityId,
        Guid? postalCodeId,
        CancellationToken cancellationToken = default)
    {
        if (countryId.HasValue)
        {
            var country = await _adminApi.GetGeolocationCountryByIdAsync(countryId.Value, cancellationToken);
            if (country == null)
                throw new InvalidOperationException($"No se encontró el país con ID {countryId.Value} en Admin.");
        }

        if (stateId.HasValue)
        {
            if (!countryId.HasValue)
                throw new InvalidOperationException("Si se indica provincia (StateId), debe indicarse también país (CountryId).");

            var states = await _adminApi.GetGeolocationStatesByCountryAsync(countryId!.Value, cancellationToken);
            if (states == null || states.All(s => s.Id != stateId.Value))
                throw new InvalidOperationException($"No se encontró la provincia con ID {stateId.Value} para el país indicado.");
        }

        if (cityId.HasValue)
        {
            if (!stateId.HasValue || !countryId.HasValue)
                throw new InvalidOperationException("Si se indica ciudad (CityId), deben indicarse provincia (StateId) y país (CountryId).");

            var cities = await _adminApi.GetGeolocationCitiesByStateAsync(stateId!.Value, cancellationToken);
            if (cities == null || cities.All(c => c.Id != cityId.Value))
                throw new InvalidOperationException($"No se encontró la ciudad con ID {cityId.Value} para la provincia indicada.");
        }

        if (postalCodeId.HasValue)
        {
            if (!cityId.HasValue || !stateId.HasValue || !countryId.HasValue)
                throw new InvalidOperationException("Si se indica código postal (PostalCodeId), deben indicarse ciudad, provincia y país.");

            var postalCodes = await _adminApi.GetGeolocationPostalCodesByCityAsync(cityId!.Value, cancellationToken);
            if (postalCodes == null || postalCodes.All(p => p.Id != postalCodeId.Value))
                throw new InvalidOperationException($"No se encontró el código postal con ID {postalCodeId.Value} para la ciudad indicada.");
        }
    }
}
