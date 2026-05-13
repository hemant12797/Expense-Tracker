using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Expense.API.Data;

public class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) {}
    public DbSet<Models.Expense> Expenses => Set<Models.Expense>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Expense>(e => {
            e.HasKey(x => x.ExpenseId);
            e.HasIndex(x => new { x.UserId, x.Date });
            e.HasIndex(x => new { x.UserId, x.CategoryId });
            e.Property(x => x.Amount).HasColumnType("DECIMAL(18,2)");
        });
    }
}
