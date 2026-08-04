using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class FuelManagementViewModel : ViewModelBase
{
    private readonly IToastService _toast;

    public FuelManagementViewModel(IServiceScopeFactory scopeFactory, IToastService toast) : base(scopeFactory)
    {
        _toast = toast;
        _ = LoadAsync();
    }

    public ObservableCollection<FuelType> FuelTypes { get; } = new();
    public ObservableCollection<Tank> Tanks { get; } = new();
    public ObservableCollection<Nozzle> Nozzles { get; } = new();
    public ObservableCollection<Purchase> Purchases { get; } = new();

    // ---- New fuel type form ----
    [ObservableProperty] private string newFuelName = string.Empty;
    [ObservableProperty] private string newFuelUnit = "Liter";
    [ObservableProperty] private string newFuelPriceText = string.Empty;

    // ---- New tank form ----
    [ObservableProperty] private string newTankName = string.Empty;
    [ObservableProperty] private FuelType? newTankFuelType;
    [ObservableProperty] private string newTankCapacityText = string.Empty;
    [ObservableProperty] private string newTankStockText = "0";
    [ObservableProperty] private string newTankLowThresholdText = "500";

    // ---- New nozzle form ----
    [ObservableProperty] private string newNozzleName = string.Empty;
    [ObservableProperty] private Tank? newNozzleTank;

    // ---- Purchase form ----
    [ObservableProperty] private DateTime purchaseDate = DateTime.Today;
    [ObservableProperty] private FuelType? purchaseFuelType;
    [ObservableProperty] private Tank? purchaseTank;
    [ObservableProperty] private string purchaseSupplier = string.Empty;
    [ObservableProperty] private string purchaseQuantityText = string.Empty;
    [ObservableProperty] private string purchaseRateText = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunAsync(async sp =>
        {
            var fuel = sp.GetRequiredService<IFuelService>();

            var fuelTypes = await fuel.GetFuelTypesAsync();
            FuelTypes.Clear();
            foreach (var f in fuelTypes) FuelTypes.Add(f);

            var tanks = await fuel.GetTanksAsync();
            Tanks.Clear();
            foreach (var t in tanks) Tanks.Add(t);

            var nozzles = await fuel.GetNozzlesAsync();
            Nozzles.Clear();
            foreach (var n in nozzles) Nozzles.Add(n);

            var purchases = await fuel.GetPurchasesAsync();
            Purchases.Clear();
            foreach (var p in purchases) Purchases.Add(p);
        });
    }

    [RelayCommand]
    private async Task AddFuelTypeAsync()
    {
        if (!decimal.TryParse(NewFuelPriceText, out var price))
        {
            ErrorMessage = "Enter a valid price.";
            return;
        }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>().AddFuelTypeAsync(NewFuelName, NewFuelUnit, price);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Added fuel type \"{NewFuelName}\".");
            NewFuelName = string.Empty; NewFuelPriceText = string.Empty; NewFuelUnit = "Liter";
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task UpdateFuelPriceAsync(FuelType fuel)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>().UpdateFuelPriceAsync(fuel.Id, fuel.CurrentPrice);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Updated {fuel.Name} price to Rs. {fuel.CurrentPrice:N2}.");
        });
    }

    [RelayCommand]
    private async Task AddTankAsync()
    {
        if (NewTankFuelType is null) { ErrorMessage = "Select a fuel type."; return; }
        if (!decimal.TryParse(NewTankCapacityText, out var capacity) ||
            !decimal.TryParse(NewTankStockText, out var stock) ||
            !decimal.TryParse(NewTankLowThresholdText, out var lowThreshold))
        {
            ErrorMessage = "Enter valid numbers for capacity, stock, and low-stock threshold.";
            return;
        }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>()
                .AddTankAsync(NewTankName, NewTankFuelType.Id, capacity, stock, lowThreshold);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Added tank \"{NewTankName}\".");
            NewTankName = string.Empty; NewTankCapacityText = string.Empty;
            NewTankStockText = "0"; NewTankLowThresholdText = "500"; NewTankFuelType = null;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteTankAsync(Tank tank)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>().DeleteTankAsync(tank.Id);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Deleted tank \"{tank.Name}\".");
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task AddNozzleAsync()
    {
        if (NewNozzleTank is null) { ErrorMessage = "Select a tank."; return; }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>().AddNozzleAsync(NewNozzleName, NewNozzleTank.Id, 0);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Added nozzle \"{NewNozzleName}\".");
            NewNozzleName = string.Empty; NewNozzleTank = null;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task ToggleNozzleStatusAsync(Nozzle nozzle)
    {
        var newStatus = nozzle.Status == ActiveStatus.Active ? ActiveStatus.Inactive : ActiveStatus.Active;
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>().SetNozzleStatusAsync(nozzle.Id, newStatus);
            if (!result.Success) { ErrorMessage = result.Error; return; }
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task RecordPurchaseAsync()
    {
        if (PurchaseFuelType is null || PurchaseTank is null) { ErrorMessage = "Select fuel type and tank."; return; }
        if (!decimal.TryParse(PurchaseQuantityText, out var qty) || !decimal.TryParse(PurchaseRateText, out var rate))
        {
            ErrorMessage = "Enter valid numbers for quantity and rate.";
            return;
        }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IFuelService>()
                .RecordPurchaseAsync(PurchaseDate, PurchaseFuelType.Id, PurchaseTank.Id, PurchaseSupplier, qty, rate);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Recorded purchase of {qty:N2} {PurchaseFuelType.Unit}.");
            PurchaseQuantityText = string.Empty; PurchaseRateText = string.Empty; PurchaseSupplier = string.Empty;
        });
        if (ErrorMessage is null) await LoadAsync();
    }
}
