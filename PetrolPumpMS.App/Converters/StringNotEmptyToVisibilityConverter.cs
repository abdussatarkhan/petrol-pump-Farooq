using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PetrolPumpMS.App.Converters;

/// <summary>Used to show/hide inline validation-error TextBlocks bound to a ViewModel error string.</summary>
public class StringNotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
