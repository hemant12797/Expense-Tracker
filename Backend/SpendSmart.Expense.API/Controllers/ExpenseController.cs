using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Expense.API.Repositories;
using SpendSmart.Expense.API.Services;

namespace SpendSmart.Expense.API.Controllers;

[Route("api/expenses")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _service;
    private readonly IExpenseRepository _repo;

    public ExpenseController(IExpenseService service, IExpenseRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddExpense(Models.Expense expense)
    {
        try
        {
            var created = await _service.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetById), new { id = created.ExpenseId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _repo.GetByIdAsync(id);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var list = await _repo.GetAllForUserAsync(userId);
        return Ok(list);
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchExpenses([FromQuery] int userId, [FromQuery] string q)
    {
        var list = await _repo.SearchExpensesAsync(userId, q);
        return Ok(list);
    }

    [HttpGet("total/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetTotal(int userId)
    {
        var total = await _repo.SumByUserIdAsync(userId);
        return Ok(new { Total = total });
    }

    [HttpPost("{id}/receipt")]
    [Authorize]
    public async Task<IActionResult> UploadReceipt(int id, [FromQuery] int userId, IFormFile file)
    {
        try
        {
            var url = await _service.UploadReceiptAsync(file, userId);
            
            var expense = await _repo.GetByIdAsync(id);
            if (expense != null)
            {
                expense.ReceiptUrl = url;
                await _repo.UpdateAsync(expense);
            }
            
            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
