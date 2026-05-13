using Microsoft.EntityFrameworkCore;
using SpendSmart.Auth.API.Models;

namespace SpendSmart.Auth.API.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {}
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e => {
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasColumnType("NVARCHAR(256)");
            e.Property(u => u.FullName).HasColumnType("NVARCHAR(150)");
        });
    }
}
