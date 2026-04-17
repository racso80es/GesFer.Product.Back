using GesFer.Product.Back.Application.Commands.Group;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Group;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.Group;

public class GetGroupByIdCommandHandler : ICommandHandler<GetGroupByIdCommand, GroupDto?>
{
    private readonly ApplicationDbContext _context;

    public GetGroupByIdCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GroupDto?> HandleAsync(GetGroupByIdCommand command, CancellationToken cancellationToken = default)
    {
        var group = await _context.Groups
            .Where(g => g.Id == command.Id)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return group;
    }
}

