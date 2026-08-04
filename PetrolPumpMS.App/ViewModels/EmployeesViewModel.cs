using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    private readonly IToastService _toast;
    public EmployeesViewModel(IServiceScopeFactory scopeFactory, IToastService toast) : base(scopeFactory)
    {
        _toast = toast;
        _ = LoadAsync();
    }

    public ObservableCollection<Employee> Employees { get; } = new();
    public string[] Roles { get; } = { "Attendant", "Cashier", "Supervisor", "Manager", "Maintenance" };
    public ActiveStatus[] Statuses { get; } = Enum.GetValues<ActiveStatus>();

    [ObservableProperty] private Employee? selected;
    [ObservableProperty] private bool isEditing;

    [ObservableProperty] private string formName = string.Empty;
    [ObservableProperty] private string formPhone = string.Empty;
    [ObservableProperty] private string formAddress = string.Empty;
    [ObservableProperty] private string formRole = "Attendant";
    [ObservableProperty] private string formSalaryText = string.Empty;
    [ObservableProperty] private DateTime formJoinDate = DateTime.Today;
    [ObservableProperty] private ActiveStatus formStatus = ActiveStatus.Active;

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunAsync(async sp =>
        {
            var list = await sp.GetRequiredService<IEmployeeService>().GetAllAsync();
            Employees.Clear();
            foreach (var e in list) Employees.Add(e);
        });
    }

    [RelayCommand]
    private void StartAdd()
    {
        Selected = null;
        IsEditing = true;
        FormName = ""; FormPhone = ""; FormAddress = ""; FormRole = "Attendant";
        FormSalaryText = ""; FormJoinDate = DateTime.Today; FormStatus = ActiveStatus.Active;
    }

    [RelayCommand]
    private void StartEdit(Employee employee)
    {
        Selected = employee;
        IsEditing = true;
        FormName = employee.Name;
        FormPhone = employee.Phone ?? "";
        FormAddress = employee.Address ?? "";
        FormRole = employee.Role;
        FormSalaryText = employee.Salary.ToString("0.##");
        FormJoinDate = employee.JoinDate;
        FormStatus = employee.Status;
    }

    [RelayCommand]
    private void CancelEdit() => IsEditing = false;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!decimal.TryParse(FormSalaryText, out var salary))
        {
            ErrorMessage = "Enter a valid salary.";
            return;
        }

        await RunAsync(async sp =>
        {
            var svc = sp.GetRequiredService<IEmployeeService>();
            var result = Selected is null
                ? await svc.AddAsync(FormName, FormPhone, FormAddress, FormRole, salary, FormJoinDate)
                : await svc.UpdateAsync(Selected.Id, FormName, FormPhone, FormAddress, FormRole, salary, FormStatus);

            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success(Selected is null ? "Employee added." : "Employee updated.");
            IsEditing = false;
        });
        if (ErrorMessage is null) await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Employee employee)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IEmployeeService>().DeleteAsync(employee.Id);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success("Employee deleted.");
        });
        if (ErrorMessage is null) await LoadAsync();
    }
}
