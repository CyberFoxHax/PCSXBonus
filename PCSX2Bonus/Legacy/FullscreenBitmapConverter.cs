using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PCSX2Bonus.Legacy {
	internal sealed class FullscreenBitmapConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;
			if (value.ToString().IsEmpty() || (value.ToString() == "none")) {
				var image = new BitmapImage();
				image.BeginInit();
				image.UriSource = new Uri("pack://application:,,,/PCSX2Bonus;component/Images/noart.png", UriKind.Absolute);
				image.DecodePixelWidth = 0x109;
				image.DecodePixelHeight = 350;
				image.EndInit();
				if (image.CanFreeze)
					image.Freeze();
				return image;
			}
			var image2 = new BitmapImage();
			using (var stream = File.OpenRead(value.ToString())) {
				image2.BeginInit();
				image2.StreamSource = stream;
				image2.DecodePixelWidth = 0x109;
				image2.DecodePixelHeight = 350;
				image2.CacheOption = BitmapCacheOption.OnLoad;
				image2.EndInit();
			}
			if (image2.CanFreeze)
				image2.Freeze();
			return image2;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

