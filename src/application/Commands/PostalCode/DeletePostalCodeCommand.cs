using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.PostalCode;

public record DeletePostalCodeCommand(Guid Id) : ICommand;

