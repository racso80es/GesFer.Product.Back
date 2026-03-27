using GesFer.Product.Back.IntegrationTests.Helpers;
using FluentAssertions;
using GesFer.Product.Back.Application.DTOs.Auth;
using GesFer.Product.Back.Application.DTOs.Supplier;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.Controllers;

[Collection("DatabaseStep")]
public class SupplierControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;
    private readonly Guid _companyId = Guid.Parse("11111111-1111-1111-1111-111111111115");

    public SupplierControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var token = await GetAuthTokenAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new LoginRequestDto
        {
            Empresa = "Empresa Demo",
            Usuario = "admin",
            Contraseña = "admin123"
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return loginResponse!.Token;
    }

    [Fact]
    public async Task GetAll_WithValidToken_ShouldReturnListOfSuppliers()
    {
        var token = await GetAuthTokenAsync();

        var response = await _client.GetWithAuthAsync("/api/supplier", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var suppliers = await response.Content.ReadFromJsonAsync<List<SupplierDto>>();
        suppliers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_WithValidToken_ShouldReturnFilteredSuppliers()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Proveedor Test",
            TaxId = "B11111119"
        };
        await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);

        var response = await _client.GetWithAuthAsync("/api/supplier", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var suppliers = await response.Content.ReadFromJsonAsync<List<SupplierDto>>();
        suppliers.Should().NotBeNull();
        suppliers!.Should().NotBeEmpty();
        suppliers!.All(s => s.CompanyId == _companyId).Should().BeTrue();
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnSupplier()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Proveedor Test GetById",
            TaxId = "B22222228"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);
        var createdSupplier = await createResponse.Content.ReadFromJsonAsync<SupplierDto>();
        var supplierId = createdSupplier!.Id;

        // Act
        var response = await _client.GetWithAuthAsync($"/api/supplier/{supplierId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var supplier = await response.Content.ReadFromJsonAsync<SupplierDto>();
        supplier.Should().NotBeNull();
        supplier!.Id.Should().Be(supplierId);
        supplier.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetWithAuthAsync($"/api/supplier/{invalidId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Nuevo Proveedor",
            TaxId = "B33333337",
            Address = "Calle Proveedor 123",
            Phone = "912345678",
            Email = "proveedor@test.com"
        };

        // Act
        var response = await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var supplier = await response.Content.ReadFromJsonAsync<SupplierDto>();
        supplier.Should().NotBeNull();
        supplier!.Name.Should().Be(createDto.Name);
        supplier.CompanyId.Should().Be(_companyId);
    }

    // Create_WithInvalidCompanyId: ya no aplica; CompanyId viene del claim

    [Fact]
    public async Task Create_WithDuplicateName_ShouldReturnBadRequest()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Proveedor Duplicado",
            TaxId = "B55555551"
        };
        await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);

        // Act - Intentar crear otro con el mismo nombre
        var response = await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Proveedor Para Actualizar",
            TaxId = "B66666660"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);
        var createdSupplier = await createResponse.Content.ReadFromJsonAsync<SupplierDto>();
        var supplierId = createdSupplier!.Id;

        var updateDto = new UpdateSupplierDto
        {
            Name = "Proveedor Actualizado",
            TaxId = "B66666660",
            Address = "Calle Actualizada 456",
            Phone = "987654321",
            Email = "actualizado@test.com",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonWithAuthAsync($"/api/supplier/{supplierId}", updateDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var supplier = await response.Content.ReadFromJsonAsync<SupplierDto>();
        supplier.Should().NotBeNull();
        supplier!.Name.Should().Be(updateDto.Name);
        supplier.Address.Should().Be(updateDto.Address);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();
        var updateDto = new UpdateSupplierDto
        {
            Name = "Proveedor Test",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonWithAuthAsync($"/api/supplier/{invalidId}", updateDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateSupplierDto
        {
            CompanyId = _companyId,
            Name = "Proveedor Para Eliminar",
            TaxId = "B77777779"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/supplier", createDto, token);
        var createdSupplier = await createResponse.Content.ReadFromJsonAsync<SupplierDto>();
        var supplierId = createdSupplier!.Id;

        // Act
        var response = await _client.DeleteWithAuthAsync($"/api/supplier/{supplierId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que el proveedor ya no se puede obtener
        var getResponse = await _client.GetWithAuthAsync($"/api/supplier/{supplierId}", token);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteWithAuthAsync($"/api/supplier/{invalidId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
