using FluentAssertions;
using GesFer.Product.Back.Application.DTOs.Auth;
using GesFer.Product.Back.IntegrationTests.Helpers;
using GesFer.Product.Back.Infrastructure.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.Controllers;

/// <summary>
/// Tests de integración para MyCompanyController (GET/PUT con token que incluye company_id).
/// Usa MockAdminApiClient registrado en IntegrationTestWebAppFactory.
/// </summary>
[Collection("DatabaseStep")]
public class MyCompanyControllerTests
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;
    private static readonly Guid DemoCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111115");

    public MyCompanyControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _fixture.AdminToken);
    }

    [Fact]
    public async Task GetMyCompany_WithValidToken_ShouldReturnCompany()
    {
        var response = await _client.GetAsync("/api/MyCompany");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<AdminCompanyDto>();
        company.Should().NotBeNull();
        company!.Id.Should().Be(DemoCompanyId);
        company.Name.Should().Be("Empresa Demo");
    }

    [Fact]
    public async Task GetMyCompany_WithoutToken_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/MyCompany");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMyCompany_WithValidToken_ShouldReturn200()
    {
        var updateDto = new AdminUpdateCompanyDto
        {
            Name = "Empresa Demo", // Do not mutate the name, other tests depend on it
            TaxId = "B87654323",
            Address = "Calle Gran Vía, 2",
            Phone = "912345679",
            Email = "demo@empresa.com",
            IsActive = true
        };

        var response = await _client.PutAsJsonAsync("/api/MyCompany", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var company = await response.Content.ReadFromJsonAsync<AdminCompanyDto>();
        company.Should().NotBeNull();
        company!.Name.Should().Be("Empresa Demo");
        company.Address.Should().Be("Calle Gran Vía, 2");
    }

    [Fact]
    public async Task UpdateMyCompany_WithoutToken_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var updateDto = new AdminUpdateCompanyDto
        {
            Name = "Test",
            Address = "Calle 1",
            IsActive = true
        };
        var response = await _client.PutAsJsonAsync("/api/MyCompany", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
