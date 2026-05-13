using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Category.API.Models;
using SpendSmart.Category.API.Repositories;
using SpendSmart.Category.API.Services;

namespace SpendSmart.Category.API.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    private readonly ICategoryRepository _repo;

    public CategoryController(ICategoryService service, ICategoryRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpGet("defaults")]
    public async Task<IActionResult> GetDefaults()
    {
        // For simplicity, passing a dummy user ID that doesn't matter for defaults
        // Or better yet, we can filter only defaults
        var all = await _service.GetAllForUserAsync(0);
        return Ok(all.Where(c => c.IsDefault));
    }

    [HttpGet("allForUser/{userId}")]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var all = await _service.GetAllForUserAsync(userId);
        return Ok(all);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Models.Category category)
    {
        await _repo.AddAsync(category);
        return Ok(category);
    }

    [HttpPut("{id}/deactivate")]
    [Authorize]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _repo.DeactivateCategoryAsync(id);
        return Ok();
    }
}
