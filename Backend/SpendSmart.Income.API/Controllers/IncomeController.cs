using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Income.API.Repositories;
using SpendSmart.Income.API.Services;

namespace SpendSmart.Income.API.Controllers;

[Route("api/incomes")]
[ApiController]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _service;
    private readonly IIncomeRepository _repo;

    public IncomeController(IIncomeService service, IIncomeRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddIncome(Models.Income income)
    {
        var created = await _service.AddIncomeAsync(income);
        return CreatedAtAction(nameof(GetById), new { id = created.IncomeId }, created);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var income = await _repo.GetByIdAsync(id);
        if (income == null) return NotFound();
        return Ok(income);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var list = await _repo.GetAllForUserAsync(userId);
        return Ok(list);
    }

    [HttpGet("total/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetTotal(int userId)
    {
        var total = await _repo.SumByUserIdAsync(userId);
        return Ok(new { Total = total });
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, Models.Income income)
    {
        if (id != income.IncomeId) return BadRequest();
        await _repo.UpdateAsync(income);
        return Ok(income);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
