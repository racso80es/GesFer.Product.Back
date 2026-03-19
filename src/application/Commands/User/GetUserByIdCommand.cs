using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.User;

namespace GesFer.Product.Back.Application.Commands.User;

public record GetUserByIdCommand(Guid Id) : ICommand<UserDto?>;

