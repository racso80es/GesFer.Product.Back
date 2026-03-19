using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.State;

namespace GesFer.Product.Back.Application.Commands.State;

public record UpdateStateCommand(Guid Id, UpdateStateDto Dto) : ICommand<StateDto>;

