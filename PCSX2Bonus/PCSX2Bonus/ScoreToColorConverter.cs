namespace PCSX2Bonus
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    internal sealed class ScoreToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            int.TryParse(value.ToString(), out result);
            if (result >= 0x4b)
            {
                return new SolidColorBrush(Color.FromRgb(0x66, 0xcc, 0x33));
            }
            if (result >= 50)
            {
                return new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x33));
            }
            if (result < 50)
            {
                return new SolidColorBrush(Color.FromRgb(0xff, 0, 0));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

