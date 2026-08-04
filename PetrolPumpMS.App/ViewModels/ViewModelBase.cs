using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace PetrolPumpMS.App.ViewModels;

/// <summary>
/// Base for all ViewModels. Holds a busy flag (drives disabled/loading button
/// states in the UI) and a scope factory so every command can resolve services
/// from a fresh DI scope — each scope gets its own AppDbContext instance,
/// which keeps EF Core usage safe even though services are registered Scoped
/// in a long-lived desktop app rather than a per-request web app.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    protected ViewModelBase(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    protected IServiceScope CreateScope() => _scopeFactory.CreateScope();

    protected T Resolve<T>() where T : notnull
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>Runs an async action inside its own DI scope, toggling IsBusy and capturing errors into ErrorMessage.</summary>
    protected async Task RunAsync(Func<IServiceProvider, Task> action)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            using var scope = CreateScope();
            await action(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
