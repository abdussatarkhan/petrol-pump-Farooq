using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class SalesService : ISalesService
{
    private readonly AppDbContext _db;
    public SalesService(AppDbContext db) => _db = db;

    public Task<List<Sale>> GetSalesAsync(int limit = 200, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var q = _db.Sales
            .Include(s => s.Nozzle)
            .Include(s => s.Employee)
            .Include(s => s.FuelType)
            .Include(s => s.Customer)
            .AsQueryable();

        if (dateFrom.HasValue) q = q.Where(s => s.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) q = q.Where(s => s.Date <= dateTo.Value.Date);

        return q.OrderByDescending(s => s.Date).ThenByDescending(s => s.Id).Take(limit).ToListAsync();
    }

    public async Task<ServiceResult> RecordSaleAsync(
        DateTime date, TimeSpan time, int nozzleId, int? employeeId,
        decimal closingReading, PaymentMode paymentMode, int? customerId)
    {
        if (paymentMode == PaymentMode.Credit && customerId is null)
            return ServiceResult.Fail("Select a customer for credit sales.");

        var nozzle = await _db.Nozzles.Include(n => n.Tank).ThenInclude(t => t!.FuelType)
            .FirstOrDefaultAsync(n => n.Id == nozzleId);
        if (nozzle is null) return ServiceResult.Fail("Select a valid nozzle.");
        if (nozzle.Status != ActiveStatus.Active) return ServiceResult.Fail("This nozzle is inactive.");
        if (nozzle.Tank is null || nozzle.Tank.FuelType is null) return ServiceResult.Fail("Nozzle is not linked to a valid tank/fuel type.");

        var openingReading = nozzle.LastReading;
        if (closingReading <= openingReading)
            return ServiceResult.Fail($"Closing reading must be greater than the opening reading ({openingReading}).");

        var quantity = closingReading - openingReading;
        var tank = nozzle.Tank;
        if (quantity > tank.CurrentStock)
            return ServiceResult.Fail($"Not enough stock in {tank.Name}. Available: {tank.CurrentStock} {tank.FuelType!.Unit}.");

        var rate = tank.FuelType!.CurrentPrice;
        var amount = quantity * rate;

        if (customerId.HasValue && !await _db.Customers.AnyAsync(c => c.Id == customerId.Value))
            return ServiceResult.Fail("Select a valid customer.");

        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Sales.Add(new Sale
        {
            Date = date.Date,
            Time = time,
            NozzleId = nozzleId,
            EmployeeId = employeeId,
            FuelTypeId = tank.FuelTypeId,
            OpeningReading = openingReading,
            ClosingReading = closingReading,
            Quantity = quantity,
            Rate = rate,
            Amount = amount,
            PaymentMode = paymentMode,
            CustomerId = paymentMode == PaymentMode.Credit ? customerId : null
        });

        nozzle.LastReading = closingReading;
        tank.CurrentStock -= quantity;

        if (paymentMode == PaymentMode.Credit && customerId.HasValue)
        {
            var customer = await _db.Customers.FindAsync(customerId.Value);
            if (customer is not null) customer.Balance += amount;
        }

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return ServiceResult.Ok();
    }
}
