using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.ArticleFamilies;

namespace GesFer.Product.Back.Application.Commands.ArticleFamilies;

public record GetArticleFamilyByIdCommand(Guid Id, Guid? CompanyId = null) : ICommand<ArticleFamilyDto?>;
