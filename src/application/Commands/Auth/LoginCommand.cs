using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Auth;

namespace GesFer.Product.Back.Application.Commands.Auth;

/// <summary>
/// Comando para realizar el login de un usuario
/// </summary>
public class LoginCommand : ICommand<LoginResponseDto?>
{
    public string Empresa { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
}

