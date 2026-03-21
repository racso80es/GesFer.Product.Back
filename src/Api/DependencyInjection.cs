using GesFer.Product.Back.Api.Services;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.Handlers.Auth;
using GesFer.Product.Back.Application.Handlers.PurchaseDeliveryNote;
using GesFer.Product.Back.Application.Handlers.SalesDeliveryNote;
using GesFer.Product.Back.Domain.Services;
using GesFer.Product.Back.Infrastructure.Data;
using GesFer.Product.Back.Infrastructure.Extensions;
using GesFer.Product.Back.Infrastructure.Repositories;
using GesFer.Product.Back.Infrastructure.Services;
using GesFer.Product.Back.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using System.Reflection;

namespace GesFer.Product.Back.Api;

/// <summary>
/// Configuración de inyección de dependencias
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios de la aplicación
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment? environment = null)
    {
        // Configurar DbContext
        var isDevelopment = environment?.IsDevelopment() ?? false;

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            if (environment?.IsEnvironment("Testing") == true)
            {
                options.UseInMemoryDatabase("GesFerProductDb");
            }
            else
            {
                var connectionString = configuration.GetRequiredConnectionString("DefaultConnection");
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 0)),
                    mysqlOptions =>
                    {
                        mysqlOptions.EnableStringComparisonTranslations();
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            }

            if (isDevelopment)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Repositorios genéricos (si se necesitan)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Generador de GUIDs secuenciales (MySQL optimizado)
        // Preparado para futuros proveedores (SQL Server, PostgreSQL) mediante inversión de dependencias
        services.AddSingleton<ISequentialGuidGenerator, MySqlSequentialGuidGenerator>();

        // Skill de Sanitización de Datos (Seguridad Kaizen)
        services.AddSingleton<ISensitiveDataSanitizer, SensitiveDataSanitizer>();

        // Servicios de infraestructura
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<JsonDataSeeder>();
        services.AddScoped<IAdminGeolocationValidationService, AdminGeolocationValidationService>();

        // HttpClient para comunicación con Admin API
        if (environment?.IsEnvironment("Testing") == true)
        {
            services.AddHttpClient();
            services.AddScoped<IAdminApiClient, MockAdminApiClient>();
        }
        else
        {
            var adminApiBaseUrl = configuration.GetRequired("AdminApi:BaseUrl");
            services.AddHttpClient<IAdminApiClient, GesFer.Product.Back.Infrastructure.Services.AdminApiClient>(client =>
            {
                client.BaseAddress = new Uri(adminApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            });
            // Cliente nombrado "AdminApi" para AsyncLogPublisher (envío de logs a Admin API)
            services.AddHttpClient("AdminApi", client =>
            {
                client.BaseAddress = new Uri(adminApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            });
        }

        // Servicio de logging asíncrono (Fire and Forget)
        services.AddSingleton<IAsyncLogPublisher, AsyncLogPublisher>();

        // Command Handlers - Registro automático de todos los handlers
        RegisterCommandHandlers(services);

        // Servicios de API
        services.AddScoped<ISetupService, SetupService>();

        return services;
    }

    /// <summary>
    /// Registra todos los Command Handlers automáticamente mediante reflexión
    /// </summary>
    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        var assembly = typeof(Application.Common.Interfaces.ICommandHandler<>).Assembly;
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) || 
                     i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) || 
                     i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, handlerType);
            }
        }
    }
}

