using System;
using System.Globalization;
using System.Collections.Generic;
using Avalonia.Data.Converters;

namespace AuthoringToolBeta.Converters;

public class CalculateDurationConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // values[0]にStartTimeが
        // values[1]にEntTimeが
        // values[2]にScaleが渡されて来ることを期待する
        if (values.Count == 3 &&
            values[0] is double startTime &&
            values[1] is double endTime &&
            values[2] is double scale)
        {
            return (endTime - startTime) * scale;
        }

        return 0.0;
    }
}