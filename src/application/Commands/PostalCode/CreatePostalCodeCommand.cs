using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.PostalCode;

namespace GesFer.Product.Back.Application.Commands.PostalCode;

public record CreatePostalCodeCommand(CreatePostalCodeDto Dto) : ICommand<PostalCodeDto>;

