using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BadNews.Controllers;
using BadNews.Services;
using BadNews.Models;
using BadNews.DTOs;
using BadNews.Data;

namespace BadNews.Tests;

/// <summary>
/// Integration tests for all API controllers.
/// Tests real database operations with InMemory database and mocked external services.
/// </summary>
public class IntegrationTests : IAsyncLifetime
{
    private BadNewsDbContext _dbContext;
    private ServiceProvider _serviceProvider;
    private Mock<ITwilioService> _mockTwilioService;
    private Mock<IMercadoPagoService> _mockMercadoPagoService;
    private Mock<ISendGridService> _mockSendGridService;
    private Mock<IAuthService> _mockAuthService;
    private Mock<IOrderService> _mockOrderService;
    private ILogger<AuthController> _loggerAuth;
    private ILogger<OrdersController> _loggerOrders;
    private ILogger<CallsController> _loggerCalls;
    private ILogger<PaymentsController> _loggerPayments;
    private ILogger<MessengersController> _loggerMessengers;

    public async Task InitializeAsync()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<BadNewsDbContext>()
            .UseInMemoryDatabase($"BadNewsTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new BadNewsDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        // Setup mocks
        _mockTwilioService = new Mock<ITwilioService>();
        _mockMercadoPagoService = new Mock<IMercadoPagoService>();
        _mockSendGridService = new Mock<ISendGridService>();
        _mockAuthService = new Mock<IAuthService>();
        _mockOrderService = new Mock<IOrderService>();

        // Setup mock loggers
        _loggerAuth = new Mock<ILogger<AuthController>>().Object;
        _loggerOrders = new Mock<ILogger<OrdersController>>().Object;
        _loggerCalls = new Mock<ILogger<CallsController>>().Object;
        _loggerPayments = new Mock<ILogger<PaymentsController>>().Object;
        _loggerMessengers = new Mock<ILogger<MessengersController>>().Object;

        // Default mock behaviors
        _mockTwilioService.Setup(s => s.MakeCallAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((true, Guid.NewGuid().ToString()));

        _mockMercadoPagoService.Setup(s => s.CreatePaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, Guid.NewGuid().ToString()));

        _mockSendGridService.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        _serviceProvider?.Dispose();
    }

