using BadNews.Data;
using BadNews.Models;
using Microsoft.EntityFrameworkCore;

namespace BadNews.Services;

public class OrderService : IOrderService
{
    private readonly BadNewsDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(BadNewsDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateOrderAsync(
        string buyerId,
        string recipientPhone,
        string recipientName,
        string message,
        bool isAnonymous,
        decimal price,
        string? preferredCallTime = null,
        string? recipientTimezone = null,
        string? recipientState = null,
        string? recipientEmail = null)
    {
        try
        {
            var order = new Order
            {
                BuyerId = Guid.Parse(buyerId),
                RecipientPhoneNumber = recipientPhone,
                RecipientName = recipientName,
                Message = message,
                IsAnonymous = isAnonymous,
                Price = price,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PreferredCallTime = preferredCallTime,
                RecipientTimezone = recipientTimezone,
                RecipientState = recipientState,
                RecipientEmail = recipientEmail,
                CallAttempts = 0,
                RetryDay = 0,
                DailyAttempts = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Order created: {order.Id} for buyer: {buyerId} in state: {recipientState} with email fallback: {!string.IsNullOrEmpty(recipientEmail)}");
            return order.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            throw;
        }
    }

    public async Task AssignOrderAsync(int orderId, string messengerId)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            order.MessengerId = Guid.Parse(messengerId);
            order.AcceptedMessengerId = Guid.Parse(messengerId);
            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order {orderId} assigned to messenger {messengerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning order");
            throw;
        }
    }

    public async Task<List<object>> GetAvailableOrdersAsync()
    {
        try
        {
            var orders = await _context.Orders
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

            return orders.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available orders");
            throw;
        }
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            order.Status = Enum.Parse<OrderStatus>(status);
            order.UpdatedAt = DateTime.UtcNow;

            if (status == nameof(OrderStatus.Completed))
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order {orderId} status updated to {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            throw;
        }
    }
}
