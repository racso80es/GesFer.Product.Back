using FluentAssertions;
using GesFer.Product.Back.Application.DTOs.ArticleFamilies;
using GesFer.Product.Back.Application.DTOs.Auth;
using GesFer.Product.Back.Application.DTOs.TaxTypes;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.Controllers;

/// <summary>
/// Tests de integración para ArticleFamiliesController (CRUD con auth).
/// Crea un TaxType vía API antes de crear ArticleFamily para no depender de seeds.
/// </summary>
[Collection("DatabaseStep")]
public class ArticleFamiliesControllerTests
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;

    public ArticleFamiliesControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _fixture.AdminToken);
    }

    private async Task<Guid> CreateTaxTypeForCompanyAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6]; // Code max 10 chars
        var createTax = new CreateTaxTypeDto
        {
            Code = "T" + suffix,
            Name = "Test IVA 21% " + suffix,
            Description = "Para tests",
            Value = 21m
        };
        var taxResponse = await _client.PostAsJsonAsync("/api/tax-types", createTax);
        if (taxResponse.StatusCode != HttpStatusCode.Created)
        {
            var body = await taxResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"POST /api/tax-types returned {taxResponse.StatusCode}: {body}");
        }
        var taxId = await taxResponse.Content.ReadFromJsonAsync<Guid>();
        taxId.Should().NotBe(Guid.Empty);
        return taxId;
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/article-families");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200AndList()
    {

        var response = await _client.GetAsync("/api/article-families");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ArticleFamilyDto>>();
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturn201AndGetById_ShouldReturnSame()
    {
        var taxTypeId = await CreateTaxTypeForCompanyAsync();

        var createDto = new CreateArticleFamilyDto
        {
            Code = "AF-TEST-01",
            Name = "Familia Test",
            Description = "Descripción test",
            TaxTypeId = taxTypeId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/article-families", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ArticleFamilyDto>();
        created.Should().NotBeNull();
        created!.Code.Should().Be("AF-TEST-01");
        created.Name.Should().Be("Familia Test");
        created.TaxTypeId.Should().Be(taxTypeId);

        var getResponse = await _client.GetAsync($"/api/article-families/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ArticleFamilyDto>();
        fetched!.Id.Should().Be(created.Id);
        fetched.Code.Should().Be(created.Code);
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturn200()
    {
        var taxTypeId = await CreateTaxTypeForCompanyAsync();

        var createDto = new CreateArticleFamilyDto
        {
            Code = "AF-UPD-01",
            Name = "Familia Para Actualizar",
            TaxTypeId = taxTypeId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/article-families", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ArticleFamilyDto>();
        created.Should().NotBeNull();

        var updateDto = new UpdateArticleFamilyDto
        {
            Id = created!.Id,
            Code = "AF-UPD-01",
            Name = "Familia Actualizada",
            Description = "Nueva descripción",
            TaxTypeId = taxTypeId
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/article-families/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ArticleFamilyDto>();
        updated!.Name.Should().Be("Familia Actualizada");
        updated.Description.Should().Be("Nueva descripción");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturn204()
    {
        var taxTypeId = await CreateTaxTypeForCompanyAsync();

        var createDto = new CreateArticleFamilyDto
        {
            Code = "AF-DEL-01",
            Name = "Familia Para Borrar",
            TaxTypeId = taxTypeId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/article-families", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ArticleFamilyDto>();
        created.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/api/article-families/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
