using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.City;

namespace GesFer.Product.Back.Application.Commands.City;

public record CreateCityCommand(CreateCityDto Dto) : ICommand<CityDto>;

