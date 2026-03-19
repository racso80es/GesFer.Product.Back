using Microsoft.AspNetCore.Mvc;

namespace GesFer.Product.Back.Api.Controllers;

/// <summary>
/// Extensiones para ControllerBase. GetCompanyId obtiene el CompanyId del usuario autenticado
/// desde los claims JWT (company_id o CompanyId); nunca del frontend.
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// Obtiene el CompanyId del usuario autenticado desde los claims del token JWT.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Si no se encuentra CompanyId en el token.</exception>
    public static Guid GetCompanyId(this ControllerBase controller)
    {
        var companyIdClaim = controller.User.FindFirst("company_id")?.Value ?? controller.User.FindFirst("CompanyId")?.Value;
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            throw new UnauthorizedAccessException("No se encontró el ID de empresa en el token del usuario.");
        return companyId;
    }
}
