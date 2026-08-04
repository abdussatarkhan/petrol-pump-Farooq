namespace PetrolPumpMS.Models;

public class Tank
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FuelTypeId { get; set; }
    public FuelType? FuelType { get; set; }
    public decimal Capacity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal LowThreshold { get; set; } = 500;

    public ICollection<Nozzle> Nozzles { get; set; } = new List<Nozzle>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public bool IsLowStock => CurrentStock <= LowThreshold;
}
