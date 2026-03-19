using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Customer;

namespace GesFer.Product.Back.Application.Commands.Customer;

public record CreateCustomerCommand(CreateCustomerDto Dto) : ICommand<CustomerDto>;

