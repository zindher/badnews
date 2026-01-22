using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Services;
using BadNews.Data;
using BadNews.Models;

namespace BadNews.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<AdminController> _logger;

    public AdminController(BadNewsDbContext dbContext, ILogger<AdminController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var totalUsers = await _dbContext.Users.CountAsync();
            var totalOrders = await _dbContext.Orders.CountAsync();
            var completedOrders = await _dbContext.Orders.CountAsync(o => o.Status == "completed");
            var totalRevenue = await _dbContext.Payments
                .Where(p => p.Status == "completed")
                .SumAsync(p => p.Amount);
            
            var successRate = totalOrders > 0 ? (double)completedOrders / totalOrders * 100 : 0;

            var recentOrders = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new
                {
                    o.Id,
                    o.Message,
                    o.TotalPrice,
                    o.Status,
                    BuyerName = o.Buyer.Name,
                    MessengerName = o.AcceptedMessenger.Name,
                    o.CreatedAt
                })
                .ToListAsync();

            var activeUsers = await _dbContext.Users
                .Where(u => u.LastLogin.HasValue && u.LastLogin > DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(u => u.LastLogin)
                .Take(10)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.LastLogin
                })
                .ToListAsync();

            return Ok(new
            {
                metrics = new
                {
                    totalUsers,
                    totalOrders,
                    completedOrders,
                    totalRevenue,
                    successRate = Math.Round(successRate, 2)
                },
                recentOrders,
                activeUsers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting dashboard: {ex.Message}");
            return BadRequest(new { error = "Error loading dashboard" });
        }
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var orders = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalRevenue = await _dbContext.Payments
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate && p.Status == "completed")
                .SumAsync(p => p.Amount);
            
            var completedOrders = orders.Count(o => o.Status == "completed");
            var successRate = totalOrders > 0 ? (double)completedOrders / totalOrders * 100 : 0;
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var topPerformers = await _dbContext.CallAttempts
                .Where(ca => ca.CreatedAt >= startDate && ca.CreatedAt <= endDate)
                .GroupBy(ca => ca.Messenger)
                .Select(g => new
                {
                    g.Key.Id,
                    g.Key.Name,
                    Orders = g.Count(),
                    Earnings = g.Sum(ca => ca.Order.TotalPrice),
                    Rating = g.Key.Rating
                })
                .OrderByDescending(m => m.Orders)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                metrics = new
                {
                    totalOrders,
                    totalRevenue,
                    successRate = Math.Round(successRate, 2),
                    completedOrders,
                    averageOrderValue = Math.Round(averageOrderValue, 2)
                },
                topPerformers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting analytics: {ex.Message}");
            return BadRequest(new { error = "Error loading analytics" });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var users = await _dbContext.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.CreatedAt,
                    u.LastLogin
                })
                .ToListAsync();

            var total = await _dbContext.Users.CountAsync();

            return Ok(new
            {
                users,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    total,
                    pages = (int)Math.Ceiling((double)total / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting users: {ex.Message}");
            return BadRequest(new { error = "Error loading users" });
        }
    }

    [HttpGet("disputes")]
    public async Task<IActionResult> GetDisputes()
    {
        try
        {
            var disputes = await _dbContext.Disputes
                .Include(d => d.Order)
                .Include(d => d.ReportedBy)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new
                {
                    d.Id,
                    d.OrderId,
                    d.Reason,
                    d.Description,
                    d.Status,
                    ReportedBy = d.ReportedBy.Name,
                    d.CreatedAt,
                    d.UpdatedAt
                })
                .ToListAsync();

            return Ok(disputes);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting disputes: {ex.Message}");
            return BadRequest(new { error = "Error loading disputes" });
        }
    }

    [HttpPatch("disputes/{id}/resolve")]
    public async Task<IActionResult> ResolveDispute(int id, [FromBody] DisputeResolution resolution)
    {
        try
        {
            var dispute = await _dbContext.Disputes.FindAsync(id);
            if (dispute == null)
                return NotFound(new { error = "Dispute not found" });

            dispute.Status = resolution.Status;
            dispute.Resolution = resolution.Resolution;
            dispute.UpdatedAt = DateTime.UtcNow;

            _dbContext.Disputes.Update(dispute);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Dispute resolved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resolving dispute: {ex.Message}");
            return BadRequest(new { error = "Error resolving dispute" });
        }
    }

    [HttpPost("withdraw-requests/{id}/approve")]
    public async Task<IActionResult> ApproveWithdrawRequest(int id)
    {
        try
        {
            var withdrawal = await _dbContext.Withdrawals.FindAsync(id);
            if (withdrawal == null)
                return NotFound(new { error = "Withdrawal request not found" });

            withdrawal.Status = "approved";
            withdrawal.UpdatedAt = DateTime.UtcNow;

            _dbContext.Withdrawals.Update(withdrawal);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Withdrawal approved" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error approving withdrawal: {ex.Message}");
            return BadRequest(new { error = "Error approving withdrawal" });
        }
    }

    [HttpPost("withdraw-requests/{id}/reject")]
    public async Task<IActionResult> RejectWithdrawRequest(int id, [FromBody] WithdrawalRejection rejection)
    {
        try
        {
            var withdrawal = await _dbContext.Withdrawals.FindAsync(id);
            if (withdrawal == null)
                return NotFound(new { error = "Withdrawal request not found" });

            withdrawal.Status = "rejected";
            withdrawal.RejectionReason = rejection.Reason;
            withdrawal.UpdatedAt = DateTime.UtcNow;

            _dbContext.Withdrawals.Update(withdrawal);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Withdrawal rejected" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rejecting withdrawal: {ex.Message}");
            return BadRequest(new { error = "Error rejecting withdrawal" });
        }
    }

    [HttpGet("system-logs")]
    public async Task<IActionResult> GetSystemLogs([FromQuery] int limit = 100)
    {
        try
        {
            // In a real app, this would query an actual logging database
            return Ok(new
            {
                logs = new List<dynamic>(),
                message = "System logs endpoint - integrate with logging service"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting system logs: {ex.Message}");
            return BadRequest(new { error = "Error loading logs" });
        }
    }
}

public class DisputeResolution
{
    public string Status { get; set; }
    public string Resolution { get; set; }
}

public class WithdrawalRejection
{
    public string Reason { get; set; }
}

public class Dispute
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid ReportedById { get; set; }
    public string Reason { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Order Order { get; set; }
    public User ReportedBy { get; set; }
}

public class Withdrawal
{
    public int Id { get; set; }
    public Guid MessengerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
