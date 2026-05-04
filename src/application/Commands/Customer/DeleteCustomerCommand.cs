using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.Customer;

public record DeleteCustomerCommand(Guid Id, Guid CompanyId) : ICommand;

