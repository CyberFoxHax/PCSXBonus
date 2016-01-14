using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCSX2Bonus.Legacy {
	internal sealed class ColorToBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) {
				return new SolidColorBrush(Colors.Transparent);
			}
			try {
				var convertFromString = ColorConverter.ConvertFromString(value.ToString());
				if (convertFromString != null)
					return new SolidColorBrush((Color)convertFromString);
			}
			catch {
				return new SolidColorBrush(Colors.Transparent);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

