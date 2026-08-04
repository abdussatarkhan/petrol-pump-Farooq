namespace PetrolPumpMS.Services.Interfaces;

public record FuelSalesSummary(string FuelName, decimal TotalQuantity, decimal TotalAmount);
public record PaymentModeSummary(string PaymentMode, decimal TotalAmount);
public record DailyTotal(DateTime Date, decimal TotalAmount);
public record EmployeePerformance(string EmployeeName, decimal TotalAmount, decimal TotalQuantity);

public record DashboardStats(
    decimal TodaySalesTotal,
    int TodayTransactionCount,
    decimal TodayExpenseTotal,
    decimal TotalCreditOutstanding);

public interface IReportService
{
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<List<FuelSalesSummary>> SalesByFuelTypeAsync(DateTime from, DateTime to);
    Task<List<PaymentModeSummary>> SalesByPaymentModeAsync(DateTime from, DateTime to);
    Task<List<DailyTotal>> DailySalesTrendAsync(DateTime from, DateTime to);
    Task<List<EmployeePerformance>> EmployeePerformanceAsync(DateTime from, DateTime to);
}
