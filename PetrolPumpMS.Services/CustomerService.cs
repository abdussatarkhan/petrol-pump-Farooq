using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    public CustomerService(AppDbContext db) => _db = db;

    public Task<List<Customer>> GetAllAsync() =>
        _db.Customers.OrderBy(c => c.Name).ToListAsync();

    public async Task<ServiceResult> AddAsync(string name, string? phone, string? address, decimal creditLimit)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Name is required.");
        if (creditLimit < 0) return ServiceResult.Fail("Credit limit cannot be negative.");

        _db.Customers.Add(new Customer { Name = name.Trim(), Phone = phone, Address = address, CreditLimit = creditLimit, Balance = 0 });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(int id, string name, string? phone, string? address, decimal creditLimit)
    {
        var c = await _db.Customers.FindAsync(id);
        if (c is null) return ServiceResult.Fail("Customer not found.");
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Name is required.");

        c.Name = name.Trim();
        c.Phone = phone;
        c.Address = address;
        c.CreditLimit = creditLimit;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var hasSales = await _db.Sales.AnyAsync(s => s.CustomerId == id);
        var hasPayments = await _db.CreditPayments.AnyAsync(p => p.CustomerId == id);
        if (hasSales || hasPayments) return ServiceResult.Fail("Cannot delete a customer with sales or payment history.");

        var c = await _db.Customers.FindAsync(id);
        if (c is null) return ServiceResult.Fail("Customer not found.");
        if (c.Balance != 0) return ServiceResult.Fail("Cannot delete a customer with an outstanding balance.");

        _db.Customers.Remove(c);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RecordPaymentAsync(int customerId, DateTime date, decimal amount, string? note)
    {
        if (amount <= 0) return ServiceResult.Fail("Payment amount must be greater than zero.");

        var customer = await _db.Customers.FindAsync(customerId);
        if (customer is null) return ServiceResult.Fail("Customer not found.");

        using var tx = await _db.Database.BeginTransactionAsync();
        _db.CreditPayments.Add(new CreditPayment { CustomerId = customerId, Date = date.Date, Amount = amount, Note = note });
        customer.Balance -= amount;
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return ServiceResult.Ok();
    }
}
