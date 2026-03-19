using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.User;

public record DeleteUserCommand(Guid Id) : ICommand;

