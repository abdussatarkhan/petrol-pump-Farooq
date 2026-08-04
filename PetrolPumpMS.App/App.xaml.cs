using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetrolPumpMS.App.Services;
using PetrolPumpMS.App.ViewModels;
using PetrolPumpMS.App.Views;
using PetrolPumpMS.Data;
using PetrolPumpMS.Services;
using PetrolPumpMS.Services.Interfaces;

namespace PetrolPumpMS.App;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("Default")
                    ?? throw new InvalidOperationException("Missing ConnectionStrings:Default in appsettings.json");

                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

                // Services (business logic layer)
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IFuelService, FuelService>();
                services.AddScoped<ISalesService, SalesService>();
                services.AddScoped<IEmployeeService, EmployeeService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IExpenseService, ExpenseService>();
                services.AddScoped<IReportService, ReportService>();
                services.AddScoped<IBackupService, BackupService>();

                // App-local UI services
                services.AddSingleton<IToastService, ToastService>();
                services.AddSingleton<ISessionService, SessionService>();

                // ViewModels
                services.AddTransient<LoginViewModel>();
                services.AddSingleton<MainViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<SalesEntryViewModel>();
                services.AddTransient<FuelManagementViewModel>();
                services.AddTransient<EmployeesViewModel>();
                services.AddTransient<CustomersViewModel>();
                services.AddTransient<ExpensesViewModel>();
                services.AddTransient<ReportsViewModel>();
                services.AddTransient<SettingsViewModel>();

                // Windows
                services.AddTransient<LoginWindow>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        try
        {
            using var scope = _host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
            await DbSeeder.SeedAsync(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not connect to the database or apply migrations.\n\n{ex.Message}\n\n" +
                "Check the connection string in appsettings.json and make sure PostgreSQL is running and " +
                "the InitialCreate migration has been generated (see PetrolPumpMS.Data/Migrations/README.md).",
                "Startup error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    public static T GetService<T>() where T : notnull =>
        ((App)Current)._host!.Services.GetRequiredService<T>();
}
