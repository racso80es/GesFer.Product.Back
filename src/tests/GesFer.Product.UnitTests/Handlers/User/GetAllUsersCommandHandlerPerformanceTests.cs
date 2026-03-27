using FluentAssertions;
using GesFer.Product.Back.Application.Commands.User;
using GesFer.Product.Back.Application.Handlers.User;
using GesFer.Product.Back.Domain.Entities;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.DTOs;
using GesFer.Product.Back.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace GesFer.Product.UnitTests.Handlers.User;

public class GetAllUsersCommandHandlerPerformanceTests
{
    private readonly ITestOutputHelper _output;

    public GetAllUsersCommandHandlerPerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task HandleAsync_PerformanceBenchmark()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed 100 users, grouped into 10 different companies
        var companyIds = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            companyIds.Add(Guid.NewGuid());
        }

        for (int i = 0; i < 100; i++)
        {
            var companyId = companyIds[i % 10];
            context.Users.Add(new GesFer.Product.Back.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Username = $"user{i}",
                PasswordHash = "hash",
                FirstName = $"First{i}",
                LastName = $"Last{i}",
                IsActive = true
            });
        }
        await context.SaveChangesAsync();

        var mockAdminApiClient = new Mock<IAdminApiClient>();

        // Simulate a tiny network latency per company lookup to simulate real life behavior
        mockAdminApiClient.Setup(x => x.GetCompanyAsync(It.IsAny<Guid>()))
            .Returns<Guid>(async id =>
            {
                await Task.Delay(10);
                return new AdminCompanyDto { Id = id, Name = $"Company {id}" };
            });

        var mockLogger = new Mock<ILogger<GetAllUsersCommandHandler>>();

        var handler = new GetAllUsersCommandHandler(context, mockLogger.Object);
        var command = new GetAllUsersCommand(null);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await handler.HandleAsync(command, CancellationToken.None);
        stopwatch.Stop();

        // Assert
        _output.WriteLine($"Time taken to fetch 100 users across 10 companies: {stopwatch.ElapsedMilliseconds} ms");
        _output.WriteLine($"Number of external API calls made: {mockAdminApiClient.Invocations.Count}");

        result.Should().HaveCount(100);

        // Verify that 0 external API calls were made due to optimization
        Assert.Equal(0, mockAdminApiClient.Invocations.Count);
    }
}
