using GesFer.Product.Back.Application.Common.Interfaces;

namespace GesFer.Product.Back.Application.Commands.ArticleFamilies;

public record DeleteArticleFamilyCommand(Guid Id) : ICommand;
