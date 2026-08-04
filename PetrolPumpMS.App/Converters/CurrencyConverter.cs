using System.Globalization;
using System.Windows.Data;

namespace PetrolPumpMS.App.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        decimal d => $"Rs. {d:N2}",
        double dd => $"Rs. {dd:N2}",
        int i => $"Rs. {i:N2}",
        _ => "Rs. 0.00"
    };

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
