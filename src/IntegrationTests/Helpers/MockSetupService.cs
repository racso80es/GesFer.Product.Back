using GesFer.Product.Back.Api.Services;

namespace GesFer.Product.Back.IntegrationTests.Helpers;

/// <summary>
/// Mock de ISetupService para tests. Evita ejecutar Docker en Initialize_EndpointShouldExist.
/// </summary>
public class MockSetupService : ISetupService
{
    public Task<SetupResult> InitializeEnvironmentAsync()
    {
        return Task.FromResult(new SetupResult
        {
            Success = true,
            Steps = new List<string> { "Mock: Inicialización simulada para tests" },
            Message = "Tests: Docker no ejecutado"
        });
    }
}
