using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace GesFer.Product.Back.Infrastructure.Extensions;

/// <summary>
/// Extensiones para <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Obtiene una cadena de conexión por nombre o lanza si falta o está vacía.
    /// </summary>
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        var value = configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"La cadena de conexión '{name}' es obligatoria. " +
                "Defínala en ConnectionStrings:" + name +
                " (appsettings, User Secrets o variable de entorno ConnectionStrings__" + name + ").");
        }

        return value;
    }

    /// <summary>
    /// Obtiene un valor de configuración por clave o lanza si falta o está vacío (solo espacios).
    /// </summary>
    public static string GetRequired(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"La configuración '{key}' es obligatoria y no puede estar vacía. " +
                "Defínala en appsettings, User Secrets o variables de entorno (notación __ para jerarquía).");
        }

        return value;
    }

    /// <summary>
    /// Obtiene un entero positivo o lanza si falta, no es un número o no es &gt; 0.
    /// </summary>
    public static int GetRequiredPositiveInt(this IConfiguration configuration, string key)
    {
        var s = configuration.GetRequired(key);
        if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) || n <= 0)
        {
            throw new InvalidOperationException(
                $"La configuración '{key}' debe ser un entero positivo. Valor actual: '{s}'.");
        }

        return n;
    }

    /// <summary>
    /// Obtiene un booleano o lanza si falta o no es parseable como bool.
    /// </summary>
    public static bool GetRequiredBool(this IConfiguration configuration, string key)
    {
        var s = configuration.GetRequired(key);
        if (!bool.TryParse(s, out var value))
        {
            throw new InvalidOperationException(
                $"La configuración '{key}' debe ser true o false. Valor actual: '{s}'.");
        }

        return value;
    }
}
