using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class ReportsViewModel : ViewModelBase
{
    public ReportsViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _ = LoadAsync();
    }

    [ObservableProperty] private DateTime dateFrom = DateTime.Today.AddDays(-6);
    [ObservableProperty] private DateTime dateTo = DateTime.Today;

    public ObservableCollection<FuelSalesSummary> FuelSummary { get; } = new();
    public ObservableCollection<PaymentModeSummary> PaymentSummary { get; } = new();
    public ObservableCollection<EmployeePerformance> EmployeeSummary { get; } = new();

    public ISeries[] TrendSeries { get; private set; } = Array.Empty<ISeries>();
    public Axis[] TrendXAxes { get; private set; } = Array.Empty<Axis>();

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (DateFrom > DateTo)
        {
            ErrorMessage = "'From' date must be before 'To' date.";
            return;
        }

        await RunAsync(async sp =>
        {
            var reports = sp.GetRequiredService<IReportService>();

            var fuelSummary = await reports.SalesByFuelTypeAsync(DateFrom, DateTo);
            FuelSummary.Clear();
            foreach (var f in fuelSummary) FuelSummary.Add(f);

            var paymentSummary = await reports.SalesByPaymentModeAsync(DateFrom, DateTo);
            PaymentSummary.Clear();
            foreach (var p in paymentSummary) PaymentSummary.Add(p);

            var empSummary = await reports.EmployeePerformanceAsync(DateFrom, DateTo);
            EmployeeSummary.Clear();
            foreach (var e in empSummary) EmployeeSummary.Add(e);

            var trend = await reports.DailySalesTrendAsync(DateFrom, DateTo);
            TrendSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = trend.Select(t => t.TotalAmount).ToArray(),
                    Name = "Sales (Rs.)",
                    Fill = null,
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(new SKColor(0x25, 0x63, 0xEB)) { StrokeThickness = 3 },
                    GeometryStroke = new SolidColorPaint(new SKColor(0x25, 0x63, 0xEB)) { StrokeThickness = 3 }
                }
            };
            TrendXAxes = new[]
            {
                new Axis { Labels = trend.Select(t => t.Date.ToString("MMM d")).ToArray() }
            };
            OnPropertyChanged(nameof(TrendSeries));
            OnPropertyChanged(nameof(TrendXAxes));
        });
    }
}
