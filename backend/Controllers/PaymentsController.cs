using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Services;
using BadNews.Models;
using BadNews.Data;

namespace BadNews.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMercadoPagoService mercadoPagoService, BadNewsDbContext dbContext, ILogger<PaymentsController> logger)
    {
        _mercadoPagoService = mercadoPagoService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
                return BadRequest("Invalid order ID");

            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found");

            var paymentId = await _mercadoPagoService.CreatePaymentAsync(
                order.Price,
                order.Id.ToString(),
                request.Email
            );

            var payment = new Payment
            {
                OrderId = orderId,
                BuyerId = order.BuyerId,
                Amount = order.Price,
                ExternalPaymentId = paymentId,
                Status = PaymentStatus.Processing
            };

            _dbContext.Payments.Add(payment);
            order.PaymentStatus = PaymentStatus.Processing;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                data = new { paymentId, amount = order.Price }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleMercadoPagoWebhook([FromBody] MercadoPagoWebhookRequest request)
    {
        try
        {
            _logger.LogInformation($"Mercado Pago webhook received: {request.Type}");

            if (request.Type == "payment")
            {
                var isValid = await _mercadoPagoService.VerifyPaymentAsync(request.Data.Id);
                if (isValid)
                {
                    var payment = await _dbContext.Payments
                        .FirstOrDefaultAsync(p => p.ExternalPaymentId == request.Data.Id);

                    if (payment != null)
                    {
                        payment.Status = PaymentStatus.Completed;
                        var order = await _dbContext.Orders.FindAsync(payment.OrderId);
                        if (order != null)
                        {
                            order.PaymentStatus = PaymentStatus.Completed;
                            order.Status = OrderStatus.Pending; // Ready for messenger to accept
                        }
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Payment verified: {request.Data.Id}");
                    }
                }
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentStatus(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var paymentId))
                return BadRequest("Invalid payment ID");

            var payment = await _dbContext.Payments.FindAsync(paymentId);
            if (payment == null)
                return NotFound();

            return Ok(new
            {
                success = true,
                data = new
                {
                    payment.Id,
                    payment.Amount,
                    payment.Status,
                    payment.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}

public class CreatePaymentRequest
{
    public string OrderId { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class MercadoPagoWebhookRequest
{
    public string Type { get; set; } = null!;
    public MercadoPagoData Data { get; set; } = null!;
}

public class MercadoPagoData
{
    public string Id { get; set; } = null!;
}
