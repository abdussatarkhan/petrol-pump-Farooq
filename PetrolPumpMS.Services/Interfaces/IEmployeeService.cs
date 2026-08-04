using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync(bool activeOnly = false);
    Task<ServiceResult> AddAsync(string name, string? phone, string? address, string role, decimal salary, DateTime joinDate);
    Task<ServiceResult> UpdateAsync(int id, string name, string? phone, string? address, string role, decimal salary, ActiveStatus status);
    Task<ServiceResult> DeleteAsync(int id);
}
