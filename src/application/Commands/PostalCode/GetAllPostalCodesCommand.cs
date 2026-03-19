using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.PostalCode;

namespace GesFer.Product.Back.Application.Commands.PostalCode;

public record GetAllPostalCodesCommand(Guid? CityId = null, Guid? StateId = null, Guid? CountryId = null) : ICommand<List<PostalCodeDto>>;

