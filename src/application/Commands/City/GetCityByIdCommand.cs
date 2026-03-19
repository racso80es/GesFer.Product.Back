using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.City;

namespace GesFer.Product.Back.Application.Commands.City;

public record GetCityByIdCommand(Guid Id) : ICommand<CityDto?>;

