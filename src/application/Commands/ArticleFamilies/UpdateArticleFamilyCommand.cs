using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.ArticleFamilies;

namespace GesFer.Product.Back.Application.Commands.ArticleFamilies;

public record UpdateArticleFamilyCommand(Guid Id, UpdateArticleFamilyDto Dto) : ICommand<ArticleFamilyDto>;
