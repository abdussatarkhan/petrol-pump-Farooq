using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Models;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISessionService _session;
    private readonly IToastService _toast;

    public SettingsViewModel(IServiceScopeFactory scopeFactory, ISessionService session, IToastService toast) : base(scopeFactory)
    {
        _session = session;
        _toast = toast;
        IsAdmin = _session.CurrentUser?.Role == UserRole.Admin;
        if (IsAdmin) _ = LoadUsersAsync();
    }

    public bool IsAdmin { get; }
    public UserRole[] Roles { get; } = Enum.GetValues<UserRole>();

    // ---- Change own password ----
    [ObservableProperty] private string currentPassword = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string? passwordMessage;

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        PasswordMessage = null;
        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "New password and confirmation do not match.";
            return;
        }

        await RunAsync(async sp =>
        {
            var auth = sp.GetRequiredService<IAuthService>();
            var result = await auth.ChangePasswordAsync(_session.CurrentUser!.Id, CurrentPassword, NewPassword);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            PasswordMessage = "Password updated successfully.";
            CurrentPassword = ""; NewPassword = ""; ConfirmPassword = "";
        });
    }

    // ---- User management (Admin only) ----
    public ObservableCollection<User> Users { get; } = new();

    [ObservableProperty] private string newUsername = string.Empty;
    [ObservableProperty] private string newUserPassword = string.Empty;
    [ObservableProperty] private string newUserFullName = string.Empty;
    [ObservableProperty] private UserRole newUserRole = UserRole.Cashier;

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        await RunAsync(async sp =>
        {
            var list = await sp.GetRequiredService<IUserService>().GetAllAsync();
            Users.Clear();
            foreach (var u in list) Users.Add(u);
        });
    }

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IUserService>()
                .CreateAsync(NewUsername, NewUserPassword, NewUserFullName, NewUserRole);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            _toast.Success($"Created user \"{NewUsername}\".");
            NewUsername = ""; NewUserPassword = ""; NewUserFullName = ""; NewUserRole = UserRole.Cashier;
        });
        if (ErrorMessage is null) await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task ToggleUserActiveAsync(User user)
    {
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IUserService>().SetActiveAsync(user.Id, !user.Active);
            if (!result.Success) { ErrorMessage = result.Error; return; }
        });
        if (ErrorMessage is null) await LoadUsersAsync();
    }

    // ---- Backup ----
    [ObservableProperty] private string? backupMessage;

    [RelayCommand]
    private async Task BackupDatabaseAsync()
    {
        var dialog = new OpenFolderDialog { Title = "Choose a folder to save the backup" };
        if (dialog.ShowDialog() != true) return;

        BackupMessage = null;
        await RunAsync(async sp =>
        {
            var result = await sp.GetRequiredService<IBackupService>().BackupToFileAsync(dialog.FolderName);
            if (!result.Success) { ErrorMessage = result.Error; return; }
            BackupMessage = $"Backup saved to: {result.Data}";
            _toast.Success("Database backup completed.");
        });
    }
}
