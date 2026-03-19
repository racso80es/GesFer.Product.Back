using GesFer.Product.Back.Application.Commands.Auth;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Infrastructure.Services;

namespace GesFer.Product.Back.Application.Handlers.Auth;

/// <summary>
/// Handler para obtener los permisos de un usuario
/// </summary>
public class GetUserPermissionsCommandHandler : ICommandHandler<GetUserPermissionsCommand, List<string>>
{
    private readonly IAuthService _authService;

    public GetUserPermissionsCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<string>> HandleAsync(GetUserPermissionsCommand command, CancellationToken cancellationToken = default)
    {
        var permissions = await _authService.GetUserPermissionsAsync(command.UserId);
        return permissions.ToList();
    }
}

