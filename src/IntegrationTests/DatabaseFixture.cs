using Xunit;
using System.Net.Http.Json;
using GesFer.Product.Back.Application.DTOs.Auth;

namespace GesFer.Product.Back.IntegrationTests;

/// <summary>
/// Fixture compartido para todos los tests en la colección "DatabaseStep".
/// Contiene la instancia única de IntegrationTestWebAppFactory que gestiona
/// el contenedor MySQL compartido entre todas las clases de test.
/// 
/// Esta clase se instancia una sola vez al inicio de la colección de tests
/// y se limpia al finalizar todos los tests.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    public IntegrationTestWebAppFactory<GesFer.Product.Back.Api.Program> Factory { get; private set; } = null!;
    public string AdminToken { get; private set; } = string.Empty;

    /// <summary>
    /// Acceso a los servicios del contenedor de inyección de dependencias.
    /// Útil para tests que necesitan acceder directamente al DbContext u otros servicios.
    /// </summary>
    public IServiceProvider Services => Factory.Services;

    /// <summary>
    /// Inicializa el fixture compartido. Se ejecuta una sola vez antes de todos los tests.
    /// Crea y configura el IntegrationTestWebAppFactory que levanta el contenedor MySQL.
    /// </summary>
    public async Task InitializeAsync()
    {
        Factory = new IntegrationTestWebAppFactory<GesFer.Product.Back.Api.Program>();
        await Factory.InitializeAsync();

        // Autenticar al usuario admin una sola vez para toda la colección
        using var client = Factory.CreateClient();
        var loginRequest = new LoginRequestDto
        {
            Empresa = "Empresa Demo",
            Usuario = "admin",
            Contraseña = "admin123"
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        AdminToken = loginResponse!.Token;
    }

    /// <summary>
    /// Limpia el fixture compartido. Se ejecuta una sola vez después de todos los tests.
    /// Destruye el contenedor MySQL y libera los recursos.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }
}
