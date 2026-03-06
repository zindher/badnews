namespace BadNews.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuyerId { get; set; }
    public Guid? MessengerId { get; set; }
    public string RecipientPhoneNumber { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string? RecipientEmail { get; set; }
    public string? RecipientState { get; set; }
    public string? RecipientTimezone { get; set; }
    public string Message { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public decimal Price { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // Call tracking
    public int CallAttempts { get; set; } = 0;
    public int RetryDay { get; set; } = 0;
    public int DailyAttempts { get; set; } = 0;
    public DateTime? LastCallAttemptAt { get; set; }
    public bool CallConnected { get; set; } = false;
    public string? CallRecordingUrl { get; set; }
    public bool FallbackSMSSent { get; set; } = false;
    public bool FallbackEmailSent { get; set; } = false;

    // Scheduling
    public string? PreferredCallTime { get; set; }

    // Rating
    public int? Rating { get; set; }
    public DateTime? RatedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public virtual User? Buyer { get; set; }
    public virtual User? Messenger { get; set; }
    public ICollection<CallAttempt>? CallAttempts_Nav { get; set; }
    public ICollection<CallRetry>? CallRetries { get; set; }
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<Message>? Messages { get; set; }
    public ICollection<Dispute>? Disputes { get; set; }
}

public enum OrderStatus
{
    Pending,
    Assigned,
    InProgress,
    Completed,
    Failed,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
