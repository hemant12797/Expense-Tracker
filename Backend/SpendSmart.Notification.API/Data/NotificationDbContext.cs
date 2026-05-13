using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Notification.API.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) {}
    public DbSet<Models.Notification> Notifications => Set<Models.Notification>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Notification>(e => {
            e.HasKey(x => x.NotificationId);
            e.HasIndex(x => x.UserId);
            e.Property(x => x.Title).HasMaxLength(100);
            e.Property(x => x.Message).HasMaxLength(500);
        });
    }
}
