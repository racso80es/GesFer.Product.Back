using GesFer.Product.Back.Domain.Entities;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GesFer.Product.Back.Infrastructure.Services;

/// <summary>
/// Datos maestros locales en Product (p. ej. idiomas). Catálogo geográfico: SSOT en Admin.
/// </summary>
public class MasterDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MasterDataSeeder> _logger;

    public MasterDataSeeder(
        ApplicationDbContext context,
        ILogger<MasterDataSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Carga idiomas maestros (es, en, ca).
    /// </summary>
    public async Task SeedLanguagesAsync()
    {
        _logger.LogInformation("Cargando idiomas maestros...");

        var languages = new[]
        {
            new Language
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Español",
                Code = "es",
                Description = "Español",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Language
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "English",
                Code = "en",
                Description = "Inglés",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Language
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Català",
                Code = "ca",
                Description = "Catalán",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        foreach (var lang in languages)
        {
            var existing = await _context.Languages.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Code == lang.Code);
            if (existing == null)
            {
                _context.Languages.Add(lang);
            }
            else if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.IsActive = true;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Idiomas maestros listos");
    }

    /// <summary>
    /// Reservado: antes insertaba país/provincias/ciudades locales. El catálogo geo vive en Admin (SSOT).
    /// </summary>
    public Task SeedSpainDataAsync()
    {
        _logger.LogInformation("Catálogo geográfico en Admin API (SSOT); Product no inserta países/estados/ciudades locales.");
        return Task.CompletedTask;
    }
}
