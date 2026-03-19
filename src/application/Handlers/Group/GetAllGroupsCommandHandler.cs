using GesFer.Product.Back.Application.Commands.Group;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Group;
using GesFer.Product.Back.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GesFer.Product.Back.Application.Handlers.Group;

public class GetAllGroupsCommandHandler : ICommandHandler<GetAllGroupsCommand, List<GroupDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllGroupsCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GroupDto>> HandleAsync(GetAllGroupsCommand command, CancellationToken cancellationToken = default)
    {
        var groups = await _context.Groups
            .Where(g => g.DeletedAt == null)
            .OrderBy(g => g.Name)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return groups;
    }
}

