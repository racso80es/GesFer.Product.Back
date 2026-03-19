using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.State;

public record DeleteStateCommand(Guid Id) : ICommand;

