using GesFer.Product.Back.Application.DTOs.Admin;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GesFer.Product.Back.Api.Controllers;

/// <summary>
/// Controlador para el dashboard administrativo en Product API
/// Expone métricas para que sean consumidas por Admin API
/// </summary>
[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;
    private readonly IConfiguration _configuration;

    public DashboardController(
        ApplicationDbContext context,
        ILogger<DashboardController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Obtiene estadísticas del sistema para el Dashboard de Admin
    /// Protegido por Internal Secret (System) o Rol Admin
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStats()
    {
        // Validación manual de Internal Secret (o usar atributo si se mueve a Common)
        // Por ahora, validamos header manualmente para no depender de Admin
        var secretHeader = Request.Headers["X-Internal-Secret"].FirstOrDefault();
        var configSecret = _configuration["InternalSecret"];

        var isAuthenticated = !string.IsNullOrEmpty(secretHeader) && secretHeader == configSecret;

        if (!isAuthenticated && !User.IsInRole("Admin"))
        {
            return Unauthorized();
        }

        try
        {
            var summary = new DashboardSummaryDto
            {
                // TotalCompanies: métrica eliminada; Companies es SSOT en Admin, Product no la expone.
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive && u.DeletedAt == null),
                TotalArticles = await _context.Articles.CountAsync(),
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                GeneratedAt = DateTime.UtcNow
            };

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }
}
