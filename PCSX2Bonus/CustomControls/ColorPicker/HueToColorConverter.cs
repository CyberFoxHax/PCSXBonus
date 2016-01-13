using System;
using System.Globalization;
using System.Windows.Data;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class HueToColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var hue = (double)value;
			return ColorUtils.ConvertHsvToRgb(hue, 1.0, 1.0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

