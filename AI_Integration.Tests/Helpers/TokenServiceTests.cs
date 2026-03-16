using System;
using System.Text;
using AI_Integration.Helpers;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Moq;
using Xunit;

namespace AI_Integration.Tests.Helpers;

public class TokenServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _tokenService = new TokenService(_mockUnitOfWork.Object);
    }

    [Theory]
    [InlineData("android")]
    [InlineData("ios")]
    [InlineData("pwa")]
    public void VerifyStringContent_WithValidToken_ReturnsTrue(string platform)
    {
        // Arrange
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
        string tokenString = $"V!tin3rar!0@2024-{{{{{platform}}}}}${{{{{timestamp}}}}}";
        byte[] inputToken = Encoding.ASCII.GetBytes(tokenString);

        // Act
        bool result = _tokenService.VerifyStringContent(inputToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyStringContent_WithInvalidPlatform_ReturnsFalse()
    {
        // Arrange
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
        string tokenString = $"V!tin3rar!0@2024-{{{{windows}}}}}${{{{{timestamp}}}}}";
        byte[] inputToken = Encoding.ASCII.GetBytes(tokenString);

        // Act
        bool result = _tokenService.VerifyStringContent(inputToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyStringContent_WithInvalidTimestamp_ReturnsFalse()
    {
        // Arrange
        string tokenString = "V!tin3rar!0@2024-{{android}}${{invalid-timestamp}}";
        byte[] inputToken = Encoding.ASCII.GetBytes(tokenString);

        // Act
        bool result = _tokenService.VerifyStringContent(inputToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyStringContent_WithMalformedToken_ReturnsFalse()
    {
        // Arrange
        byte[] inputToken = Encoding.ASCII.GetBytes("malformed-token");

        // Act
        bool result = _tokenService.VerifyStringContent(inputToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyStringContent_WithEmptyToken_ReturnsFalse()
    {
        // Arrange
        byte[] inputToken = Array.Empty<byte>();

        // Act
        bool result = _tokenService.VerifyStringContent(inputToken);

        // Assert
        Assert.False(result);
    }
}
