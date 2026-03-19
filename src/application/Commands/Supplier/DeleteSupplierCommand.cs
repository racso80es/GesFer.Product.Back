using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.Supplier;

public record DeleteSupplierCommand(Guid Id) : ICommand;

