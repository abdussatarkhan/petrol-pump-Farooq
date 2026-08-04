using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface IExpenseService
{
    Task<List<Expense>> GetAllAsync(int limit = 200);
    Task<ServiceResult> AddAsync(DateTime date, string category, string? description, decimal amount);
    Task<ServiceResult> DeleteAsync(int id);
}
