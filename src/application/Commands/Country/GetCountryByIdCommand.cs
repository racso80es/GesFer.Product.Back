using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.Country;

namespace GesFer.Product.Back.Application.Commands.Country;

public record GetCountryByIdCommand(Guid Id) : ICommand<CountryDto?>;

