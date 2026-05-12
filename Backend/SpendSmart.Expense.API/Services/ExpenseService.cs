using System.Net.Http;
using System.Text.Json;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using SpendSmart.Expense.API.Repositories;
using SpendSmart.Shared.Messages;

namespace SpendSmart.Expense.API.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repo;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(IExpenseRepository repo, HttpClient httpClient, IConfiguration config, ILogger<ExpenseService> logger)
    {
        _repo = repo;
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<Models.Expense> AddExpenseAsync(Models.Expense expense)
    {
        expense.CreatedAt = DateTime.UtcNow;

        // ── Budget validation BEFORE saving ──────────────────────────────────
        try
        {
            var payload = new
            {
                UserId = expense.UserId,
                CategoryId = expense.CategoryId,
                Amount = expense.Amount
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var validateResponse = await _httpClient.PostAsync("http://localhost:5149/api/budgets/validate", content);

            if (!validateResponse.IsSuccessStatusCode)
            {
                var body = await validateResponse.Content.ReadAsStringAsync();
                string errorMsg = "Budget limit exceeded.";
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                        errorMsg = msgProp.GetString() ?? errorMsg;
                }
                catch { }

                throw new InvalidOperationException(errorMsg);
            }
        }
        catch (InvalidOperationException)
        {
            throw; // re-throw validation errors
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not reach Budget API for validation – proceeding without check.");
        }
        // ─────────────────────────────────────────────────────────────────────

        await _repo.AddAsync(expense);

        // Notify Budget API to update spent amount
        try
        {
            var payload = new {
                UserId = expense.UserId,
                CategoryId = expense.CategoryId,
                Amount = expense.Amount
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("http://localhost:5149/api/budgets/check", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify Budget API via HTTP");
        }
            
        return expense;
    }

    public async Task<string> UploadReceiptAsync(IFormFile file, int userId)
    {
        var connStr = _config["AzureBlob:ConnectionString"];
        if (string.IsNullOrEmpty(connStr))
            throw new Exception("Azure Blob connection string is not configured.");

        var containerClient = new BlobContainerClient(connStr, "receipts");
        await containerClient.CreateIfNotExistsAsync();
        
        var blobName = $"{userId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);
        
        // Return SAS URL (time-limited, not public)
        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read,
            DateTimeOffset.UtcNow.AddHours(1));
            
        return sasUri.ToString();
    }
}
