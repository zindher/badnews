namespace BadNews.Models;

public class Withdrawal
{
    public int Id { get; set; }
    public Guid MessengerId { get; set; }
    public decimal Amount { get; set; }
    public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? BankAccount { get; set; } // Encrypted in production
    public string? BankName { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    // Navigation
    public virtual User? Messenger { get; set; }
}

public enum WithdrawalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Processed = 3
}
