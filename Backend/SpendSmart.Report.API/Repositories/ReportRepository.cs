using Microsoft.EntityFrameworkCore;
using SpendSmart.Report.API.Data;

namespace SpendSmart.Report.API.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Models.Report>> GetAllForUserAsync(int userId)
    {
        return await _context.Reports
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<Models.Report?> GetByIdAsync(int reportId)
    {
        return await _context.Reports.FindAsync(reportId);
    }

    public async Task AddAsync(Models.Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Report report)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Models.Report report)
    {
        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllForUserAsync(int userId)
    {
        var reports = await _context.Reports.Where(r => r.UserId == userId).ToListAsync();
        _context.Reports.RemoveRange(reports);
        await _context.SaveChangesAsync();
    }
}
