namespace BadNews.Models;

public class CallAttempt
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid MessengerId { get; set; }
    public CallAttemptStatus Status { get; set; } = CallAttemptStatus.Initiated;
    public string? CallSid { get; set; } // Twilio Call SID
    public string? RecordingSid { get; set; } // Twilio Recording SID
    public string? RecordingUrl { get; set; } // Recording URL
    public int? RecordingDuration { get; set; } // in seconds
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; } // Actual call duration in seconds
    public int RetryNumber { get; set; } = 1; // 1-9
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? Messenger { get; set; }
}

public enum CallAttemptStatus
{
    Initiated = 0,
    Ringing = 1,
    Connected = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}
