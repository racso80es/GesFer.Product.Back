using GesFer.Product.Back.Application.Commands.Auth;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Auth;
using GesFer.Product.Back.Infrastructure.Services;

namespace GesFer.Product.Back.Application.Handlers.Auth;

/// <summary>
/// Handler para el comando de login
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseDto?>
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto?> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Empresa) ||
            string.IsNullOrWhiteSpace(command.Usuario) ||
            string.IsNullOrWhiteSpace(command.Contraseña))
        {
            return null;
        }

        var user = await _authService.AuthenticateAsync(
            command.Empresa,
            command.Usuario,
            command.Contraseña
        );

        if (user == null)
            return null;

        var permissions = await _authService.GetUserPermissionsAsync(user.Id);
        // TODO-default-lang-ES: idioma maestro español por defecto (D3/D4); alinear con MasterDataSeeder LanguageId es.
        var defaultSpanishLanguageId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var resolvedLanguageId = user.LanguageId ?? defaultSpanishLanguageId;

        // Cursor ID es el UserId convertido a string
        var cursorId = user.Id.ToString();

        // Generar token JWT (incluye company_id para MyCompanyController)
        var token = _jwtService.GenerateToken(
            cursorId: cursorId,
            username: user.Username,
            userId: user.Id,
            companyId: user.CompanyId,
            permissions: permissions.ToList()
        );

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CompanyId = user.CompanyId,
            CompanyName = command.Empresa,
            UserLanguageId = user.LanguageId,
            CompanyLanguageId = null,
            CountryLanguageId = defaultSpanishLanguageId,
            EffectiveLanguageId = resolvedLanguageId,
            Permissions = permissions.ToList(),
            Token = token,
            CursorId = cursorId
        };
    }
}

