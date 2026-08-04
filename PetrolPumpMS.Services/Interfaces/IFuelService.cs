using PetrolPumpMS.Models;

namespace PetrolPumpMS.Services.Interfaces;

public interface IFuelService
{
    Task<List<FuelType>> GetFuelTypesAsync();
    Task<ServiceResult> AddFuelTypeAsync(string name, string unit, decimal price);
    Task<ServiceResult> UpdateFuelPriceAsync(int fuelTypeId, decimal newPrice);

    Task<List<Tank>> GetTanksAsync();
    Task<ServiceResult> AddTankAsync(string name, int fuelTypeId, decimal capacity, decimal currentStock, decimal lowThreshold);
    Task<ServiceResult> UpdateTankAsync(int tankId, string name, decimal capacity, decimal lowThreshold);
    Task<ServiceResult> DeleteTankAsync(int tankId);

    Task<List<Nozzle>> GetNozzlesAsync();
    Task<ServiceResult> AddNozzleAsync(string name, int tankId, decimal openingReading);
    Task<ServiceResult> SetNozzleStatusAsync(int nozzleId, ActiveStatus status);
    Task<ServiceResult> DeleteNozzleAsync(int nozzleId);

    Task<List<Purchase>> GetPurchasesAsync(int limit = 200);
    Task<ServiceResult> RecordPurchaseAsync(DateTime date, int fuelTypeId, int tankId, string? supplier, decimal quantity, decimal rate);
}
