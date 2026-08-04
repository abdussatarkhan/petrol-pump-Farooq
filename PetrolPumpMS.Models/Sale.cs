namespace PetrolPumpMS.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public int NozzleId { get; set; }
    public Nozzle? Nozzle { get; set; }
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int FuelTypeId { get; set; }
    public FuelType? FuelType { get; set; }
    public decimal OpeningReading { get; set; }
    public decimal ClosingReading { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
