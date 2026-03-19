using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Group;

namespace GesFer.Product.Back.Application.Commands.Group;

public record GetGroupByIdCommand(Guid Id) : ICommand<GroupDto?>;

