using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GesFer.Product.Back.Infrastructure.DTOs;
using Xunit;

namespace GesFer.Product.Back.E2ETests;

/// <summary>
/// E2E contra API real (por defecto <c>http://localhost:5020</c>): <c>GET/PUT /api/MyCompany</c>.
/// Requiere Admin API y BD alineados con seeds (empresa demo). Ejecutar: <c>dotnet test --filter Category=E2E</c>.
/// </summary>
[Trait("Category", "E2E")]
public class MyCompanyE2ETests
{
    private static readonly Guid DemoCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111115");

    private static HttpClient CreateUnauthenticatedClient() =>
        new() { BaseAddress = new Uri(E2EApiProbe.BaseUrl + "/", UriKind.Absolute), Timeout = TimeSpan.FromSeconds(60) };

    [SkippableFact]
    public async Task GetMyCompany_WithoutToken_Returns401()
    {
        await E2EApiProbe.EnsureReachableOrSkipAsync();
        using var client = CreateUnauthenticatedClient();
        var response = await client.GetAsync("api/MyCompany");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [SkippableFact]
    public async Task GetMyCompany_WithToken_Returns200_AndDemoCompany()
    {
        await E2EApiProbe.EnsureReachableOrSkipAsync();
        using var client = CreateUnauthenticatedClient();
        var token = await E2EAuth.LoginBearerTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/MyCompany");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<AdminCompanyDto>();
        company.Should().NotBeNull();
        company!.Id.Should().Be(DemoCompanyId);
        company.Name.Should().Be("Empresa Demo");
    }

    [SkippableFact]
    public async Task PutMyCompany_WithToken_RoundTrip_Returns200()
    {
        await E2EApiProbe.EnsureReachableOrSkipAsync();
        using var client = CreateUnauthenticatedClient();
        var token = await E2EAuth.LoginBearerTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var get = await client.GetAsync("api/MyCompany");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var current = await get.Content.ReadFromJsonAsync<AdminCompanyDto>();
        current.Should().NotBeNull();

        var update = new AdminUpdateCompanyDto
        {
            Name = current!.Name,
            TaxId = current.TaxId,
            Address = current.Address,
            Phone = current.Phone,
            Email = current.Email,
            PostalCodeId = current.PostalCodeId,
            CityId = current.CityId,
            StateId = current.StateId,
            CountryId = current.CountryId,
            LanguageId = current.LanguageId,
            IsActive = current.IsActive
        };

        var put = await client.PutAsJsonAsync("api/MyCompany", update);
        put.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await put.Content.ReadFromJsonAsync<AdminCompanyDto>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(current.Id);
        body.Name.Should().Be(current.Name);
    }
}
