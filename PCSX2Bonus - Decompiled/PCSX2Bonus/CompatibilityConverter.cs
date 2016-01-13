namespace PCSX2Bonus
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal sealed class CompatibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (((int) value))
            {
                case 0:
                    return Tools.GetLocalImage("star1.png");

                case 1:
                    return Tools.GetLocalImage("star1.png");

                case 2:
                    return Tools.GetLocalImage("star2.png");

                case 3:
                    return Tools.GetLocalImage("star3.png");

                case 4:
                    return Tools.GetLocalImage("star4.png");

                case 5:
                    return Tools.GetLocalImage("star5.png");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

