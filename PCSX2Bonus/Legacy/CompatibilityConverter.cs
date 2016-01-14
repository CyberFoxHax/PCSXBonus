using System;
using System.Globalization;
using System.Windows.Data;

namespace PCSX2Bonus.Legacy {
	internal sealed class CompatibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture){
			return Tools.GetLocalImage(string.Format("star{0}.png", ((int)value)+1));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

