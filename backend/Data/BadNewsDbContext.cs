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
    public DbSet<Payment> Payments { get; set; } = null!;

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

        // Messenger Configuration
        modelBuilder.Entity<Messenger>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Messenger>()
            .HasOne(m => m.User)
            .WithMany(u => u.MessengerProfile)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order Configuration
        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.CallAttempts_Nav)
            .WithOne(ca => ca.Order)
            .HasForeignKey(ca => ca.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment Configuration
        modelBuilder.Entity<Payment>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.BuyerId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.MessengerId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ExternalPaymentId)
            .IsUnique();
    }
}
