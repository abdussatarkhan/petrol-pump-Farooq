using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _ = LoadAsync();
    }

    [ObservableProperty] private decimal todaySalesTotal;
    [ObservableProperty] private int todayTransactionCount;
    [ObservableProperty] private decimal todayExpenseTotal;
    [ObservableProperty] private decimal totalCreditOutstanding;

    public ObservableCollection<Tank> Tanks { get; } = new();
    public ObservableCollection<Sale> RecentSales { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunAsync(async sp =>
        {
            var reports = sp.GetRequiredService<IReportService>();
            var fuel = sp.GetRequiredService<IFuelService>();
            var sales = sp.GetRequiredService<ISalesService>();

            var stats = await reports.GetDashboardStatsAsync();
            TodaySalesTotal = stats.TodaySalesTotal;
            TodayTransactionCount = stats.TodayTransactionCount;
            TodayExpenseTotal = stats.TodayExpenseTotal;
            TotalCreditOutstanding = stats.TotalCreditOutstanding;

            var tanks = await fuel.GetTanksAsync();
            Tanks.Clear();
            foreach (var t in tanks) Tanks.Add(t);

            var recent = await sales.GetSalesAsync(limit: 15);
            RecentSales.Clear();
            foreach (var s in recent) RecentSales.Add(s);
        });
    }
}
