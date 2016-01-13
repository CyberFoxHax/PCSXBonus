namespace ColorPicker
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(double), typeof(string))]
    public sealed class DoubleToIntegerStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num = (double) value;
            int num2 = (int) num;
            return num2.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string) value;
            double result = 0.0;
            if (!double.TryParse(s, out result))
            {
                result = 0.0;
            }
            return result;
        }
    }
}

