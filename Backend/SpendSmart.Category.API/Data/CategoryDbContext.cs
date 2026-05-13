using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Models;

namespace SpendSmart.Category.API.Data;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options) {}
    public DbSet<Models.Category> Categories => Set<Models.Category>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Category>(e => {
            e.HasKey(c => c.CategoryId);
            e.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            e.Property(c => c.UserId).IsRequired(false);
        });
    }
}
