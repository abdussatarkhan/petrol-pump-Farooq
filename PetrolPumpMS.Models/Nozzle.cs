namespace PetrolPumpMS.Models;

public class Nozzle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TankId { get; set; }
    public Tank? Tank { get; set; }
    public ActiveStatus Status { get; set; } = ActiveStatus.Active;
    public decimal LastReading { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
