using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class FuelService : IFuelService
{
    private readonly AppDbContext _db;
    public FuelService(AppDbContext db) => _db = db;

    // ---- Fuel types ----
    public Task<List<FuelType>> GetFuelTypesAsync() =>
        _db.FuelTypes.OrderBy(f => f.Name).ToListAsync();

    public async Task<ServiceResult> AddFuelTypeAsync(string name, string unit, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Fuel name is required.");
        if (price < 0) return ServiceResult.Fail("Price cannot be negative.");
        if (await _db.FuelTypes.AnyAsync(f => f.Name == name)) return ServiceResult.Fail("That fuel type already exists.");

        _db.FuelTypes.Add(new FuelType { Name = name.Trim(), Unit = string.IsNullOrWhiteSpace(unit) ? "Liter" : unit, CurrentPrice = price });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateFuelPriceAsync(int fuelTypeId, decimal newPrice)
    {
        if (newPrice < 0) return ServiceResult.Fail("Price cannot be negative.");
        var fuel = await _db.FuelTypes.FindAsync(fuelTypeId);
        if (fuel is null) return ServiceResult.Fail("Fuel type not found.");
        fuel.CurrentPrice = newPrice;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    // ---- Tanks ----
    public Task<List<Tank>> GetTanksAsync() =>
        _db.Tanks.Include(t => t.FuelType).OrderBy(t => t.Name).ToListAsync();

    public async Task<ServiceResult> AddTankAsync(string name, int fuelTypeId, decimal capacity, decimal currentStock, decimal lowThreshold)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Tank name is required.");
        if (capacity <= 0) return ServiceResult.Fail("Capacity must be greater than zero.");
        if (currentStock > capacity) return ServiceResult.Fail("Current stock cannot exceed capacity.");
        if (!await _db.FuelTypes.AnyAsync(f => f.Id == fuelTypeId)) return ServiceResult.Fail("Select a valid fuel type.");

        _db.Tanks.Add(new Tank { Name = name.Trim(), FuelTypeId = fuelTypeId, Capacity = capacity, CurrentStock = currentStock, LowThreshold = lowThreshold });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateTankAsync(int tankId, string name, decimal capacity, decimal lowThreshold)
    {
        var tank = await _db.Tanks.FindAsync(tankId);
        if (tank is null) return ServiceResult.Fail("Tank not found.");
        if (capacity < tank.CurrentStock) return ServiceResult.Fail("Capacity cannot be less than the current stock.");
        tank.Name = name.Trim();
        tank.Capacity = capacity;
        tank.LowThreshold = lowThreshold;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteTankAsync(int tankId)
    {
        var hasPurchases = await _db.Purchases.AnyAsync(p => p.TankId == tankId);
        var hasNozzles = await _db.Nozzles.AnyAsync(n => n.TankId == tankId);
        if (hasPurchases || hasNozzles)
            return ServiceResult.Fail("Cannot delete a tank that has linked nozzles or purchase history.");

        var tank = await _db.Tanks.FindAsync(tankId);
        if (tank is null) return ServiceResult.Fail("Tank not found.");
        _db.Tanks.Remove(tank);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    // ---- Nozzles ----
    public Task<List<Nozzle>> GetNozzlesAsync() =>
        _db.Nozzles.Include(n => n.Tank).ThenInclude(t => t!.FuelType).OrderBy(n => n.Name).ToListAsync();

    public async Task<ServiceResult> AddNozzleAsync(string name, int tankId, decimal openingReading)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Fail("Nozzle name is required.");
        if (!await _db.Tanks.AnyAsync(t => t.Id == tankId)) return ServiceResult.Fail("Select a valid tank.");

        _db.Nozzles.Add(new Nozzle { Name = name.Trim(), TankId = tankId, LastReading = openingReading, Status = ActiveStatus.Active });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetNozzleStatusAsync(int nozzleId, ActiveStatus status)
    {
        var nozzle = await _db.Nozzles.FindAsync(nozzleId);
        if (nozzle is null) return ServiceResult.Fail("Nozzle not found.");
        nozzle.Status = status;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteNozzleAsync(int nozzleId)
    {
        if (await _db.Sales.AnyAsync(s => s.NozzleId == nozzleId))
            return ServiceResult.Fail("Cannot delete a nozzle that has sales history. Set it Inactive instead.");

        var nozzle = await _db.Nozzles.FindAsync(nozzleId);
        if (nozzle is null) return ServiceResult.Fail("Nozzle not found.");
        _db.Nozzles.Remove(nozzle);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    // ---- Purchases ----
    public Task<List<Purchase>> GetPurchasesAsync(int limit = 200) =>
        _db.Purchases.Include(p => p.FuelType).Include(p => p.Tank)
            .OrderByDescending(p => p.Date).ThenByDescending(p => p.Id)
            .Take(limit).ToListAsync();

    public async Task<ServiceResult> RecordPurchaseAsync(DateTime date, int fuelTypeId, int tankId, string? supplier, decimal quantity, decimal rate)
    {
        if (quantity <= 0) return ServiceResult.Fail("Quantity must be greater than zero.");
        if (rate <= 0) return ServiceResult.Fail("Rate must be greater than zero.");

        var tank = await _db.Tanks.FindAsync(tankId);
        if (tank is null) return ServiceResult.Fail("Select a valid tank.");
        if (tank.FuelTypeId != fuelTypeId) return ServiceResult.Fail("Selected tank does not hold this fuel type.");
        if (tank.CurrentStock + quantity > tank.Capacity)
            return ServiceResult.Fail($"This delivery would exceed the tank's capacity ({tank.Capacity} {tank.FuelType?.Unit ?? "L"}).");

        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Purchases.Add(new Purchase
        {
            Date = date,
            FuelTypeId = fuelTypeId,
            TankId = tankId,
            Supplier = supplier,
            Quantity = quantity,
            Rate = rate,
            Total = quantity * rate
        });
        tank.CurrentStock += quantity;

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return ServiceResult.Ok();
    }
}
