using BadNews.Data;
using BadNews.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BadNews.Services;

public interface ICallRetryService
{
    Task ScheduleRetryAsync(int orderId);
    Task ProcessRetryAsync(int orderId);
}

public class CallRetryService : ICallRetryService
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<CallRetryService> _logger;

    public CallRetryService(BadNewsDbContext dbContext, ILogger<CallRetryService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ScheduleRetryAsync(int orderId)
    {
        // Stub implementation
        await Task.CompletedTask;
    }

    public async Task ProcessRetryAsync(int orderId)
    {
        // Stub implementation
        await Task.CompletedTask;
    }
}
