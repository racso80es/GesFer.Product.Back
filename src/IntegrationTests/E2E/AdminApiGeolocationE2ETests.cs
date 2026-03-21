using System.Net;
using FluentAssertions;
using GesFer.Product.Back.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.E2E;

/// <summary>
/// E2E contra Admin API real: salud HTTP y cadena geográfica según
/// <c>docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json</c>
/// (rutas bajo <c>/api/geolocation/*</c>, mismas que <see cref="AdminApiClient"/>).
/// Variables opcionales: <c>E2E_ADMIN_API_BASE_URL</c>, <c>E2E_ADMIN_INTERNAL_SECRET</c>.
/// Omitir en CI sin Admin: <c>E2E_ADMIN_DISABLED=1</c>.
/// Contrato SSOT: por defecto obligatorio. Contra Admin sin <c>/api/geolocation/*</c> (solo <c>/api/Country</c> legacy),
/// ponga <c>E2E_ADMIN_SSOT_REQUIRED=0</c> para omitir la prueba estricta (solo desarrollo; no sustituye desplegar Admin alineado).
/// </summary>
[Trait("Category", "E2E")]
public sealed class AdminApiGeolocationE2ETests
{
    private const string DefaultBaseUrl = "http://localhost:5020";
    private const string DefaultSecret = "dev-internal-secret-change-in-production";

