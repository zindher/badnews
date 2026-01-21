using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Services;
using BadNews.Models;
using BadNews.Data;

namespace BadNews.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ITimezoneService _timezoneService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ITimezoneService timezoneService, BadNewsDbContext dbContext, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _timezoneService = timezoneService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Validar horario preferido
            if (!string.IsNullOrWhiteSpace(request.PreferredCallTime) && !_timezoneService.IsValidCallTime(request.PreferredCallTime))
            {
                return BadRequest(new { success = false, message = "El horario máximo permitido es 21:00 (9 PM)" });
            }

            // Detectar zona horaria del receptor basada en su estado
            var recipientTimezone = _timezoneService.GetTimezoneByState(request.RecipientState);

            var orderId = await _orderService.CreateOrderAsync(
                request.BuyerId,
                request.RecipientPhoneNumber,
                request.RecipientName,
                request.Message,
                request.IsAnonymous,
                request.Price,
                request.PreferredCallTime,
                recipientTimezone,
                request.RecipientState,
                request.RecipientEmail
            );

            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize(Roles = "Messenger")]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableOrders()
    {
        try
        {
            var orders = await _dbContext.Orders
                .Where(o => o.Status == OrderStatus.Pending && o.PaymentStatus == PaymentStatus.Completed)
                .Select(o => new
                {
                    o.Id,
                    o.RecipientName,
                    o.RecipientPhoneNumber,
                    o.Message,
                    o.IsAnonymous,
                    o.Price,
                    o.PreferredCallTime,
                    o.RecipientTimezone,
                    o.CreatedAt
                })
                .ToListAsync();

            return Ok(new { success = true, data = orders });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available orders");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Messenger)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize(Roles = "Buyer")]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var orders = await _dbContext.Orders
                .Where(o => o.BuyerId == parsedUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(new { success = true, data = orders });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user orders");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize(Roles = "Messenger")]
    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptOrder(Guid id)
    {
        try
        {
            var messengerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(messengerId, out var parsedMessengerId))
                return Unauthorized();

            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Order is not available");

            order.MessengerId = parsedMessengerId;
            order.Status = OrderStatus.Assigned;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order {id} accepted by messenger {messengerId}");

            return Ok(new { success = true, message = "Order accepted" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting order");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize]
    [HttpPut("{id}/rate")]
    public async Task<IActionResult> RateOrder(Guid id, [FromBody] RateOrderRequest request)
    {
        try
        {
            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating must be between 1 and 5");

            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Rating = request.Rating;
            order.RatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order {id} rated {request.Rating} stars");

            return Ok(new { success = true, message = "Order rated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating order");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            await _orderService.UpdateOrderStatusAsync(id, request.Status);
            return Ok(new { success = true, message = "Order status updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}

public class CreateOrderRequest
{
    public string BuyerId { get; set; } = null!;
    public string RecipientPhoneNumber { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string? RecipientEmail { get; set; } // Optional: for email fallback notification
    public string Message { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public decimal Price { get; set; }
    public string? PreferredCallTime { get; set; } // HH:MM format
    public string? RecipientState { get; set; } // For timezone mapping
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = null!;
}

public class RateOrderRequest
{
    public int Rating { get; set; }
}
