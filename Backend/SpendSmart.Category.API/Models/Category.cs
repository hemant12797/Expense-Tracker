namespace SpendSmart.Category.API.Models;

public class Category
{
    public int CategoryId { get; set; }
    public int? UserId { get; set; }          // NULL = system-wide default
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }          // emoji or CSS icon class
    public string? Color { get; set; }         // #RRGGBB
    public string Type { get; set; } = "EXPENSE"; // EXPENSE or INCOME
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
