using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PetrolPumpMS.App.ViewModels;

namespace PetrolPumpMS.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _vm;

    public LoginWindow(LoginViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = _vm;
        _vm.LoginSucceeded += OnLoginSucceeded;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) =>
        _vm.Password = PasswordBox.Password;

    private void OnLoginSucceeded()
    {
        var mainWindow = App.GetService<MainWindow>();
        mainWindow.Show();
        Close();
    }
}
