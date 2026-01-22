namespace BadNews.Models;

public class CallRetry
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int RetryNumber { get; set; } // 1-9
    public DateTime ScheduledAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public CallRetryStatus Status { get; set; } = CallRetryStatus.Scheduled;
    public string? Reason { get; set; }
    public string? HangfireJobId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
}

public enum CallRetryStatus
{
    Scheduled = 0,
    Executed = 1,
    Cancelled = 2,
    Failed = 3
}
