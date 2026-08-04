using System.Globalization;
using System.Windows.Data;

namespace PetrolPumpMS.App.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !(value is bool v && v);

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        !(value is bool v && v);
}
