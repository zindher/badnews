namespace BadNews.Models;

public class Order
{
    public int Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid? AcceptedMessengerId { get; set; }
    public string Message { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string? RecipientPhone { get; set; }
    public string? Category { get; set; }
    public int? WordCount { get; set; }
    public int? EstimatedDuration { get; set; } // in minutes
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    // Retry tracking
    public int CallAttempts { get; set; } = 0;
    public DateTime? LastRetryDate { get; set; }
    public DateTime? NextRetryDate { get; set; }
    
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
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Failed = 5
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}
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
