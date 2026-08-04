using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    public EmployeeService(AppDbContext db) => _db = db;

    public Task<List<Employee>> GetAllAsync(bool activeOnly = false)
    {
        var q = _db.Employees.AsQueryable();
        if (activeOnly) q = q.Where(e => e.Status == ActiveStatus.Active);
        return q.OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<ServiceResult> AddAsync(string name, string? phone, string? address, string role, decimal salary, DateTime joinDate)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Name is required.");
        if (salary < 0) return ServiceResult.Fail("Salary cannot be negative.");

        _db.Employees.Add(new Employee
        {
            Name = name.Trim(), Phone = phone, Address = address, Role = role,
            Salary = salary, JoinDate = joinDate.Date, Status = ActiveStatus.Active
        });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(int id, string name, string? phone, string? address, string role, decimal salary, ActiveStatus status)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return ServiceResult.Fail("Employee not found.");
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Name is required.");

        emp.Name = name.Trim();
        emp.Phone = phone;
        emp.Address = address;
        emp.Role = role;
        emp.Salary = salary;
        emp.Status = status;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        if (await _db.Sales.AnyAsync(s => s.EmployeeId == id))
            return ServiceResult.Fail("Cannot delete an employee with sales history. Set them Inactive instead.");

        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return ServiceResult.Fail("Employee not found.");
        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
