using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;

namespace PetrolPumpMS.App.ViewModels;

public enum AppPage { Dashboard, SalesEntry, FuelManagement, Employees, Customers, Expenses, Reports, Settings }

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly ISessionService _session;
    private readonly IToastService _toast;

    public MainViewModel(IServiceScopeFactory scopeFactory, IServiceProvider provider, ISessionService session, IToastService toast)
        : base(scopeFactory)
    {
        _provider = provider;
        _session = session;
        _toast = toast;
        _toast.MessagePosted += OnToastPosted;

        CurrentUserName = _session.CurrentUser?.FullName ?? "";
        CurrentUserRole = _session.CurrentUser?.Role.ToString() ?? "";
        IsAdmin = _session.CurrentUser?.Role == UserRole.Admin;

        NavigateTo(AppPage.Dashboard);
    }

    [ObservableProperty] private string currentUserName = string.Empty;
    [ObservableProperty] private string currentUserRole = string.Empty;
    [ObservableProperty] private bool isAdmin;
    [ObservableProperty] private AppPage activePage = AppPage.Dashboard;
    [ObservableProperty] private object? currentViewModel;

    public ObservableCollection<ToastMessage> Toasts { get; } = new();

    public event Action? LogoutRequested;

    private void OnToastPosted(ToastMessage msg)
    {
        // UI thread marshaling is handled by the view (Dispatcher) subscribing indirectly via this collection change;
        // for simplicity we push directly since ToastService is only ever invoked from UI-bound async continuations.
        Toasts.Add(msg);
        _ = RemoveAfterDelay(msg);
    }

    private async Task RemoveAfterDelay(ToastMessage msg)
    {
        await Task.Delay(3500);
        Toasts.Remove(msg);
    }

    [RelayCommand]
    private void NavigateTo(AppPage page)
    {
        ActivePage = page;
        CurrentViewModel = page switch
        {
            AppPage.Dashboard => _provider.GetRequiredService<DashboardViewModel>(),
            AppPage.SalesEntry => _provider.GetRequiredService<SalesEntryViewModel>(),
            AppPage.FuelManagement => _provider.GetRequiredService<FuelManagementViewModel>(),
            AppPage.Employees => _provider.GetRequiredService<EmployeesViewModel>(),
            AppPage.Customers => _provider.GetRequiredService<CustomersViewModel>(),
            AppPage.Expenses => _provider.GetRequiredService<ExpensesViewModel>(),
            AppPage.Reports => _provider.GetRequiredService<ReportsViewModel>(),
            AppPage.Settings => _provider.GetRequiredService<SettingsViewModel>(),
            _ => CurrentViewModel
        };
    }

    [RelayCommand]
    private void Logout()
    {
        _session.SignOut();
        LogoutRequested?.Invoke();
    }
}
