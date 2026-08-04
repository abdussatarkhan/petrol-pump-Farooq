using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();
    Task<ServiceResult> AddAsync(string name, string? phone, string? address, decimal creditLimit);
    Task<ServiceResult> UpdateAsync(int id, string name, string? phone, string? address, decimal creditLimit);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult> RecordPaymentAsync(int customerId, DateTime date, decimal amount, string? note);
}
