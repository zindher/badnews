using Microsoft.EntityFrameworkCore;
using BadNews.Models;

namespace BadNews.Data;

public class BadNewsDbContext : DbContext
{
    public BadNewsDbContext(DbContextOptions<BadNewsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Messenger> Messengers { get; set; } = null!;
    public DbSet<CallAttempt> CallAttempts { get; set; } = null!;
    public DbSet<CallRetry> CallRetries { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Withdrawal> Withdrawals { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Dispute> Disputes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.BuyerOrders)
            .WithOne(o => o.Buyer)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.MessengerOrders)
            .WithOne(o => o.Messenger)
            .HasForeignKey(o => o.MessengerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CallAttempts)
            .WithOne(ca => ca.Messenger)
            .HasForeignKey(ca => ca.MessengerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Payments)
            .WithOne(p => p.Buyer)
            .HasForeignKey(p => p.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Withdrawals)
            .WithOne(w => w.Messenger)
            .HasForeignKey(w => w.MessengerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Disputes)
            .WithOne(d => d.ReportedBy)
            .HasForeignKey(d => d.ReportedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Messenger Configuration
        modelBuilder.Entity<Messenger>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Messenger>()
            .HasOne(m => m.User)
            .WithOne(u => u.MessengerProfile)
            .HasForeignKey<Messenger>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Messenger>()
            .HasMany(m => m.Withdrawals)
            .WithOne(w => w.Messenger)
            .HasForeignKey(w => w.MessengerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order Configuration
        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany(u => u.BuyerOrders)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Messenger)
            .WithMany(u => u.MessengerOrders)
            .HasForeignKey(o => o.AcceptedMessengerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.CallAttempts_Nav)
            .WithOne(ca => ca.Order)
            .HasForeignKey(ca => ca.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.CallRetries)
            .WithOne(cr => cr.Order)
            .HasForeignKey(cr => cr.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Messages)
            .WithOne(m => m.Order)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Disputes)
            .WithOne(d => d.Order)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // CallAttempt Configuration
        modelBuilder.Entity<CallAttempt>()
            .HasKey(ca => ca.Id);

        modelBuilder.Entity<CallAttempt>()
            .HasOne(ca => ca.Order)
            .WithMany(o => o.CallAttempts_Nav)
            .HasForeignKey(ca => ca.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CallAttempt>()
            .HasOne(ca => ca.Messenger)
            .WithMany(u => u.CallAttempts)
            .HasForeignKey(ca => ca.MessengerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CallAttempt>()
            .HasIndex(ca => ca.CallSid)
            .IsUnique();

        modelBuilder.Entity<CallAttempt>()
            .HasIndex(ca => ca.OrderId);

        // CallRetry Configuration
        modelBuilder.Entity<CallRetry>()
            .HasKey(cr => cr.Id);

        modelBuilder.Entity<CallRetry>()
            .HasOne(cr => cr.Order)
            .WithMany(o => o.CallRetries)
            .HasForeignKey(cr => cr.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CallRetry>()
            .HasIndex(cr => cr.OrderId);

        // Payment Configuration
        modelBuilder.Entity<Payment>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Buyer)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ExternalPaymentId)
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.OrderId);

        // Withdrawal Configuration
        modelBuilder.Entity<Withdrawal>()
            .HasKey(w => w.Id);

        modelBuilder.Entity<Withdrawal>()
            .HasOne(w => w.Messenger)
            .WithMany(m => m.Withdrawals)
            .HasForeignKey(w => w.MessengerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Withdrawal>()
            .HasIndex(w => w.MessengerId);

        // Message Configuration
        modelBuilder.Entity<Message>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Order)
            .WithMany(o => o.Messages)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasIndex(m => m.OrderId);

        // Dispute Configuration
        modelBuilder.Entity<Dispute>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Dispute>()
            .HasOne(d => d.Order)
            .WithMany(o => o.Disputes)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Dispute>()
            .HasOne(d => d.ReportedBy)
            .WithMany(u => u.Disputes)
            .HasForeignKey(d => d.ReportedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dispute>()
            .HasIndex(d => d.OrderId);

        // General Indexes
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.BuyerId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.AcceptedMessengerId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CreatedAt);

        modelBuilder.Entity<CallRetry>()
            .HasIndex(cr => new { cr.OrderId, cr.RetryNumber })
            .IsUnique();

        modelBuilder.Entity<Withdrawal>()
            .HasIndex(w => w.CreatedAt);

        modelBuilder.Entity<Message>()
            .HasIndex(m => m.CreatedAt);
    }
}
