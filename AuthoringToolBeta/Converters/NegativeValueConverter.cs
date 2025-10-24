// Converters/NegativeValueConverter.cs
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AuthoringToolBeta.Converters
{
    public class NegativeValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return -d;
            }
            return 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}