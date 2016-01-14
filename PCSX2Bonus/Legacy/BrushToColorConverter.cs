using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCSX2Bonus.Legacy {
	internal sealed class BrushToColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture){
			return value == null ? Colors.Transparent : ((SolidColorBrush)value).Color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture){
			return value == null ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush((Color)value);
		}
	}
}

