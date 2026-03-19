using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.TaxTypes;

public record DeleteTaxTypeCommand(Guid Id, Guid? CompanyId = null) : ICommand;
