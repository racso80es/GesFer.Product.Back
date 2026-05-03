using GesFer.Product.Back.Application.Commands.Customer;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.Customer;

public class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteCustomerCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.CompanyId == command.CompanyId, cancellationToken);

        if (customer == null)
            throw new InvalidOperationException($"No se encontró el cliente con ID {command.Id}");

        // Soft delete
        customer.DeletedAt = DateTime.UtcNow;
        customer.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

