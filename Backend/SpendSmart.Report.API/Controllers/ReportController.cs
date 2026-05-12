using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Report.API.Repositories;
using SpendSmart.Report.API.Services;

namespace SpendSmart.Report.API.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _service;
    private readonly IReportRepository _repo;

    public ReportController(IReportService service, IReportRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    public class GenerateReportRequest
    {
        public int UserId { get; set; }
        public string ReportType { get; set; } = "MONTHLY";
        public string Title { get; set; } = string.Empty;
        public string? Parameters { get; set; }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest req)
    {
        var report = await _service.GenerateReportAsync(req.UserId, req.ReportType, req.Title, req.Parameters);
        return Ok(report);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var list = await _repo.GetAllForUserAsync(userId);
        return Ok(list);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> GetDownloadUrl(int id)
    {
        try
        {
            var url = await _service.GetReportSasUrlAsync(id);
            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _service.DeleteReportAsync(id, userId);
            return Ok(new { Message = "Report deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("history")]
    public async Task<IActionResult> ClearDownloadHistory()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _service.ClearDownloadHistoryAsync(userId);
            return Ok(new { Message = "Download history cleared successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
