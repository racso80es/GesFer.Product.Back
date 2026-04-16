using GesFer.Product.Back.Application.Commands.PostalCode;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.PostalCode;

public class DeletePostalCodeCommandHandler : ICommandHandler<DeletePostalCodeCommand>
{
    private readonly ApplicationDbContext _context;

    public DeletePostalCodeCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(DeletePostalCodeCommand command, CancellationToken cancellationToken = default)
    {
        var postalCode = await _context.PostalCodes
            .FirstOrDefaultAsync(pc => pc.Id == command.Id, cancellationToken);

        if (postalCode == null)
            throw new InvalidOperationException($"No se encontró el código postal con ID {command.Id}");

        // Soft delete
        postalCode.DeletedAt = DateTime.UtcNow;
        postalCode.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

