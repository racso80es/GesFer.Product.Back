using GesFer.Product.Back.Application.Commands.Auth;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GesFer.Product.Back.Api.Controllers;

/// <summary>
/// Controlador para autenticación
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<LoginCommand, LoginResponseDto?> _loginHandler;
    private readonly ICommandHandler<GetUserPermissionsCommand, List<string>> _getPermissionsHandler;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor del controlador de autenticación
    /// </summary>
    public AuthController(
        ICommandHandler<LoginCommand, LoginResponseDto?> loginHandler,
        ICommandHandler<GetUserPermissionsCommand, List<string>> getPermissionsHandler,
        ILogger<AuthController> logger)
    {
        _loginHandler = loginHandler;
        _getPermissionsHandler = getPermissionsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Realiza el login del usuario
    /// </summary>
    /// <param name="request">Datos de login (Empresa, Usuario, Contraseña)</param>
    /// <returns>Información del usuario autenticado y sus permisos</returns>
    /// <remarks>
    /// Ejemplo de solicitud:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "empresa": "Empresa Demo",
    ///         "usuario": "admin",
    ///         "contraseña": "admin123"
    ///     }
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var command = new LoginCommand
            {
                // Support both English (new) and Spanish (legacy) property names
                Empresa = request.Company ?? request.Empresa ?? string.Empty,
                Usuario = request.Username ?? request.Usuario ?? string.Empty,
                Contraseña = request.Password ?? request.Contraseña ?? string.Empty
            };

            var result = await _loginHandler.HandleAsync(command);

            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            var empresa = request.Company ?? request.Empresa ?? string.Empty;
            var usuario = request.Username ?? request.Usuario ?? string.Empty;

            _logger.LogError(ex, "Error al realizar login para empresa: {Empresa}, usuario: {Usuario}. Error: {Message}",
                empresa, usuario, ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene los permisos de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de permisos del usuario</returns>
    [HttpGet("permissions/{userId}")]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        try
        {
            var command = new GetUserPermissionsCommand
            {
                UserId = userId
            };

            var permissions = await _getPermissionsHandler.HandleAsync(command);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del usuario {UserId}", userId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}


