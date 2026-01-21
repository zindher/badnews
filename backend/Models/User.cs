namespace BadNews.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Order>? BuyerOrders { get; set; }
    public ICollection<Order>? MessengerOrders { get; set; }
    public ICollection<Messenger>? MessengerProfile { get; set; }
}

public enum UserRole
{
    Buyer,
    Messenger,
    Admin
}
