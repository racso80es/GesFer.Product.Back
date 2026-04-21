using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using GesFer.Product.Back.IntegrationTests;

namespace GesFer.IntegrationTests.Security
{
    [Collection("DatabaseStep")]
    public class CorsIntegrationTests
    {
        private readonly HttpClient _client;

        public CorsIntegrationTests(DatabaseFixture fixture)
        {
            var factory = fixture.Factory;
            // Create a client that prevents auto-redirects to observe raw CORS headers
            var clientOptions = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = factory.CreateClient(clientOptions);
        }

        [Fact]
        public async Task Cors_Should_Allow_Configured_Origin()
        {
            // Arrange
            // We use one of the origins configured in appsettings.json for testing.
            // When Testing environment is used, it falls back to appsettings.json, which has "https://app.gesfer.com"
            var request = new HttpRequestMessage(HttpMethod.Options, "/health");
            request.Headers.Add("Origin", "https://app.gesfer.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // OPTIONS returns NoContent for successful preflight
            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeTrue();
            response.Headers.GetValues("Access-Control-Allow-Origin").Should().Contain("https://app.gesfer.com");
        }

        [Fact]
        public async Task Cors_Should_Block_Unconfigured_Origin()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/health");
            request.Headers.Add("Origin", "http://malicious-site.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            // When CORS fails, ASP.NET Core typically still processes the request if the endpoint doesn't require CORS,
            // OR returns 204 No Content for OPTIONS without CORS headers.
            // The key is that the Allow-Origin header is missing.
            response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }
    }
}
