using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.TaxTypes;

namespace GesFer.Product.Back.Application.Commands.TaxTypes;

public record GetAllTaxTypesCommand(Guid? CompanyId = null) : ICommand<List<TaxTypeDto>>;