    private static bool IsDisabled() =>
        string.Equals(Environment.GetEnvironmentVariable("E2E_ADMIN_DISABLED"), "1", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Environment.GetEnvironmentVariable("E2E_ADMIN_DISABLED"), "true", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Por defecto true: exige <c>GET /api/geolocation/countries</c>. Con <c>0</c>/<c>false</c> se omite vía <see cref="Skip"/>.
    /// </summary>
    private static bool SsotContractRequired()
    {
        var v = Environment.GetEnvironmentVariable("E2E_ADMIN_SSOT_REQUIRED")?.Trim();
        if (string.IsNullOrEmpty(v))
            return true;
        return !string.Equals(v, "0", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(v, "false", StringComparison.OrdinalIgnoreCase);
    }

    private static (string BaseUrl, string Secret) GetEnv()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_ADMIN_API_BASE_URL")?.Trim();
        if (string.IsNullOrEmpty(baseUrl))
            baseUrl = DefaultBaseUrl;
        var secret = Environment.GetEnvironmentVariable("E2E_ADMIN_INTERNAL_SECRET")?.Trim();
        if (string.IsNullOrEmpty(secret))
            secret = DefaultSecret;
        return (baseUrl, secret);
    }

    private static (AdminApiClient Client, HttpClient Http) CreateAdminApiClient()
    {
        var (baseUrl, secret) = GetEnv();
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/") };
        var logger = LoggerFactory.Create(_ => { }).CreateLogger<AdminApiClient>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["InternalSecret"] = secret
        }).Build();
        return (new AdminApiClient(httpClient, logger, config), httpClient);
    }

    [SkippableFact]
    public async Task Health_ShouldReturnSuccess()
    {
        Skip.If(IsDisabled());
        var (baseUrl, secret) = GetEnv();
        using var http = new HttpClient();
        http.DefaultRequestHeaders.TryAddWithoutValidation("X-Internal-Secret", secret);
        var response = await http.GetAsync($"{baseUrl.TrimEnd('/')}/health");
        response.IsSuccessStatusCode.Should().BeTrue("Admin API debe responder 2xx en /health");
    }

    /// <summary>
    /// Garantiza: (1) existe el contrato SSOT <c>GET api/geolocation/countries</c> en Admin;
    /// (2) <see cref="AdminApiClient"/> deserializa y recorre país → estado → ciudad → CP con coherencia de ids;
    /// (3) país inexistente → null.
    /// </summary>
    [SkippableFact]
    public async Task Geolocation_SsotContract_ShouldHold_ViaAdminApiClient()
    {
        Skip.If(IsDisabled());
        Skip.If(!SsotContractRequired(), "E2E_ADMIN_SSOT_REQUIRED=0: omitida prueba de contrato /api/geolocation/*.");
        var (baseUrl, secret) = GetEnv();
        using (var probe = new HttpClient())
        {
            probe.DefaultRequestHeaders.TryAddWithoutValidation("X-Internal-Secret", secret);
            var probeResp = await probe.GetAsync($"{baseUrl.TrimEnd('/')}/api/geolocation/countries");
            if (probeResp.StatusCode == HttpStatusCode.NotFound)
            {
                Assert.Fail(
                    "Admin no expone GET /api/geolocation/countries (404). " +
                    "El Product usa el contrato SSOT (admin-geolocation-contract.json). " +
                    "Despliegue GesFer.Admin.Back con GeolocationController bajo /api/geolocation/*. " +
                    "Si solo ve /api/Country en swagger, la versión de Admin es anterior al contrato. " +
                    "Temporalmente: E2E_ADMIN_SSOT_REQUIRED=0 (no recomendado en CI).");
            }
        }

        var (admin, http) = CreateAdminApiClient();
        using (http)
        {
            var countries = await admin.GetGeolocationCountriesAsync();
            countries.Should().NotBeNull();
            foreach (var c in countries)
            {
                c.Id.Should().NotBeEmpty();
                c.Name.Should().NotBeNullOrWhiteSpace();
                c.Code.Should().NotBeNullOrWhiteSpace();
            }

            countries.Should().NotBeEmpty("Admin debe exponer al menos un país para validar SSOT geo");

            var country = countries[0];
            var byId = await admin.GetGeolocationCountryByIdAsync(country.Id);
            byId.Should().NotBeNull();
            byId!.Id.Should().Be(country.Id);

            var states = await admin.GetGeolocationStatesByCountryAsync(country.Id);
            states.Should().NotBeNull();
            foreach (var s in states)
                s.CountryId.Should().Be(country.Id);

            if (states.Count == 0)
            {
                (await admin.GetGeolocationCountryByIdAsync(Guid.NewGuid())).Should().BeNull();
                return;
            }

            var state = states[0];
            var cities = await admin.GetGeolocationCitiesByStateAsync(state.Id);
            cities.Should().NotBeNull();
            foreach (var c in cities)
                c.StateId.Should().Be(state.Id);

            if (cities.Count == 0)
            {
                (await admin.GetGeolocationCountryByIdAsync(Guid.NewGuid())).Should().BeNull();
                return;
            }

            var firstCity = cities[0];
            var postalCodes = await admin.GetGeolocationPostalCodesByCityAsync(firstCity.Id);
            postalCodes.Should().NotBeNull();
            foreach (var pc in postalCodes)
                pc.CityId.Should().Be(firstCity.Id);

            (await admin.GetGeolocationCountryByIdAsync(Guid.NewGuid())).Should().BeNull();
        }
    }

    /// <summary>
    /// Cuando el contrato SSOT no está desplegado en Admin, al menos comprobar que la superficie legacy responde (smoke).
    /// Solo corre si <c>E2E_ADMIN_SSOT_REQUIRED=0</c> y la ruta SSOT sigue en 404.
    /// </summary>
    [SkippableFact]
    public async Task Geolocation_LegacySurface_ShouldRespond_WhenSsotNotDeployed()
    {
        Skip.If(IsDisabled());
        Skip.If(SsotContractRequired(), "Use E2E_ADMIN_SSOT_REQUIRED=0 para activar smoke legacy junto a Admin sin /api/geolocation/*.");

        var (baseUrl, secret) = GetEnv();
        using var probe = new HttpClient();
        probe.DefaultRequestHeaders.TryAddWithoutValidation("X-Internal-Secret", secret);
        var ssot = await probe.GetAsync($"{baseUrl.TrimEnd('/')}/api/geolocation/countries");
        Skip.If(ssot.StatusCode != HttpStatusCode.NotFound,
            "Admin ya expone /api/geolocation/countries; use la prueba SSOT principal con E2E_ADMIN_SSOT_REQUIRED=1.");

        var legacy = await probe.GetAsync($"{baseUrl.TrimEnd('/')}/api/Country");
        legacy.IsSuccessStatusCode.Should().BeTrue("Con SSOT ausente, se espera GET /api/Country disponible (swagger legacy).");
    }
}
