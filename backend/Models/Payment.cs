namespace BadNews.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MXN";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? ExternalPaymentId { get; set; } // Mercado Pago payment ID
    public string? PaymentMethod { get; set; } // credit_card, debit_card, etc
    public string? PaymentMethodId { get; set; } // Last 4 digits or account
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? Buyer { get; set; }
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}
