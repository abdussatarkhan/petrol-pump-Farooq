namespace PetrolPumpMS.Models;

public class Purchase
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int FuelTypeId { get; set; }
    public FuelType? FuelType { get; set; }
    public int TankId { get; set; }
    public Tank? Tank { get; set; }
    public string? Supplier { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Total { get; set; }
}
