namespace PCSX2Bonus
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    internal sealed class SafeBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            Size size = new Size(55.0, 75.0);
            if (parameter != null)
            {
                size = (Size) parameter;
            }
            if (value.ToString().IsEmpty() || (value.ToString() == "none"))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("pack://application:,,,/PCSX2Bonus;component/Images/noart.png", UriKind.Absolute);
                image.DecodePixelWidth = (int) size.Width;
                image.DecodePixelHeight = (int) size.Height;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                if (image.CanFreeze)
                {
                    image.Freeze();
                }
                return image;
            }
            BitmapImage image2 = new BitmapImage();
            using (FileStream stream = File.OpenRead(value.ToString()))
            {
                image2.BeginInit();
                image2.StreamSource = stream;
                image2.DecodePixelWidth = (int) size.Width;
                image2.DecodePixelHeight = (int) size.Height;
                image2.CacheOption = BitmapCacheOption.OnLoad;
                image2.EndInit();
            }
            if (image2.CanFreeze)
            {
                image2.Freeze();
            }
            return image2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

