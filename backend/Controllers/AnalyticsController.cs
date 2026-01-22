using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Data;

namespace BadNews.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(BadNewsDbContext dbContext, ILogger<AnalyticsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("orders/daily")]
    public async Task<IActionResult> GetDailyOrders([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var dailyOrders = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key,
                    count = g.Count(),
                    completed = g.Count(o => o.Status == "completed"),
                    cancelled = g.Count(o => o.Status == "cancelled")
                })
                .OrderBy(g => g.date)
                .ToListAsync();

            return Ok(dailyOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting daily orders: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("revenue/daily")]
    public async Task<IActionResult> GetDailyRevenue([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var dailyRevenue = await _dbContext.Payments
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate && p.Status == "completed")
                .GroupBy(p => p.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key,
                    amount = g.Sum(p => p.Amount),
                    count = g.Count()
                })
                .OrderBy(g => g.date)
                .ToListAsync();

            return Ok(dailyRevenue);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting daily revenue: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("messengers/performance")]
    public async Task<IActionResult> GetMessengerPerformance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var performance = await _dbContext.CallAttempts
                .Where(ca => ca.CreatedAt >= startDate && ca.CreatedAt <= endDate)
                .GroupBy(ca => new { ca.Messenger.Id, ca.Messenger.Name })
                .Select(g => new
                {
                    messengerId = g.Key.Id,
                    messengerName = g.Key.Name,
                    ordersCompleted = g.Count(ca => ca.Status == "completed"),
                    ordersTotal = g.Count(),
                    totalEarnings = g.Where(ca => ca.Status == "completed").Sum(ca => ca.Order.TotalPrice),
                    avgDuration = g.Average(ca => ca.Duration),
                    successRate = (double)g.Count(ca => ca.Status == "completed") / g.Count() * 100
                })
                .OrderByDescending(p => p.ordersCompleted)
                .ToListAsync();

            return Ok(performance);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting messenger performance: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("message-types")]
    public async Task<IActionResult> GetMessageTypeDistribution([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            // Group by message type/category (birthday, congratulations, etc.)
            var distribution = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.Category)
                .Select(g => new
                {
                    category = g.Key,
                    count = g.Count(),
                    percentage = (double)g.Count() / _dbContext.Orders.Count() * 100
                })
                .OrderByDescending(g => g.count)
                .ToListAsync();

            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting message types: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("conversion-funnel")]
    public async Task<IActionResult> GetConversionFunnel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var totalUsers = await _dbContext.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate && u.Role == "buyer")
                .CountAsync();

            var usersCreatedOrders = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Select(o => o.BuyerId)
                .Distinct()
                .CountAsync();

            var usersCompletedPayment = await _dbContext.Payments
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate && p.Status == "completed")
                .Select(p => p.Order.BuyerId)
                .Distinct()
                .CountAsync();

            var ordersCompleted = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == "completed")
                .CountAsync();

            return Ok(new
            {
                steps = new object[]
                {
                    new { step = "Users Created Account", count = totalUsers, conversionRate = 100 },
                    new { step = "Created Order", count = usersCreatedOrders, conversionRate = totalUsers > 0 ? (double)usersCreatedOrders / totalUsers * 100 : 0 },
                    new { step = "Completed Payment", count = usersCompletedPayment, conversionRate = usersCreatedOrders > 0 ? (double)usersCompletedPayment / usersCreatedOrders * 100 : 0 },
                    new { step = "Order Completed", count = ordersCompleted, conversionRate = usersCompletedPayment > 0 ? (double)ordersCompleted / usersCompletedPayment * 100 : 0 }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting conversion funnel: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("export/csv")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ExportAnalyticsCSV([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var orders = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .ToListAsync();

            var csv = "Order ID,Buyer,Messenger,Amount,Status,Created At\n";
            foreach (var order in orders)
            {
                csv += $"\"{order.Id}\",\"{order.Buyer.Name}\",\"{order.AcceptedMessenger?.Name ?? "N/A"}\",{order.TotalPrice},\"{order.Status}\",\"{order.CreatedAt:yyyy-MM-dd HH:mm:ss}\"\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"analytics_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error exporting CSV: {ex.Message}");
            return BadRequest(new { error = "Error exporting data" });
        }
    }

    [HttpGet("top-buyers")]
    public async Task<IActionResult> GetTopBuyers([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int limit = 10)
    {
        try
        {
            var topBuyers = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => new { o.BuyerId, o.Buyer.Name })
                .Select(g => new
                {
                    buyerId = g.Key.BuyerId,
                    buyerName = g.Key.Name,
                    ordersCount = g.Count(),
                    totalSpent = g.Sum(o => o.TotalPrice),
                    avgOrderValue = g.Average(o => o.TotalPrice)
                })
                .OrderByDescending(b => b.totalSpent)
                .Take(limit)
                .ToListAsync();

            return Ok(topBuyers);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting top buyers: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }
}
