using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Budget.API.Data;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) {}
    public DbSet<Models.Budget> Budgets => Set<Models.Budget>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Budget>(e => {
            e.HasKey(x => x.BudgetId);
            e.HasIndex(x => new { x.UserId, x.CategoryId }).IsUnique().HasFilter("[CategoryId] IS NOT NULL");
            e.Property(x => x.LimitAmount).HasColumnType("DECIMAL(18,2)");
            e.Property(x => x.SpentAmount).HasColumnType("DECIMAL(18,2)");
        });
    }
}
