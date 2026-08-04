namespace PetrolPumpMS.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal Balance { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<CreditPayment> CreditPayments { get; set; } = new List<CreditPayment>();

    public bool IsOverLimit => CreditLimit > 0 && Balance > CreditLimit;
}
