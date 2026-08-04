using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly ISessionService _session;

    public LoginViewModel(IServiceScopeFactory scopeFactory, ISessionService session) : base(scopeFactory)
    {
        _session = session;
    }

    [ObservableProperty]
    private string username = string.Empty;

    /// <summary>Set from code-behind on PasswordBox.PasswordChanged (PasswordBox can't be data-bound directly).</summary>
    public string Password { private get; set; } = string.Empty;

    public event Action? LoginSucceeded;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Enter both username and password.";
            return;
        }

        await RunAsync(async sp =>
        {
            var auth = sp.GetRequiredService<IAuthService>();
            var user = await auth.AuthenticateAsync(Username.Trim(), Password);
            if (user is null)
            {
                ErrorMessage = "Invalid username or password, or the account is inactive.";
                return;
            }
            _session.SignIn(user);
            LoginSucceeded?.Invoke();
        });
    }
}
