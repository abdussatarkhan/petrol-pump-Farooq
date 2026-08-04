using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;
    public ExpenseService(AppDbContext db) => _db = db;

    public Task<List<Models.Expense>> GetAllAsync(int limit = 200) =>
        _db.Expenses.OrderByDescending(e => e.Date).ThenByDescending(e => e.Id).Take(limit).ToListAsync();

    public async Task<ServiceResult> AddAsync(DateTime date, string category, string? description, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(category)) return ServiceResult.Fail("Category is required.");
        if (amount <= 0) return ServiceResult.Fail("Amount must be greater than zero.");

        _db.Expenses.Add(new Models.Expense { Date = date.Date, Category = category.Trim(), Description = description, Amount = amount });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var e = await _db.Expenses.FindAsync(id);
        if (e is null) return ServiceResult.Fail("Expense not found.");
        _db.Expenses.Remove(e);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
