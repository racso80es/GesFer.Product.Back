using FluentAssertions;
using GesFer.Product.Back.Infrastructure.Security;

namespace GesFer.Product.UnitTests.Infrastructure.Security;

public class SecretsConfigurationValidatorTests
{
    private static string LongSecret =>
        "0123456789abcdef0123456789abcdef"; // 32 chars, no placeholder

    [Fact]
    public void ValidateJwtSecretKey_Null_Throws()
    {
        var act = () => SecretsConfigurationValidator.ValidateJwtSecretKey(null);
        act.Should().Throw<InvalidOperationException>().WithMessage("*no está configurado*");
    }

    [Fact]
    public void ValidateJwtSecretKey_TooShort_Throws()
    {
        var act = () => SecretsConfigurationValidator.ValidateJwtSecretKey("short");
        act.Should().Throw<InvalidOperationException>().WithMessage("*al menos 32 caracteres*");
    }

    [Fact]
    public void ValidateJwtSecretKey_Placeholder_Throws()
    {
        var placeholder = "[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]";
        var act = () => SecretsConfigurationValidator.ValidateJwtSecretKey(placeholder);
        act.Should().Throw<InvalidOperationException>().WithMessage("*marcador de plantilla*");
    }

    [Fact]
    public void ValidateJwtSecretKey_Valid_ReturnsValue()
    {
        var result = SecretsConfigurationValidator.ValidateJwtSecretKey(LongSecret);
        result.Should().Be(LongSecret);
    }

    [Fact]
    public void ValidateInternalSecretIfPresent_NullOrEmpty_NoThrow()
    {
        SecretsConfigurationValidator.ValidateInternalSecretIfPresent(null);
        SecretsConfigurationValidator.ValidateInternalSecretIfPresent(string.Empty);
    }

    [Fact]
    public void ValidateInternalSecretIfPresent_Placeholder_Throws()
    {
        var act = () => SecretsConfigurationValidator.ValidateInternalSecretIfPresent(
            "[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]");
        act.Should().Throw<InvalidOperationException>().WithMessage("*InternalSecret*");
    }

    [Fact]
    public void ValidateInternalSecretIfPresent_ShortNonPlaceholder_NoThrow()
    {
        SecretsConfigurationValidator.ValidateInternalSecretIfPresent("test-secret");
    }
}
