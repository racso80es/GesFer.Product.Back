namespace GesFer.Product.Back.Infrastructure.Services;

/// <summary>
/// Valida jerarquía país → provincia → ciudad → código postal contra la API Admin (SSOT).
/// </summary>
public interface IAdminGeolocationValidationService
{
    /// <summary>
    /// Valida existencia y coherencia de los ids geo indicados (solo los que vienen con valor).
    /// </summary>
    Task ValidateGeoHierarchyAsync(
        Guid? countryId,
        Guid? stateId,
        Guid? cityId,
        Guid? postalCodeId,
        CancellationToken cancellationToken = default);
}
