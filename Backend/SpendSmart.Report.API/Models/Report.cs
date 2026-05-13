namespace SpendSmart.Report.API.Models;

public class Report
{
    public int ReportId { get; set; }
    public int UserId { get; set; }
    public string ReportType { get; set; } = "MONTHLY"; // MONTHLY/CATEGORY/TREND/YEARLY/CUSTOM
    public string Title { get; set; } = string.Empty;
    public string? FilePath { get; set; } // Azure Blob URL
    public string? Parameters { get; set; } // JSON string for criteria
    public string Status { get; set; } = "PENDING"; // PENDING/GENERATING/COMPLETED/FAILED
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
