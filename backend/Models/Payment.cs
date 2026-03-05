namespace BadNews.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MXN";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? ExternalPaymentId { get; set; } // Mercado Pago payment ID
    public string? PaymentMethod { get; set; }
    public string? PaymentMethodId { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? Buyer { get; set; }
}
