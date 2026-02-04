namespace BadNews.Models;

public class Order
{
    public int Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid? AcceptedMessengerId { get; set; }
    public Guid? MessengerId { get; set; } // Deprecated, use AcceptedMessengerId
    public string Message { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string? RecipientPhone { get; set; }
    public string? RecipientPhoneNumber { get; set; } // Alias for RecipientPhone
    public string? RecipientEmail { get; set; }
    public string? RecipientState { get; set; }
    public string? Category { get; set; }
    public int? WordCount { get; set; }
    public int? EstimatedDuration { get; set; } // in minutes
    public decimal TotalPrice { get; set; }
    public decimal Price { get; set; } // Alias for TotalPrice
    public bool IsAnonymous { get; set; } = false;
    public string? PreferredCallTime { get; set; } // HH:MM format
    public string? RecipientTimezone { get; set; }
    public int? Rating { get; set; }
    public DateTime? RatedAt { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    // Retry tracking
    public int CallAttempts { get; set; } = 0;
    public int? RetryDay { get; set; } = 0;
    public int? DailyAttempts { get; set; } = 0;
    public DateTime? LastRetryDate { get; set; }
    public DateTime? LastCallAttemptAt { get; set; }
    public DateTime? NextRetryDate { get; set; }
    public bool CallConnected { get; set; } = false;
    public string? CallRecordingUrl { get; set; }
    
    public string? Notes { get; set; }
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
    Pending = 0,
    Accepted = 1,
    Assigned = 1, // Alias for Accepted
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Failed = 5
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}
