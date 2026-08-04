namespace PetrolPumpMS.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime JoinDate { get; set; }
    public ActiveStatus Status { get; set; } = ActiveStatus.Active;

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
