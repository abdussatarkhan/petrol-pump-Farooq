namespace PetrolPumpMS.Models;

public class FuelType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = "Liter";
    public decimal CurrentPrice { get; set; }

    public ICollection<Tank> Tanks { get; set; } = new List<Tank>();
}
