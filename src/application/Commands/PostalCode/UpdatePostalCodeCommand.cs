using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.PostalCode;

namespace GesFer.Product.Back.Application.Commands.PostalCode;

public record UpdatePostalCodeCommand(Guid Id, UpdatePostalCodeDto Dto) : ICommand<PostalCodeDto>;

