namespace BadNews.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuyerId { get; set; }
    public Guid? MessengerId { get; set; }
    public string RecipientPhoneNumber { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public decimal Price { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Payment
    public string? PaymentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // Call Details
    public string? CallRecordingUrl { get; set; }
    public int CallAttempts { get; set; } = 0;
    public DateTime? LastCallAttemptAt { get; set; }
    public bool CallConnected { get; set; } = false;

    // Preferred Call Time & Timezone
    public string? PreferredCallTime { get; set; } // HH:MM format (Aguascalientes time)
    public string? RecipientTimezone { get; set; } // CENTRO, MONTANA, PACIFICO, NOROESTE, QUINTANA_ROO
    public string? RecipientState { get; set; } // Estado/región para referencia

    // Recipient Email (optional, for email fallback)
    public string? RecipientEmail { get; set; }

    // Retry Strategy: 3 calls/day for 3 days = 9 attempts max
    public int RetryDay { get; set; } = 0; // Current day (0-2)
    public int DailyAttempts { get; set; } = 0; // Attempts on current day
    public DateTime? FirstCallAttemptDate { get; set; } // Track when first attempt was made
    public bool FallbackSMSSent { get; set; } = false;
    public bool FallbackEmailSent { get; set; } = false;

    // Rating
    public int? Rating { get; set; } // 1-5 stars
    public DateTime? RatedAt { get; set; }

    // Navigation
    public virtual User? Buyer { get; set; }
    public virtual User? Messenger { get; set; }
    public ICollection<CallAttempt>? CallAttempts_Nav { get; set; }
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
