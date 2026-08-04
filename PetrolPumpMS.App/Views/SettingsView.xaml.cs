using System.Windows;
using System.Windows.Controls;
using PetrolPumpMS.App.ViewModels;

namespace PetrolPumpMS.App.Views;

public partial class SettingsView : UserControl
{
    public SettingsView() => InitializeComponent();

    private SettingsViewModel? Vm => DataContext as SettingsViewModel;

    private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (Vm is not null) Vm.CurrentPassword = CurrentPasswordBox.Password;
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (Vm is not null) Vm.NewPassword = NewPasswordBox.Password;
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (Vm is not null) Vm.ConfirmPassword = ConfirmPasswordBox.Password;
    }
}
