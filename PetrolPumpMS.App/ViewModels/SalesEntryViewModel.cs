using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class SalesEntryViewModel : ViewModelBase
{
    private readonly ISessionService _session;
    private readonly IToastService _toast;

    public SalesEntryViewModel(IServiceScopeFactory scopeFactory, ISessionService session, IToastService toast) : base(scopeFactory)
    {
        _session = session;
        _toast = toast;
        _ = InitializeAsync();
    }

    public ObservableCollection<Nozzle> Nozzles { get; } = new();
    public ObservableCollection<Employee> Employees { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Sale> RecentSales { get; } = new();
    public PaymentMode[] PaymentModes { get; } = Enum.GetValues<PaymentMode>();

    [ObservableProperty] private Nozzle? selectedNozzle;
    [ObservableProperty] private decimal openingReading;
    [ObservableProperty] private string closingReadingText = string.Empty;
    [ObservableProperty] private decimal quantity;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private Employee? selectedEmployee;
    [ObservableProperty] private PaymentMode selectedPaymentMode = PaymentMode.Cash;
    [ObservableProperty] private Customer? selectedCustomer;
    [ObservableProperty] private bool isCreditMode;

    partial void OnSelectedNozzleChanged(Nozzle? value)
    {
        OpeningReading = value?.LastReading ?? 0;
        Recalculate();
    }

    partial void OnClosingReadingTextChanged(string value) => Recalculate();

    partial void OnSelectedPaymentModeChanged(PaymentMode value) => IsCreditMode = value == PaymentMode.Credit;

    private void Recalculate()
    {
        if (SelectedNozzle?.Tank?.FuelType is null) { Quantity = 0; Amount = 0; return; }
        if (!decimal.TryParse(ClosingReadingText, out var closing) || closing <= OpeningReading)
        {
            Quantity = 0; Amount = 0; return;
        }
        Quantity = closing - OpeningReading;
        Amount = Quantity * SelectedNozzle.Tank.FuelType.CurrentPrice;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await RunAsync(async sp =>
        {
            var fuel = sp.GetRequiredService<IFuelService>();
            var emp = sp.GetRequiredService<IEmployeeService>();
            var cust = sp.GetRequiredService<ICustomerService>();
            var sales = sp.GetRequiredService<ISalesService>();

            var nozzles = (await fuel.GetNozzlesAsync()).Where(n => n.Status == ActiveStatus.Active).ToList();
            Nozzles.Clear();
            foreach (var n in nozzles) Nozzles.Add(n);

            var employees = await emp.GetAllAsync(activeOnly: true);
            Employees.Clear();
            foreach (var e in employees) Employees.Add(e);

            var customers = await cust.GetAllAsync();
            Customers.Clear();
            foreach (var c in customers) Customers.Add(c);

            var recent = await sales.GetSalesAsync(limit: 20);
            RecentSales.Clear();
            foreach (var s in recent) RecentSales.Add(s);
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedNozzle is null) { ErrorMessage = "Select a nozzle."; return; }
        if (!decimal.TryParse(ClosingReadingText, out var closing))
        {
            ErrorMessage = "Enter a valid closing reading.";
            return;
        }
        if (SelectedPaymentMode == PaymentMode.Credit && SelectedCustomer is null)
        {
            ErrorMessage = "Select a customer for credit sales.";
            return;
        }

        await RunAsync(async sp =>
        {
            var sales = sp.GetRequiredService<ISalesService>();
            var result = await sales.RecordSaleAsync(
                DateTime.Today, DateTime.Now.TimeOfDay, SelectedNozzle.Id,
                SelectedEmployee?.Id, closing, SelectedPaymentMode, SelectedCustomer?.Id);

            if (!result.Success)
            {
                ErrorMessage = result.Error;
                return;
            }

            _toast.Success($"Sale recorded — {Quantity:N2} {SelectedNozzle.Tank?.FuelType?.Unit} for Rs. {Amount:N2}");
            ClosingReadingText = string.Empty;
            SelectedCustomer = null;
            SelectedPaymentMode = PaymentMode.Cash;
        });

        if (ErrorMessage is null)
            await InitializeAsync();
    }
}
