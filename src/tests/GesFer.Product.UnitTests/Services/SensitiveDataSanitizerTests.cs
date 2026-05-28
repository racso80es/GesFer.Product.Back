using System;
using FluentAssertions;
using GesFer.Product.Back.Domain.Services;
using Xunit;

namespace GesFer.Product.UnitTests.Services;

public class SensitiveDataSanitizerTests
{
    private readonly SensitiveDataSanitizer _sanitizer;

    public SensitiveDataSanitizerTests()
    {
        _sanitizer = new SensitiveDataSanitizer();
    }

    [Fact]
    public void GenerateRandomPassword_WithValidLength_ReturnsPasswordOfCorrectLength()
    {
        var password = _sanitizer.GenerateRandomPassword(15);
        password.Length.Should().Be(15);
    }

    [Fact]
    public void GenerateRandomPassword_WithInvalidLength_ThrowsArgumentOutOfRangeException()
    {
        var action = () => _sanitizer.GenerateRandomPassword(0);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GenerateRandomEmail_WithPrefix_ReturnsEmailWithPrefix()
    {
        var email = _sanitizer.GenerateRandomEmail("test");
        email.Should().StartWith("test@");
        email.Should().EndWith("@gesfer.local");
    }

    [Fact]
    public void GenerateRandomEmail_WithoutPrefix_ReturnsEmailWithDefaultPrefix()
    {
        var email = _sanitizer.GenerateRandomEmail();
        email.Should().StartWith("user_");
        email.Should().EndWith("@gesfer.local");
    }

    [Fact]
    public void Sanitize_WithInput_ReturnsSameInput()
    {
        var sanitized = _sanitizer.Sanitize("test");
        sanitized.Should().Be("test");
    }
}
