using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCSX2Bonus.PCSX2Bonus
{
	internal sealed class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            try
            {
                return new SolidColorBrush((Color) ColorConverter.ConvertFromString(value.ToString()));
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

