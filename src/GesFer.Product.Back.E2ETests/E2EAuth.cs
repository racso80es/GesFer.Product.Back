using System.Net.Http.Json;
using GesFer.Product.Back.Application.DTOs.Auth;

namespace GesFer.Product.Back.E2ETests;

internal static class E2EAuth
{
    public static string Company =>
        Environment.GetEnvironmentVariable("E2E_LOGIN_COMPANY")?.Trim() is { Length: > 0 } c
            ? c
            : "Empresa Demo";

    public static string Username =>
        Environment.GetEnvironmentVariable("E2E_LOGIN_USER")?.Trim() is { Length: > 0 } u
            ? u
            : "admin";

    public static string Password =>
        Environment.GetEnvironmentVariable("E2E_LOGIN_PASSWORD")?.Trim() is { Length: > 0 } p
            ? p
            : "admin123";

    public static async Task<string> LoginBearerTokenAsync(HttpClient client, CancellationToken cancellationToken = default)
    {
        var body = new { Company = Company, Username = Username, Password = Password };

        var response = await client
            .PostAsJsonAsync("api/auth/login", body, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (string.IsNullOrEmpty(dto?.Token))
            throw new InvalidOperationException("Login no devolvió Token.");

        return dto.Token;
    }
}
