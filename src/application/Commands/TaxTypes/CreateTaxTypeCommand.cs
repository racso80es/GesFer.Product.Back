using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.TaxTypes;

namespace GesFer.Product.Back.Application.Commands.TaxTypes;

public record CreateTaxTypeCommand(CreateTaxTypeDto TaxType, Guid? CompanyId = null) : ICommand<Guid>;
