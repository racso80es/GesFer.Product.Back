using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Supplier;

namespace GesFer.Product.Back.Application.Commands.Supplier;

public record CreateSupplierCommand(CreateSupplierDto Dto) : ICommand<SupplierDto>;

