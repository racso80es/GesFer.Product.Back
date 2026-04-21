namespace GesFer.Product.Back.Infrastructure.Security;

/// <summary>
/// Valida secretos de configuración para evitar arranque con marcadores versionados o valores no operativos.
/// </summary>
public static class SecretsConfigurationValidator
{
    private const string PlaceholderMarker = "INJECTED_VIA_ENV_OR_SECRET_MANAGER";

    /// <summary>
    /// Comprueba la clave de configuración JwtSettings:SecretKey y devuelve el valor validado.
    /// </summary>
    public static string ValidateJwtSecretKey(string? jwtSecretKey)
    {
        if (string.IsNullOrWhiteSpace(jwtSecretKey))
            throw new InvalidOperationException("JwtSettings:SecretKey no está configurado");

        if (jwtSecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey debe tener al menos 32 caracteres (256 bits) para cumplir con el algoritmo SHA-256 (HS256). " +
                $"Longitud actual: {jwtSecretKey.Length} caracteres.");
        }

        if (LooksLikeVersionedPlaceholder(jwtSecretKey))
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey no puede ser un marcador de plantilla del repositorio. " +
                "Configure un secreto real mediante User Secrets, variables de entorno (JwtSettings__SecretKey) o un secret manager.");
        }

        return jwtSecretKey;
    }

    /// <summary>
    /// Si <paramref name="internalSecret"/> tiene valor, no puede ser un marcador de plantilla versionado.
    /// </summary>
    public static void ValidateInternalSecretIfPresent(string? internalSecret)
    {
        if (string.IsNullOrEmpty(internalSecret))
            return;

        if (LooksLikeVersionedPlaceholder(internalSecret))
        {
            throw new InvalidOperationException(
                "InternalSecret no puede ser un marcador de plantilla del repositorio. " +
                "Configure un valor real mediante User Secrets, variables de entorno (InternalSecret) o un secret manager.");
        }
    }

    private static bool LooksLikeVersionedPlaceholder(string value) =>
        value.Contains(PlaceholderMarker, StringComparison.OrdinalIgnoreCase);
}
