using Microsoft.AspNetCore.Mvc;

namespace GesFer.Product.Back.Api.Controllers;

/// <summary>
/// Controlador para verificar el estado de la API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="HealthController"/>
    /// </summary>
    public HealthController()
    {
    }

    /// <summary>
    /// Verifica el estado de la API
    /// </summary>
    /// <returns>Estado actual del servicio</returns>
    [HttpGet]
    [ProducesResponseType<object>(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}