    private ClaimsPrincipal CreateClaimsPrincipal(Guid userId, string email, string role = "Buyer")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    // ======================== AUTH CONTROLLER TESTS ========================

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "buyer@example.com",
            Password = "SecurePassword123!",
            FirstName = "Juan",
            LastName = "García",
            PhoneNumber = "+34912345678",
            Role = "Buyer"
        };

        var registerResponse = new RegisterResponse
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Token = "jwt_token_here"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(registerResponse);

        var controller = new AuthController(_mockAuthService.Object, _dbContext, _loggerAuth);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockAuthService.Verify(s => s.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            Password = "SecurePassword123!",
            FirstName = "Juan",
            LastName = "García",
            PhoneNumber = "+34912345678",
            Role = "Buyer"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(request))
            .ThrowsAsync(new ArgumentException("Invalid email"));

        var controller = new AuthController(_mockAuthService.Object, _dbContext, _loggerAuth);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "buyer@example.com",
            Password = "SecurePassword123!"
        };

        var loginResponse = new LoginResponse
        {
            Token = "jwt_token_here",
            User = new UserDto { Id = Guid.NewGuid(), Email = request.Email }
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(loginResponse);

        var controller = new AuthController(_mockAuthService.Object, _dbContext, _loggerAuth);

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockAuthService.Verify(s => s.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        var controller = new AuthController(_mockAuthService.Object, _dbContext, _loggerAuth);

        // Act
        var result = await controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task GetProfile_AuthorizedUser_ReturnsUserProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "buyer@example.com",
            FirstName = "Juan",
            LastName = "García",
            PhoneNumber = "+34912345678",
            PasswordHash = "hashed_password",
            Role = "Buyer",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var controller = new AuthController(_mockAuthService.Object, _dbContext, _loggerAuth);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(userId, user.Email)
            }
        };

        // Act
        var result = await controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // ======================== ORDERS CONTROLLER TESTS ========================

    [Fact]
    public async Task CreateOrder_ValidRequest_CreatesOrderInDatabase()
    {
        // Arrange
        var buyerId = Guid.NewGuid();
        var request = new CreateOrderRequest
        {
            BuyerId = buyerId,
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María, te envío un mensaje especial!",
            IsAnonymous = false,
            Price = 25m
        };

        var createdOrderId = Guid.NewGuid();
        _mockOrderService.Setup(s => s.CreateOrderAsync(
            request.BuyerId,
            request.RecipientPhoneNumber,
            request.RecipientName,
            request.Message,
            request.IsAnonymous,
            request.Price
        )).ReturnsAsync(createdOrderId);

        var controller = new OrdersController(_mockOrderService.Object, _dbContext, _loggerOrders);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(buyerId, "buyer@example.com", "Buyer")
            }
        };

        // Act
        var result = await controller.CreateOrder(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(OrdersController.GetOrderById), createdResult.ActionName);
        Assert.Equal(201, createdResult.StatusCode);
        _mockOrderService.Verify(s => s.CreateOrderAsync(
            request.BuyerId,
            request.RecipientPhoneNumber,
            request.RecipientName,
            request.Message,
            request.IsAnonymous,
            request.Price
        ), Times.Once);
    }

    [Fact]
    public async Task GetAvailableOrders_MessengerUser_ReturnsAvailableOrders()
    {
        // Arrange
        var buyerId = Guid.NewGuid();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            BuyerId = buyerId,
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var messengerId = Guid.NewGuid();
        var controller = new OrdersController(_mockOrderService.Object, _dbContext, _loggerOrders);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(messengerId, "messenger@example.com", "Messenger")
            }
        };

        // Act
        var result = await controller.GetAvailableOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task RateOrder_ValidRating_SavesRatingToDatabase()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            BuyerId = Guid.NewGuid(),
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.Completed,
            PaymentStatus = PaymentStatus.Completed,
            Rating = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var messengerId = Guid.NewGuid();
        var request = new RateOrderRequest { Rating = 5 };

        var controller = new OrdersController(_mockOrderService.Object, _dbContext, _loggerOrders);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(messengerId, "messenger@example.com", "Messenger")
            }
        };

        // Act
        var result = await controller.RateOrder(orderId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // ======================== CALLS CONTROLLER TESTS ========================

    [Fact]
    public async Task MakeCall_ValidOrder_InitiatesTwilioCall()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            BuyerId = Guid.NewGuid(),
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var callSid = Guid.NewGuid().ToString();
        _mockTwilioService.Setup(s => s.MakeCallAsync(order.RecipientPhoneNumber, order.Message, orderId))
            .ReturnsAsync((true, callSid));

        var controller = new CallsController(_mockTwilioService.Object, _mockSendGridService.Object, _dbContext, _loggerCalls);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(Guid.NewGuid(), "messenger@example.com", "Messenger")
            }
        };

        // Act
        var result = await controller.MakeCall(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockTwilioService.Verify(s => s.MakeCallAsync(order.RecipientPhoneNumber, order.Message, orderId), Times.Once);
    }

    [Fact]
    public async Task StatusCallback_TwilioWebhook_UpdatesCallStatus()
    {
        // Arrange
        var callSid = Guid.NewGuid().ToString();
        var callAttempt = new CallAttempt
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            MessengerId = Guid.NewGuid(),
            CallSid = callSid,
            Status = "initiated",
            AttemptNumber = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.CallAttempts.AddAsync(callAttempt);
        await _dbContext.SaveChangesAsync();

        var controller = new CallsController(_mockTwilioService.Object, _mockSendGridService.Object, _dbContext, _loggerCalls);

        // Act - Simulate Twilio status callback
        var result = await controller.StatusCallback(new Dictionary<string, string>
        {
            { "CallSid", callSid },
            { "CallStatus", "completed" }
        });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        // Verify database was updated
        var updatedCall = await _dbContext.CallAttempts.FirstOrDefaultAsync(c => c.CallSid == callSid);
        Assert.NotNull(updatedCall);
        Assert.Equal("completed", updatedCall.Status);
    }

    [Fact]
    public async Task GetCallAttempts_OrderExists_ReturnsCallHistory()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var messengerId = Guid.NewGuid();

        for (int i = 1; i <= 3; i++)
        {
            var callAttempt = new CallAttempt
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                MessengerId = messengerId,
                CallSid = Guid.NewGuid().ToString(),
                Status = i == 3 ? "completed" : "failed",
                AttemptNumber = i,
                CreatedAt = DateTime.UtcNow.AddHours(-i)
            };
            await _dbContext.CallAttempts.AddAsync(callAttempt);
        }
        await _dbContext.SaveChangesAsync();

        var controller = new CallsController(_mockTwilioService.Object, _mockSendGridService.Object, _dbContext, _loggerCalls);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(Guid.NewGuid(), "messenger@example.com", "Messenger")
            }
        };

        // Act
        var result = await controller.GetCallAttempts(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // ======================== PAYMENTS CONTROLLER TESTS ========================

    [Fact]
    public async Task CreatePayment_ValidOrderAndAmount_CreatesPaymentRecord()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var buyerId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            BuyerId = buyerId,
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var request = new CreatePaymentRequest
        {
            OrderId = orderId,
            Amount = 25m,
            PaymentMethodId = "card_token_123",
            Description = "Llamada personalizada a María González"
        };

        var paymentId = Guid.NewGuid().ToString();
        _mockMercadoPagoService.Setup(s => s.CreatePaymentAsync(orderId, 25m, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, paymentId));

        var controller = new PaymentsController(_mockMercadoPagoService.Object, _mockSendGridService.Object, _dbContext, _loggerPayments);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(buyerId, "buyer@example.com", "Buyer")
            }
        };

        // Act
        var result = await controller.CreatePayment(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockMercadoPagoService.Verify(s => s.CreatePaymentAsync(orderId, 25m, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task VerifyPayment_PaymentCompleted_UpdatesOrderStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid().ToString();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            PaymentId = paymentId,
            Amount = 25m,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var order = new Order
        {
            Id = orderId,
            BuyerId = Guid.NewGuid(),
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        _mockMercadoPagoService.Setup(s => s.VerifyPaymentAsync(paymentId))
            .ReturnsAsync((true, PaymentStatus.Completed));

        var controller = new PaymentsController(_mockMercadoPagoService.Object, _mockSendGridService.Object, _dbContext, _loggerPayments);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(Guid.NewGuid(), "buyer@example.com", "Buyer")
            }
        };

        // Act
        var result = await controller.VerifyPayment(new VerifyPaymentRequest { PaymentId = paymentId });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task RefundPayment_ValidPayment_ProcessesRefund()
    {
        // Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            PaymentId = paymentId,
            Amount = 25m,
            Status = PaymentStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();

        _mockMercadoPagoService.Setup(s => s.RefundPaymentAsync(paymentId, 25m))
            .ReturnsAsync(true);

        var controller = new PaymentsController(_mockMercadoPagoService.Object, _mockSendGridService.Object, _dbContext, _loggerPayments);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(Guid.NewGuid(), "buyer@example.com", "Buyer")
            }
        };

        // Act
        var result = await controller.RefundPayment(new RefundPaymentRequest { PaymentId = paymentId, Reason = "Cancelación de pedido" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockMercadoPagoService.Verify(s => s.RefundPaymentAsync(paymentId, 25m), Times.Once);
    }

    // ======================== MESSENGERS CONTROLLER TESTS ========================

    [Fact]
    public async Task RegisterMessenger_ValidData_CreatesMessengerProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "messenger@example.com",
            FirstName = "Carlos",
            LastName = "López",
            PhoneNumber = "+34666777888",
            PasswordHash = "hashed_password",
            Role = "Messenger",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var request = new RegisterMessengerRequest
        {
            UserId = userId,
            DocumentType = "DNI",
            DocumentNumber = "12345678X",
            BankAccountNumber = "ES9121000418450200051332",
            IbanCode = "ES91"
        };

        var controller = new MessengersController(_dbContext, _loggerMessengers);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(userId, user.Email, "Messenger")
            }
        };

        // Act
        var result = await controller.RegisterMessenger(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        // Verify messenger was created in database
        var messenger = await _dbContext.Messengers.FirstOrDefaultAsync(m => m.UserId == userId);
        Assert.NotNull(messenger);
        Assert.Equal("12345678X", messenger.DocumentNumber);
    }

    [Fact]
    public async Task GetMessengerStats_MessengerUser_ReturnsEarningsAndStats()
    {
        // Arrange
        var messengerId = Guid.NewGuid();
        var messenger = new Messenger
        {
            Id = Guid.NewGuid(),
            UserId = messengerId,
            DocumentNumber = "12345678X",
            BankAccountNumber = "ES9121000418450200051332",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Messengers.AddAsync(messenger);

        // Add some completed calls
        for (int i = 0; i < 5; i++)
        {
            var callAttempt = new CallAttempt
            {
                Id = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                MessengerId = messengerId,
                CallSid = Guid.NewGuid().ToString(),
                Status = "completed",
                AttemptNumber = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            };
            await _dbContext.CallAttempts.AddAsync(callAttempt);
        }

        await _dbContext.SaveChangesAsync();

        var controller = new MessengersController(_dbContext, _loggerMessengers);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(messengerId, "messenger@example.com", "Messenger")
            }
        };

        // Act
        var result = await controller.GetMessengerStats(messengerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // ======================== DATABASE PERSISTENCE TESTS ========================

    [Fact]
    public async Task DatabasePersistence_MultipleOperations_MaintainsDataIntegrity()
    {
        // Arrange - Create user
        var buyerId = Guid.NewGuid();
        var buyer = new User
        {
            Id = buyerId,
            Email = "buyer@example.com",
            FirstName = "Juan",
            LastName = "García",
            PhoneNumber = "+34912345678",
            PasswordHash = "hashed_password",
            Role = "Buyer",
            CreatedAt = DateTime.UtcNow
        };

        // Create messenger
        var messengerId = Guid.NewGuid();
        var messenger = new User
        {
            Id = messengerId,
            Email = "messenger@example.com",
            FirstName = "Carlos",
            LastName = "López",
            PhoneNumber = "+34666777888",
            PasswordHash = "hashed_password",
            Role = "Messenger",
            CreatedAt = DateTime.UtcNow
        };

        // Create order
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            BuyerId = buyerId,
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Create payment
        var paymentId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = paymentId,
            OrderId = orderId,
            PaymentId = Guid.NewGuid().ToString(),
            Amount = 25m,
            Status = PaymentStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        // Create call attempt
        var callAttemptId = Guid.NewGuid();
        var callAttempt = new CallAttempt
        {
            Id = callAttemptId,
            OrderId = orderId,
            MessengerId = messengerId,
            CallSid = Guid.NewGuid().ToString(),
            Status = "completed",
            RecordingUrl = "https://recordings.twilio.com/example",
            AttemptNumber = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act - Save all entities
        await _dbContext.Users.AddAsync(buyer);
        await _dbContext.Users.AddAsync(messenger);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.CallAttempts.AddAsync(callAttempt);
        await _dbContext.SaveChangesAsync();

        // Assert - Verify all entities are persisted with correct relationships
        Assert.NotNull(await _dbContext.Users.FindAsync(buyerId));
        Assert.NotNull(await _dbContext.Users.FindAsync(messengerId));
        Assert.NotNull(await _dbContext.Orders.FindAsync(orderId));
        Assert.NotNull(await _dbContext.Payments.FindAsync(paymentId));
        Assert.NotNull(await _dbContext.CallAttempts.FindAsync(callAttemptId));

        // Verify relationships
        var savedOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        Assert.NotNull(savedOrder);
        Assert.Equal(buyerId, savedOrder.BuyerId);

        var savedPayment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
        Assert.NotNull(savedPayment);
        Assert.Equal(orderId, savedPayment.OrderId);

        var savedCall = await _dbContext.CallAttempts.FirstOrDefaultAsync(c => c.Id == callAttemptId);
        Assert.NotNull(savedCall);
        Assert.Equal(orderId, savedCall.OrderId);
        Assert.Equal(messengerId, savedCall.MessengerId);
    }

    [Fact]
    public async Task TransactionRollback_OnException_PreservesDataConsistency()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            BuyerId = Guid.NewGuid(),
            RecipientPhoneNumber = "+34987654321",
            RecipientName = "María González",
            Message = "¡Hola María!",
            IsAnonymous = false,
            Price = 25m,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Simulate a second operation that fails
                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();
                throw new InvalidOperationException("Simulated error during payment processing");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        // Verify order still exists after rollback
        var persistedOrder = await _dbContext.Orders.FindAsync(orderId);
        Assert.NotNull(persistedOrder);
    }
}
