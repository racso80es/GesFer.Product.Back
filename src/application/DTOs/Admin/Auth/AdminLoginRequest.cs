using System.ComponentModel;

namespace GesFer.Product.Back.Application.DTOs.Admin.Auth;

/// <summary>
/// Contrato estándar de identidad global (Admin): solo Usuario y Contraseña (sin tenant/empresa).
/// </summary>
public class AdminLoginRequest
{
    /// <summary>
    /// Nombre de usuario administrativo
    /// </summary>
    /// <example>admin</example>
    [DefaultValue("admin")]
    public string Usuario { get; set; } = "admin";

    /// <summary>
    /// Contraseña del usuario administrativo
    /// </summary>
    /// <example>admin123</example>
    [DefaultValue("admin123")]
    public string Contraseña { get; set; } = "admin123";
}

