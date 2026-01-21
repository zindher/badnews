using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BadNews.Services;
using BadNews.Data;
using BadNews.DTOs;

namespace BadNews.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<BadNewsDbContext> _mockDbContext;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AuthService>> _mockLogger;

    public AuthServiceTests()
    {
        _mockDbContext = new Mock<BadNewsDbContext>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthService>>();
    }

    [Fact]
    public void HashPassword_Should_Return_Valid_Hash()
    {
        // Arrange
        var service = new AuthService(_mockDbContext.Object, _mockConfiguration.Object, _mockLogger.Object);
        var password = "TestPassword123!";

        // Act
        var hash = service.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var service = new AuthService(_mockDbContext.Object, _mockConfiguration.Object, _mockLogger.Object);
        var password = "TestPassword123!";
        var hash = service.HashPassword(password);

        // Act
        var result = service.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Incorrect_Password()
    {
        // Arrange
        var service = new AuthService(_mockDbContext.Object, _mockConfiguration.Object, _mockLogger.Object);
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = service.HashPassword(password);

        // Act
        var result = service.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GenerateJwtToken_Should_Return_Valid_Token()
    {
        // Arrange
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-at-least-32-characters");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("BadNews");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("BadNewsUsers");
        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");

        var service = new AuthService(_mockDbContext.Object, _mockConfiguration.Object, _mockLogger.Object);
        var user = new Models.User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+5215551234567",
            Role = Models.UserRole.Buyer
        };

        // Act
        var token = service.GenerateJwtToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT format check
    }
}

public class ValidatorTests
{
    [Fact]
    public void RegisterRequestValidator_Should_Pass_Valid_Data()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPassword123!",
            FirstName = "Juan",
            LastName = "Pérez",
            PhoneNumber = "+5215551234567",
            Role = "Buyer"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterRequestValidator_Should_Fail_Invalid_Email()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            Password = "ValidPassword123!",
            FirstName = "Juan",
            LastName = "Pérez",
            PhoneNumber = "+5215551234567",
            Role = "Buyer"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void RegisterRequestValidator_Should_Fail_Weak_Password()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "weak",  // Too short, no uppercase, no number
            FirstName = "Juan",
            LastName = "Pérez",
            PhoneNumber = "+5215551234567",
            Role = "Buyer"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void CreateOrderDtoValidator_Should_Pass_Valid_Order()
    {
        // Arrange
        var validator = new CreateOrderDtoValidator();
        var order = new CreateOrderDto
        {
            RecipientPhone = "+5215551234567",
            Message = "This is a valid message for testing purposes only",
            MessageType = "joke",
            Price = 99.99m,
            IsAnonymous = false
        };

        // Act
        var result = validator.Validate(order);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateOrderDtoValidator_Should_Fail_Invalid_Price()
    {
        // Arrange
        var validator = new CreateOrderDtoValidator();
        var order = new CreateOrderDto
        {
            RecipientPhone = "+5215551234567",
            Message = "Valid message here for testing",
            MessageType = "joke",
            Price = -50m,  // Negative price
            IsAnonymous = false
        };

        // Act
        var result = validator.Validate(order);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void CreateOrderDtoValidator_Should_Fail_Invalid_MessageType()
    {
        // Arrange
        var validator = new CreateOrderDtoValidator();
        var order = new CreateOrderDto
        {
            RecipientPhone = "+5215551234567",
            Message = "Valid message for testing purposes",
            MessageType = "invalid_type",  // Invalid type
            Price = 99.99m,
            IsAnonymous = false
        };

        // Act
        var result = validator.Validate(order);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MessageType");
    }

    [Fact]
    public void RateOrderDtoValidator_Should_Pass_Valid_Rating()
    {
        // Arrange
        var validator = new RateOrderDtoValidator();
        var rating = new RateOrderDto
        {
            Rating = 5,
            Review = "Excellent service!"
        };

        // Act
        var result = validator.Validate(rating);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RateOrderDtoValidator_Should_Fail_Invalid_Rating()
    {
        // Arrange
        var validator = new RateOrderDtoValidator();
        var rating = new RateOrderDto
        {
            Rating = 10,  // Should be 1-5
            Review = "Invalid rating"
        };

        // Act
        var result = validator.Validate(rating);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Rating");
    }
}

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<JwtService>> _mockLogger;

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<JwtService>>();
    }

    [Fact]
    public void GenerateToken_Should_Create_Valid_Jwt()
    {
        // Arrange
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-at-least-32-characters-long");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("BadNews");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("BadNewsUsers");
        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");

        var service = new JwtService(_mockConfiguration.Object, _mockLogger.Object);
        var user = new Models.User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+5215551234567",
            Role = Models.UserRole.Buyer
        };

        // Act
        var token = service.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token);
    }

    [Fact]
    public void ValidateToken_Should_Return_Valid_Principal_For_Good_Token()
    {
        // Arrange
        var secret = "test-secret-key-that-is-at-least-32-characters-long";
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns(secret);
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("BadNews");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("BadNewsUsers");
        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");

        var service = new JwtService(_mockConfiguration.Object, _mockLogger.Object);
        var user = new Models.User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+5215551234567",
            Role = Models.UserRole.Buyer
        };

        var token = service.GenerateToken(user);

        // Act
        var principal = service.ValidateToken(token);

        // Assert
        Assert.NotNull(principal);
    }

    [Fact]
    public void ValidateToken_Should_Return_Null_For_Invalid_Token()
    {
        // Arrange
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-at-least-32-characters-long");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("BadNews");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("BadNewsUsers");

        var service = new JwtService(_mockConfiguration.Object, _mockLogger.Object);
        var invalidToken = "invalid.token.here";

        // Act
        var principal = service.ValidateToken(invalidToken);

        // Assert
        Assert.Null(principal);
    }
}
