namespace SpendSmart.Report.API.Repositories;

public interface IReportRepository
{
    Task<IList<Models.Report>> GetAllForUserAsync(int userId);
    Task<Models.Report?> GetByIdAsync(int reportId);
    Task AddAsync(Models.Report report);
    Task UpdateAsync(Models.Report report);
    Task DeleteAsync(Models.Report report);
    Task DeleteAllForUserAsync(int userId);
}
