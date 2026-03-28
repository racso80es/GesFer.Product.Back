using System;
using FluentAssertions;
using GesFer.Product.Back.Domain.ValueObjects;
using Xunit;

namespace GesFer.Product.Back.UnitTests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.com")]
    [InlineData("user+tag@domain.com")]
    [InlineData("user@sub.domain.com")]
    [InlineData("1234567890@domain.com")]
    [InlineData("user@domain.co.uk")]
    [InlineData("user@domain.museum")]
    [InlineData("u@domain.com")]
    [InlineData("user@domain.io")]
    public void Create_WithValidEmail_ShouldReturnEmailInstance(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("  test@example.com  ")]
    [InlineData("\ttest@example.com\n")]
    public void Create_WithValidEmailWithWhitespace_ShouldReturnTrimmedEmailInstance(string validEmailWithWhitespace)
    {
        // Act
        var email = Email.Create(validEmailWithWhitespace);

        // Assert
        email.Value.Should().Be(validEmailWithWhitespace.Trim());
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@no-local-part.com")]
    [InlineData("no-at-sign.com")]
    [InlineData("no-domain@.com")]
    [InlineData("user@domain")]
    [InlineData("user@domain.c")] // TLD too short
    public void Create_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        Action act = () => Email.Create(invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"El formato del email '{invalidEmail.Trim()}' no es válido.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithNullOrWhiteSpace_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Act
        Action act = () => Email.Create(invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El email no puede ser nulo o vacío.*");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.com")]
    public void TryCreate_WithValidEmail_ShouldReturnTrueAndEmailInstance(string validEmail)
    {
        // Act
        var result = Email.TryCreate(validEmail, out var email);

        // Assert
        result.Should().BeTrue();
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData(null)]
    [InlineData("")]
    public void TryCreate_WithInvalidEmail_ShouldReturnFalse(string? invalidEmail)
    {
        // Act
        var result = Email.TryCreate(invalidEmail, out var email);

        // Assert
        result.Should().BeFalse();
        // Since Email is a readonly record struct, default value is an uninitialized struct.
        // We verify that the value property is null.
        email.Value.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversionFromString_WithValidEmail_ShouldCreateEmailInstance()
    {
        // Arrange
        string emailString = "test@example.com";

        // Act
        Email email = emailString;

        // Assert
        email.Value.Should().Be(emailString);
    }

    [Fact]
    public void ImplicitConversionToString_ShouldReturnStringValue()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string? emailString = email;

        // Assert
        emailString.Should().Be("test@example.com");
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM"); // Case insensitive

        // Act
        var result = email1.Equals(email2);

        // Assert
        result.Should().BeTrue();
    }
}
