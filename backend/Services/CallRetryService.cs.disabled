using BadNews.Models;
using BadNews.Data;
using Hangfire;

namespace BadNews.Services;

public interface ICallRetryService
{
    Task<bool> ScheduleRetryAsync(int orderId, int maxRetries = 3, int retryIntervalDays = 1);
    Task<bool> ProcessRetryAsync(int orderId);
    Task<List<RetryAttempt>> GetRetryHistoryAsync(int orderId);
    Task<bool> CancelRetriesAsync(int orderId);
}

public class CallRetryService : ICallRetryService
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ITwilioService _twilioService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<CallRetryService> _logger;

    public CallRetryService(
        BadNewsDbContext dbContext,
        ITwilioService twilioService,
        IEmailService emailService,
        IBackgroundJobClient backgroundJobClient,
        ILogger<CallRetryService> logger)
    {
        _dbContext = dbContext;
        _twilioService = twilioService;
        _emailService = emailService;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task<bool> ScheduleRetryAsync(int orderId, int maxRetries = 3, int retryIntervalDays = 1)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning($"Order not found: {orderId}");
                return false;
            }

            // Schedule retries for the next N days
            for (int i = 1; i <= maxRetries; i++)
            {
                var retryDate = DateTime.UtcNow.AddDays(i * retryIntervalDays);
                var jobId = _backgroundJobClient.Schedule(
                    () => ProcessRetryAsync(orderId),
                    retryDate
                );

                var retry = new CallRetry
                {
                    OrderId = orderId,
                    RetryNumber = i,
                    ScheduledAt = retryDate,
                    Status = "scheduled",
                    HangfireJobId = jobId,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.CallRetries.Add(retry);
                _logger.LogInformation($"Retry {i} scheduled for order {orderId} on {retryDate}");
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error scheduling retries: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessRetryAsync(int orderId)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning($"Order not found for retry: {orderId}");
                return false;
            }

            // Check if order is already completed
            if (order.Status == "completed")
            {
                _logger.LogInformation($"Order already completed, skipping retry: {orderId}");
                return true;
            }

            // Find the next messenger to try
            var nextMessenger = await _dbContext.Users
                .Where(u => u.Role == "messenger" 
                    && u.Id != order.AcceptedMessenger.Id
                    && u.IsAvailable)
                .FirstOrDefaultAsync();

            if (nextMessenger == null)
            {
                _logger.LogWarning($"No available messenger for retry: {orderId}");
                return false;
            }

            // Update order with new messenger
            order.AcceptedMessengerId = nextMessenger.Id;
            order.UpdatedAt = DateTime.UtcNow;

            // Create new call attempt
            var callAttempt = new CallAttempt
            {
                OrderId = orderId,
                MessengerId = nextMessenger.Id,
                Status = "initiated",
                RetryNumber = 1,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CallAttempts.Add(callAttempt);
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            // Send notification to messenger
            await _emailService.SendOrderConfirmationAsync(
                nextMessenger.Email,
                nextMessenger.Name,
                order.Id.ToString(),
                order.TotalPrice
            );

            _logger.LogInformation($"Retry processed for order {orderId}, new messenger: {nextMessenger.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing retry: {ex.Message}");
            return false;
        }
    }

    public async Task<List<RetryAttempt>> GetRetryHistoryAsync(int orderId)
    {
        try
        {
            var retries = await _dbContext.CallRetries
                .Where(r => r.OrderId == orderId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RetryAttempt
                {
                    Id = r.Id,
                    RetryNumber = r.RetryNumber,
                    ScheduledAt = r.ScheduledAt,
                    ExecutedAt = r.ExecutedAt,
                    Status = r.Status,
                    Reason = r.Reason
                })
                .ToListAsync();

            return retries;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting retry history: {ex.Message}");
            return new List<RetryAttempt>();
        }
    }

    public async Task<bool> CancelRetriesAsync(int orderId)
    {
        try
        {
            var retries = await _dbContext.CallRetries
                .Where(r => r.OrderId == orderId && r.Status == "scheduled")
                .ToListAsync();

            foreach (var retry in retries)
            {
                // Remove Hangfire job
                if (!string.IsNullOrEmpty(retry.HangfireJobId))
                {
                    BackgroundJob.Delete(retry.HangfireJobId);
                }

                retry.Status = "cancelled";
                retry.UpdatedAt = DateTime.UtcNow;
                _dbContext.CallRetries.Update(retry);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Retries cancelled for order {orderId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling retries: {ex.Message}");
            return false;
        }
    }
}

public class RetryAttempt
{
    public int Id { get; set; }
    public int RetryNumber { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
}

public class CallRetry
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int RetryNumber { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
    public string HangfireJobId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Order Order { get; set; }
}
