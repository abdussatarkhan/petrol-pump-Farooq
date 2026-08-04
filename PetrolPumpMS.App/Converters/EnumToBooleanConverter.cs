using System.Globalization;
using System.Windows.Data;

namespace PetrolPumpMS.App.Converters;

/// <summary>Binds a RadioButton's IsChecked to an enum property matching ConverterParameter.</summary>
public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.ToString() == parameter?.ToString();

    public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        (value is bool b && b) ? Enum.Parse(targetType, parameter!.ToString()!) : Binding.DoNothing;
}
