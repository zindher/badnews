namespace BadNews.Models;

public class Dispute
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid ReportedById { get; set; }
    public string Reason { get; set; } = null!;
    public string? Description { get; set; }
    public DisputeStatus Status { get; set; } = DisputeStatus.Open;
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Navigation
    public virtual Order? Order { get; set; }
    public virtual User? ReportedBy { get; set; }
}

public enum DisputeStatus
{
    Open = 0,
    Investigating = 1,
    Resolved = 2,
    Closed = 3
}
