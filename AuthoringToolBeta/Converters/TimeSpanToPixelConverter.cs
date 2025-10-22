using System;
using System.Globalization;
using System.Collections.Generic;
using Avalonia.Data.Converters;

namespace AuthoringToolBeta.Converters
{
    public class TimeSpanToPixelConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // values[0] に 時間 (StartTime or Duration) が、
            // values[1] に スケール (Scale) が渡されてくることを期待する
            if (values.Count == 2 && 
                values[0] is double timeSpan && 
                values[1] is double scale)
            {
                return timeSpan * scale; // 時間 × スケール = ピクセル を計算して返す
            }

            return 0.0; // 変換に失敗した場合は 0 を返す
        }
    }
}