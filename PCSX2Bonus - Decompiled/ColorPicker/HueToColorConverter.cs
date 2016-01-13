namespace ColorPicker
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class HueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hue = (double) value;
            return ColorUtils.ConvertHsvToRgb(hue, 1.0, 1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

