namespace BadNews.Models;

public class CallAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid? MessengerId { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public CallStatus Status { get; set; } = CallStatus.Queued;
    public string? TwilioCallSid { get; set; }
    public string? RecordingUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public string? FailureReason { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? Messenger { get; set; }
}

public enum CallStatus
{
    Queued = 0,
    Ringing = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    NoAnswer = 5,
    Busy = 6,
    Cancelled = 7
}
