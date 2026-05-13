using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Budget.API.Repositories;

namespace SpendSmart.Budget.API.Controllers;

[Route("api/budgets")]
[ApiController]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetRepository _repo;

    public BudgetController(IBudgetRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var budgets = await _repo.GetAllForUserAsync(userId);
        return Ok(budgets);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Models.Budget budget)
    {
        try 
        {
            await _repo.AddAsync(budget);
            return Ok(budget);
        }
        catch (Exception ex)
        {
            // Log for debugging
            Console.WriteLine($"[BudgetController] Error creating budget: {ex.Message}");
            Console.WriteLine($"[BudgetController] Inner: {ex.InnerException?.Message}");
            
            var msg = ex.InnerException?.Message ?? ex.Message;
            if (msg.Contains("UNIQUE") || msg.Contains("duplicate") || msg.Contains("IX_"))
                return BadRequest("A budget for this category already exists.");
            
            return BadRequest($"Failed to create budget: {msg}");
        }
    }

    [HttpPost("check")]
    [AllowAnonymous] // Internal service-to-service call
    public async Task<IActionResult> CheckBudget([FromBody] BudgetCheckRequest request)
    {
        var service = HttpContext.RequestServices.GetRequiredService<SpendSmart.Budget.API.Services.IBudgetService>();
        await service.CheckBudgetOnExpenseAsync(request.UserId, request.CategoryId ?? 0, request.Amount);
        return Ok();
    }

    [HttpPost("validate")]
    [AllowAnonymous] // Called by Expense API before saving
    public async Task<IActionResult> ValidateBudget([FromBody] BudgetCheckRequest request)
    {
        var service = HttpContext.RequestServices.GetRequiredService<SpendSmart.Budget.API.Services.IBudgetService>();
        var error = await service.ValidateBudgetAsync(request.UserId, request.CategoryId, request.Amount);
        if (error != null)
            return BadRequest(new { message = error });
        return Ok(new { message = "Budget is within limits." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok();
    }
}

public class BudgetCheckRequest
{
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
}
