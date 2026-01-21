namespace BadNews.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "MercadoPago"; // MercadoPago, etc
    public string ExternalPaymentId { get; set; } = null!; // Mercado Pago transaction ID
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? FailureReason { get; set; }

    // Navigation
    public virtual Order? Order { get; set; }
}
