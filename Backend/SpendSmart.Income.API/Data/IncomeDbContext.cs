using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Income.API.Data;

public class IncomeDbContext : DbContext
{
    public IncomeDbContext(DbContextOptions<IncomeDbContext> options) : base(options) {}
    public DbSet<Models.Income> Incomes => Set<Models.Income>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Income>(e => {
            e.HasKey(x => x.IncomeId);
            e.HasIndex(x => x.UserId);
            e.Property(x => x.Amount).HasColumnType("DECIMAL(18,2)");
            e.Property(x => x.Source).HasMaxLength(30);
        });
    }
}
