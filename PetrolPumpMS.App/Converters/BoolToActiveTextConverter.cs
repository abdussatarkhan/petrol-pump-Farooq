using System.Globalization;
using System.Windows.Data;

namespace PetrolPumpMS.App.Converters;

public class BoolToActiveTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value is bool b && b) ? "Active" : "Inactive";

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
