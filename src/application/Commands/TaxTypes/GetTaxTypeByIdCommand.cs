using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Application.DTOs.TaxTypes;

namespace GesFer.Product.Back.Application.Commands.TaxTypes;

public record GetTaxTypeByIdCommand(Guid Id, Guid? CompanyId = null) : ICommand<TaxTypeDto?>;
