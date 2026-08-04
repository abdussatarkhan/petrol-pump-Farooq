using Microsoft.EntityFrameworkCore;
using PetrolPumpMS.Data;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var today = DateTime.Today;

        var todaySales = _db.Sales.Where(s => s.Date == today);
        var salesTotal = await todaySales.SumAsync(s => (decimal?)s.Amount) ?? 0m;
        var txnCount = await todaySales.CountAsync();
        var expenseTotal = await _db.Expenses.Where(e => e.Date == today).SumAsync(e => (decimal?)e.Amount) ?? 0m;
        var creditOutstanding = await _db.Customers.SumAsync(c => (decimal?)c.Balance) ?? 0m;

        return new DashboardStats(salesTotal, txnCount, expenseTotal, creditOutstanding);
    }

    public Task<List<FuelSalesSummary>> SalesByFuelTypeAsync(DateTime from, DateTime to) =>
        _db.Sales.Where(s => s.Date >= from.Date && s.Date <= to.Date)
            .Include(s => s.FuelType)
            .GroupBy(s => s.FuelType!.Name)
            .Select(g => new FuelSalesSummary(g.Key, g.Sum(x => x.Quantity), g.Sum(x => x.Amount)))
            .ToListAsync();

    public Task<List<PaymentModeSummary>> SalesByPaymentModeAsync(DateTime from, DateTime to) =>
        _db.Sales.Where(s => s.Date >= from.Date && s.Date <= to.Date)
            .GroupBy(s => s.PaymentMode)
            .Select(g => new PaymentModeSummary(g.Key.ToString(), g.Sum(x => x.Amount)))
            .ToListAsync();

    public async Task<List<DailyTotal>> DailySalesTrendAsync(DateTime from, DateTime to)
    {
        var raw = await _db.Sales.Where(s => s.Date >= from.Date && s.Date <= to.Date)
            .GroupBy(s => s.Date)
            .Select(g => new { g.Key, Total = g.Sum(x => x.Amount) })
            .ToListAsync();

        return raw.OrderBy(x => x.Key).Select(x => new DailyTotal(x.Key, x.Total)).ToList();
    }

    public Task<List<EmployeePerformance>> EmployeePerformanceAsync(DateTime from, DateTime to) =>
        _db.Sales.Where(s => s.Date >= from.Date && s.Date <= to.Date)
            .Include(s => s.Employee)
            .GroupBy(s => s.Employee != null ? s.Employee.Name : "Unassigned")
            .Select(g => new EmployeePerformance(g.Key, g.Sum(x => x.Amount), g.Sum(x => x.Quantity)))
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();
}
