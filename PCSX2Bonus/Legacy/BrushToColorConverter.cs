using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCSX2Bonus.Legacy
{
	internal sealed class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Colors.Transparent;
            }
            return ((SolidColorBrush) value).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            return new SolidColorBrush((Color) value);
        }
    }
}

