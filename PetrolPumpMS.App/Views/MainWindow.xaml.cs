using System.Windows;
using PetrolPumpMS.App.ViewModels;

namespace PetrolPumpMS.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        vm.LogoutRequested += OnLogoutRequested;
    }

    private void OnLogoutRequested()
    {
        var loginWindow = App.GetService<LoginWindow>();
        loginWindow.Show();
        Close();
    }
}
