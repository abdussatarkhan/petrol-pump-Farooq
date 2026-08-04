namespace PetrolPumpMS.Models;

public class Expense
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}
