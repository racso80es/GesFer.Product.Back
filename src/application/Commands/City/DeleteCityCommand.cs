using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.City;

public record DeleteCityCommand(Guid Id) : ICommand;

