using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.User;

namespace GesFer.Product.Back.Application.Commands.User;

public record CreateUserCommand(CreateUserDto Dto) : ICommand<UserDto>;

