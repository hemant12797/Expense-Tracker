namespace SpendSmart.Report.API.Services;

public interface IReportService
{
    Task<Models.Report> GenerateReportAsync(int userId, string type, string title, string? parameters);
    Task<string> GetReportSasUrlAsync(int reportId);
    Task DeleteReportAsync(int reportId, int userId);
    Task ClearDownloadHistoryAsync(int userId);
}
