using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Customer;

namespace GesFer.Product.Back.Application.Commands.Customer;

public record UpdateCustomerCommand(Guid Id, UpdateCustomerDto Dto) : ICommand<CustomerDto>;

