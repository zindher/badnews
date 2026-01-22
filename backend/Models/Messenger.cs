namespace BadNews.Models;

public class Messenger
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal AverageRating { get; set; } = 0;
    public int TotalCompletedOrders { get; set; } = 0;
    public decimal TotalEarnings { get; set; } = 0;
    public decimal PendingBalance { get; set; } = 0;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual User? User { get; set; }
    public ICollection<Withdrawal>? Withdrawals { get; set; }
}
