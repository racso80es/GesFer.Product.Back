using GesFer.Product.Back.Application.Commands.ArticleFamilies;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.ArticleFamilies;

public class DeleteArticleFamilyCommandHandler : ICommandHandler<DeleteArticleFamilyCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteArticleFamilyCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(DeleteArticleFamilyCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ArticleFamilies
            .FirstOrDefaultAsync(af => af.Id == command.Id && af.DeletedAt == null, cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"No se encontró la familia de artículos con ID {command.Id}.");

        entity.DeletedAt = DateTime.UtcNow;
        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
