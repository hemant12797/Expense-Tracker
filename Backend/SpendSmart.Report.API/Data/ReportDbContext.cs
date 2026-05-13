using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Report.API.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) {}
    public DbSet<Models.Report> Reports => Set<Models.Report>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Models.Report>(e => {
            e.HasKey(x => x.ReportId);
            e.HasIndex(x => x.UserId);
        });
    }
}
