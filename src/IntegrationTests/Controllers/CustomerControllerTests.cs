using GesFer.Product.Back.IntegrationTests.Helpers;
using FluentAssertions;
using GesFer.Product.Back.Application.DTOs.Auth;
using GesFer.Product.Back.Application.DTOs.Customer;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.Controllers;

[Collection("DatabaseStep")]
public class CustomerControllerTests
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;
    private readonly Guid _companyId = Guid.Parse("11111111-1111-1111-1111-111111111115");

    public CustomerControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
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
    public async Task GetAll_WithValidToken_ShouldReturnListOfCustomers()
    {
        var token = await GetAuthTokenAsync();

        var response = await _client.GetWithAuthAsync("/api/customer", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_WithValidToken_ShouldReturnFilteredCustomers()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Cliente Test",
            TaxId = "B11111119"
        };
        await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);

        var response = await _client.GetWithAuthAsync("/api/customer", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
        customers!.Should().NotBeEmpty();
        customers!.All(c => c.CompanyId == _companyId).Should().BeTrue();
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnCustomer()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Cliente Test GetById",
            TaxId = "B22222228"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        var customerId = createdCustomer!.Id;

        // Act
        var response = await _client.GetWithAuthAsync($"/api/customer/{customerId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(customerId);
        customer.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetWithAuthAsync($"/api/customer/{invalidId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Nuevo Cliente",
            TaxId = "B33333337",
            Address = "Calle Cliente 123",
            Phone = "912345678",
            Email = "cliente@test.com"
        };

        // Act
        var response = await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Error details: {content}");
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be(createDto.Name);
        customer.CompanyId.Should().Be(_companyId);
    }

    // Create_WithInvalidCompanyId: ya no aplica; CompanyId viene del claim

    [Fact]
    public async Task Create_WithDuplicateName_ShouldReturnBadRequest()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Cliente Duplicado",
            TaxId = "B55555551"
        };
        await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);

        // Act - Intentar crear otro con el mismo nombre
        var response = await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Cliente Para Actualizar",
            TaxId = "B66666660"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        var customerId = createdCustomer!.Id;

        var updateDto = new UpdateCustomerDto
        {
            Name = "Cliente Actualizado",
            TaxId = "B66666660",
            Address = "Calle Actualizada 456",
            Phone = "987654321",
            Email = "actualizado@test.com",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonWithAuthAsync($"/api/customer/{customerId}", updateDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be(updateDto.Name);
        customer.Address.Should().Be(updateDto.Address);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();
        var updateDto = new UpdateCustomerDto
        {
            Name = "Cliente Test",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonWithAuthAsync($"/api/customer/{invalidId}", updateDto, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        var token = await GetAuthTokenAsync();
        var createDto = new CreateCustomerDto
        {
            CompanyId = _companyId,
            Name = "Cliente Para Eliminar",
            TaxId = "B77777779"
        };
        var createResponse = await _client.PostAsJsonWithAuthAsync("/api/customer", createDto, token);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        var customerId = createdCustomer!.Id;

        // Act
        var response = await _client.DeleteWithAuthAsync($"/api/customer/{customerId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que el cliente ya no se puede obtener
        var getResponse = await _client.GetWithAuthAsync($"/api/customer/{customerId}", token);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteWithAuthAsync($"/api/customer/{invalidId}", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
