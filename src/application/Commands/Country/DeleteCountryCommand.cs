using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.Country;

public record DeleteCountryCommand(Guid Id) : ICommand;

