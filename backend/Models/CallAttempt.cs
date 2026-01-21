namespace BadNews.Models;

public class CallAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public CallStatus Status { get; set; }
    public string? TwilioCallSid { get; set; }
    public int? DurationSeconds { get; set; }
    public string? RecordingUrl { get; set; }

    // Navigation
    public virtual Order? Order { get; set; }
}

public enum CallStatus
{
    Queued,
    Ringing,
    InProgress,
    Completed,
    Failed,
    NoAnswer,
    Busy,
    Cancelled
}
