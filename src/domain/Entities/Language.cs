using GesFer.Product.Back.Domain.Common;

namespace GesFer.Product.Back.Domain.Entities;

/// <summary>
/// Idioma maestro del sistema.
/// </summary>
public class Language : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // abreviatura ISO (es, en, ca)
    public string? Description { get; set; }

}
