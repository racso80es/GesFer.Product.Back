using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.Group;

public record DeleteGroupCommand(Guid Id) : ICommand;

