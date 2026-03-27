using FluentAssertions;
using GesFer.Product.Back.Application.DTOs.User;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace GesFer.Product.Back.IntegrationTests.Controllers;

[Collection("DatabaseStep")]
public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;
    private readonly Guid _testCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111115");

    public UserControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.AdminToken);
    }

    [Fact]
    public async Task GetAll_WithValidToken_ShouldReturnListOfUsers()
    {
        var response = await _client.GetAsync("/api/user");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
        users!.Should().NotBeEmpty();
        users!.All(u => u.CompanyId == _testCompanyId).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/user");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnUser()
    {
        var userId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var response = await _client.GetAsync($"/api/user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId);
        user.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        var invalidId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/user/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId, // Será ignorado; controller usa claim
            Username = "nuevo_usuario",
            Password = "password123",
            FirstName = "Nuevo",
            LastName = "Usuario",
            Email = "nuevo@empresa.com",
            Phone = "912345678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Username.Should().Be(createDto.Username);
        user.FirstName.Should().Be(createDto.FirstName);
        user.CompanyId.Should().Be(_testCompanyId);
    }

    [Fact]
    public async Task Create_WithDuplicateUsername_ShouldReturnBadRequest()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = "admin", // Username duplicado
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Create_WithInvalidCompanyId: ya no aplica; CompanyId viene del claim, no del body

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = $"usuario_test_update_{Guid.NewGuid():N}",
            Password = "admin123",
            FirstName = "Usuario",
            LastName = "Test Update",
            Email = $"testupdate_{Guid.NewGuid():N}@empresa.com",
            Phone = "911111111"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/user", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created,
            "El usuario de test debería crearse correctamente");
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        var userId = createdUser!.Id;

        var updateDto = new UpdateUserDto
        {
            Username = "usuario_test_update_actualizado",
            FirstName = "Usuario",
            LastName = "Test Update Actualizado",
            Email = "testupdate_actualizado@empresa.com",
            Phone = "999999999",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            $"La actualización debería devolver OK, pero devolvió {response.StatusCode}. " +
            $"Respuesta: {await response.Content.ReadAsStringAsync()}");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId); // Verificar que se actualizó el usuario correcto
        user.FirstName.Should().Be(updateDto.FirstName);
        user.Email.Should().Be(updateDto.Email);
    }

    [Fact]
    public async Task Update_WithPassword_ShouldUpdatePassword()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = $"usuario_test_password_{Guid.NewGuid():N}",
            Password = "admin123",
            FirstName = "Usuario",
            LastName = "Test Password",
            Email = $"testpassword_{Guid.NewGuid():N}@empresa.com",
            Phone = "922222222"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/user", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created,
            "El usuario de test debería crearse correctamente");
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        var userId = createdUser!.Id;

        var updateDto = new UpdateUserDto
        {
            Username = createdUser.Username, // Mantener el mismo username
            Password = "nueva_password",
            FirstName = "Usuario",
            LastName = "Test Password",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            $"La actualización debería devolver OK, pero devolvió {response.StatusCode}. " +
            $"Respuesta: {await response.Content.ReadAsStringAsync()}");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId); // Verificar que se actualizó el usuario correcto
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        var invalidId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            Username = "test",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/user/{invalidId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = "usuario_eliminar",
            Password = "password123",
            FirstName = "Eliminar",
            LastName = "Usuario"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/user", createDto);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        var userId = createdUser!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que el usuario ya no se puede obtener
        var getResponse = await _client.GetAsync($"/api/user/{userId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/user/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Test2P2_CreateUser_WithAllProperties_ShouldValidateAllFields()
    {
        var languageId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = "usuario_test2p2",
            Password = "password123",
            FirstName = "Usuario",
            LastName = "Test 2P2",
            Email = "usuario2p2@empresa.com",
            Phone = "912345678",
            Address = "Calle Usuario 123",
            PostalCodeId = null,
            CityId = null,
            StateId = null,
            CountryId = null,
            LanguageId = languageId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();

        // Validar todas las propiedades (CompanyId viene del claim)
        user!.CompanyId.Should().Be(_testCompanyId);
        user.Username.Should().Be(createDto.Username);
        user.FirstName.Should().Be(createDto.FirstName);
        user.LastName.Should().Be(createDto.LastName);
        user.Email.Should().Be(createDto.Email);
        user.Phone.Should().Be(createDto.Phone);
        user.Address.Should().Be(createDto.Address);
        user.PostalCodeId.Should().Be(createDto.PostalCodeId);
        user.CityId.Should().Be(createDto.CityId);
        user.StateId.Should().Be(createDto.StateId);
        user.CountryId.Should().Be(createDto.CountryId);
        user.LanguageId.Should().Be(createDto.LanguageId);
        user.IsActive.Should().BeTrue(); // Por defecto debe ser activo
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.CompanyName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Test2P2_UpdateUser_WithAllProperties_ShouldValidateAllFields()
    {
        var createDto = new CreateUserDto
        {
            CompanyId = _testCompanyId,
            Username = "usuario_original2p2",
            Password = "password123",
            FirstName = "Usuario",
            LastName = "Original",
            Email = "original2p2@empresa.com",
            Phone = "911111111"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/user", createDto);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        var userId = createdUser!.Id;

        // Actualizar con todas las propiedades
        var languageId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var updateDto = new UpdateUserDto
        {
            Username = "usuario_actualizado2p2",
            Password = "nueva_password_123",
            FirstName = "Usuario",
            LastName = "Actualizado 2P2",
            Email = "actualizado2p2@empresa.com",
            Phone = "922222222",
            Address = "Calle Actualizada 456",
            PostalCodeId = null,
            CityId = null,
            StateId = null,
            CountryId = null,
            LanguageId = languageId,
            IsActive = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();

        // Validar todas las propiedades
        user!.Id.Should().Be(userId);
        user.Username.Should().Be(updateDto.Username);
        user.FirstName.Should().Be(updateDto.FirstName);
        user.LastName.Should().Be(updateDto.LastName);
        user.Email.Should().Be(updateDto.Email);
        user.Phone.Should().Be(updateDto.Phone);
        user.Address.Should().Be(updateDto.Address);
        user.PostalCodeId.Should().Be(updateDto.PostalCodeId);
        user.CityId.Should().Be(updateDto.CityId);
        user.StateId.Should().Be(updateDto.StateId);
        user.CountryId.Should().Be(updateDto.CountryId);
        user.LanguageId.Should().Be(updateDto.LanguageId);
        user.IsActive.Should().Be(updateDto.IsActive);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.CompanyId.Should().Be(_testCompanyId); // No debe cambiar
    }
}
