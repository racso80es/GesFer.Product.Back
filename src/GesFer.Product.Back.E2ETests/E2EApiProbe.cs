using Xunit;

namespace GesFer.Product.Back.E2ETests;

/// <summary>
/// Sonda única para no ejecutar E2E cuando la API no está levantada (p. ej. CI sin Docker).
/// </summary>
internal static class E2EApiProbe
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static bool? _reachable;

    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("E2E_BASE_URL")?.Trim().TrimEnd('/') is { Length: > 0 } u
            ? u
            : "http://localhost:5020";

    public static async Task EnsureReachableOrSkipAsync(CancellationToken cancellationToken = default)
    {
        await Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_reachable == false)
            {
                Skip.If(true, $"API no disponible en {BaseUrl}. Levanta la API o define E2E_BASE_URL.");
            }

            if (_reachable == true)
                return;

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var response = await client
                    .GetAsync(new Uri(new Uri(BaseUrl + "/", UriKind.Absolute), "health"), cancellationToken)
                    .ConfigureAwait(false);
                _reachable = response.IsSuccessStatusCode;
            }
            catch
            {
                _reachable = false;
            }

            Skip.If(_reachable != true, $"API no disponible en {BaseUrl}. Levanta la API o define E2E_BASE_URL.");
        }
        finally
        {
            Gate.Release();
        }
    }
}
