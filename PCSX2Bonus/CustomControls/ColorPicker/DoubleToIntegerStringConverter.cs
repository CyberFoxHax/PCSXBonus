using System;
using System.Globalization;
using System.Windows.Data;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	[ValueConversion(typeof(double), typeof(string))]
	public sealed class DoubleToIntegerStringConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var num = (double)value;
			var num2 = (int)num;
			return num2 + "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			var s = (string)value;
			double result;
			if (double.TryParse(s, out result) == false)
				result = 0.0;
			return result;
		}
	}
}

