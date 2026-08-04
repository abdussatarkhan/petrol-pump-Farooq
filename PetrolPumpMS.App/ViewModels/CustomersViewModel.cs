using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class CustomersViewModel : ViewModelBase
{
    private readonly IToastService _toast;
    public CustomersViewModel(IServiceScopeFactory scopeFactory, IToastService toast) : base(scopeFactory)
    {
        _toast = toast;
        _ = LoadAsync();
    }

    public ObservableCollection<Customer> Customers { get; } = new();

    [ObservableProperty] private Customer? selected;
    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private bool isPaying;

    [ObservableProperty] private string formName = string.Empty;
    [ObservableProperty] private string formPhone = string.Empty;
    [ObservableProperty] private string formAddress = string.Empty;
    [ObservableProperty] private string formCreditLimitText = string.Empty;

    [ObservableProperty] private string paymentAmountText = string.Empty;
    [ObservableProperty] private string paymentNote = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunAsync(async sp =>
        {
            var list = await sp.GetRequiredService<ICustomerService>().GetAllAsync();
            Customers.Clear();
            foreach (var c in list) Customers.Add(c);
        });
    }

    [RelayCommand]
    private void StartAdd()
    {
        Selected = null;
        IsEditing = true;
        FormName = ""; FormPhone = ""; FormAddress = ""; FormCreditLimitText = "";
    }

    [RelayCommand]
    private void StartEdit(Customer customer)
    {
        Selected = customer;
        IsEditing = true;
        FormName = customer.Name;
        FormPhone = customer.Phone ?? "";
        FormAddress = customer.Address ?? "";
        FormCreditLimitText = customer.CreditLimit.ToString("0.##");
    }

    [RelayCommand]
    private void CancelEdit() => IsEditing = false;

    [RelayCommand]
    private void StartPayment(Customer customer)
    {
        Selected = customer;
        IsPaying = true;
        PaymentAmountText = ""; PaymentNote = "";
    }

    [RelayCommand]
    private void CancelPayment() => IsPaying = false;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!decimal.TryParse(FormCreditLimitText, out var limit))
        {
            ErrorMessage = "Enter a valid credit limit.";
            return;
        }

        await RunAsync(async sp =>
        {
            var svc = sp.GetRequiredService<ICustomerService>();
            var result = Selected is null
                ? await svc.AddAsync(FormName, FormPhone, FormAddress, limit)
                : await svc.UpdateAsync(Selected.Id, FormName, FormPhone, FormAddress, limit);

            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success(Selected is null ? "Customer added." : "Customer updated.");
            IsEditing = false;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task RecordPaymentAsync()
    {
        if (Selected is null) return;
        if (!decimal.TryParse(PaymentAmountText, out var amount))
        {
            ErrorMessage = "Enter a valid amount.";
            return;
        }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<ICustomerService>()
                .RecordPaymentAsync(Selected.Id, DateTime.Today, amount, PaymentNote);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Recorded payment of Rs. {amount:N2} for {Selected.Name}.");
            IsPaying = false;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Customer customer)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<ICustomerService>().DeleteAsync(customer.Id);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success("Customer deleted.");
        });
        if (ErrorMessage is null) await LoadAsync();
    }
}
