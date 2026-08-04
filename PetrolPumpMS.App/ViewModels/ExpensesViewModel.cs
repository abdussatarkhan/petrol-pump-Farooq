using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class ExpensesViewModel : ViewModelBase
{
    private readonly IToastService _toast;
    public ExpensesViewModel(IServiceScopeFactory scopeFactory, IToastService toast) : base(scopeFactory)
    {
        _toast = toast;
        _ = LoadAsync();
    }

    public ObservableCollection<Expense> Expenses { get; } = new();
    public string[] Categories { get; } = { "Salaries", "Maintenance", "Utilities", "Rent", "Supplies", "Other" };

    [ObservableProperty] private DateTime formDate = DateTime.Today;
    [ObservableProperty] private string formCategory = "Other";
    [ObservableProperty] private string formDescription = string.Empty;
    [ObservableProperty] private string formAmountText = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunAsync(async sp =>
        {
            var list = await sp.GetRequiredService<IExpenseService>().GetAllAsync();
            Expenses.Clear();
            foreach (var e in list) Expenses.Add(e);
        });
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (!decimal.TryParse(FormAmountText, out var amount))
        {
            ErrorMessage = "Enter a valid amount.";
            return;
        }

        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IExpenseService>().AddAsync(FormDate, FormCategory, FormDescription, amount);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success("Expense logged.");
            FormAmountText = ""; FormDescription = ""; FormDate = DateTime.Today;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Expense expense)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IExpenseService>().DeleteAsync(expense.Id);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success("Expense removed.");
        });
        if (ErrorMessage is null) await LoadAsync();
    }
}
