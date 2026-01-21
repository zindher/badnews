namespace BadNews.Models;

public class Messenger
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public double AverageRating { get; set; } = 0;
    public int TotalCompletedOrders { get; set; } = 0;
    public decimal TotalEarnings { get; set; } = 0;
    public decimal PendingBalance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual User? User { get; set; }
}
