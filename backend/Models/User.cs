namespace BadNews.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Timezone support
    public string PreferredTimezone { get; set; } = "America/Mexico_City";
    public string? PreferredCallTime { get; set; }
    
    // Email verification
    public bool EmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiry { get; set; }
    
    // Google OAuth
    public string? GoogleId { get; set; }
    public string? GoogleEmail { get; set; }
    public string? GoogleProfilePictureUrl { get; set; }
    public bool IsGoogleLinked { get; set; } = false;

    // Terms and Conditions
    public DateTime? TermsAcceptedAt { get; set; }
    public string? TermsAcceptedVersion { get; set; } = "1.0"; // For versioning T&C

    // Navigation
    public ICollection<Order>? BuyerOrders { get; set; }
    public ICollection<Order>? MessengerOrders { get; set; }
    public virtual Messenger? MessengerProfile { get; set; }
    public ICollection<CallAttempt>? CallAttempts { get; set; }
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<Message>? Messages { get; set; }
    public ICollection<Withdrawal>? Withdrawals { get; set; }
    public ICollection<Dispute>? Disputes { get; set; }
}

public enum UserRole
{
    Buyer = 0,
    Messenger = 1,
    Admin = 2
}
