namespace BadNews.Models;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? Sender { get; set; }
}
