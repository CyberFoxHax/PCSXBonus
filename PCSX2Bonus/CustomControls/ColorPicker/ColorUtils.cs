using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public static class ColorUtils {
		private static Color BuildColor(double red, double green, double blue, double m) {
			return Color.FromArgb(0xff, (byte)(((red + m) * 255.0) + 0.5), (byte)(((green + m) * 255.0) + 0.5), (byte)(((blue + m) * 255.0) + 0.5));
		}

		public static Color ConvertHsvToRgb(double hue, double saturation, double value) {
			var red = value * saturation;
			if (Math.Abs(hue - 360.0) < double.Epsilon) {
				hue = 0.0;
			}
			var num2 = hue / 60.0;
			var green = red * (1.0 - Math.Abs((num2 % 2.0) - 1.0));
			var m = value - red;
			switch (((int)num2)) {
				case 0:
					return BuildColor(red, green, 0.0, m);

				case 1:
					return BuildColor(green, red, 0.0, m);

				case 2:
					return BuildColor(0.0, red, green, m);

				case 3:
					return BuildColor(0.0, green, red, m);

				case 4:
					return BuildColor(green, 0.0, red, m);
			}
			return BuildColor(red, 0.0, green, m);
		}

		public static void ConvertRgbToHsv(Color color, out double hue, out double saturation, out double value) {
			var num = color.R / 255.0;
			var num2 = color.G / 255.0;
			var num3 = color.B / 255.0;
			var num4 = Math.Min(num, Math.Min(num2, num3));
			var num5 = Math.Max(num, Math.Max(num2, num3));
			value = num5;
			var num6 = num5 - num4;

			if (Math.Abs(value) < double.Epsilon)
				saturation = 0.0;
			else
				saturation = num6 / num5;

			if (Math.Abs(saturation) < double.Epsilon)
				hue = 0.0;
			else if (Math.Abs(num - num5) < double.Epsilon)
				hue = (num2 - num3) / num6;
			else if (Math.Abs(num2 - num5) < double.Epsilon)
				hue = 2.0 + ((num3 - num) / num6);
			else
				hue = 4.0 + ((num - num2) / num6);

			hue *= 60.0;
			if (hue < 0.0)
				hue += 360.0;
		}

		public static void FireSelectedColorChangedEvent(UIElement issuer, RoutedEvent routedEvent, Color oldColor, Color newColor) {
			var e = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor) {
				RoutedEvent = routedEvent
			};
			issuer.RaiseEvent(e);
		}

		public static string[] GetColorNames() {
			return typeof(Colors).GetProperties().Select(info => info.Name).ToArray();
		}

		public static Color[] GetSpectrumColors(int colorCount) {
			var colorArray = new Color[colorCount];
			for (var i = 0; i < colorCount; i++) {
				var hue = (i * 360.0) / colorCount;
				colorArray[i] = ConvertHsvToRgb(hue, 1.0, 1.0);
			}
			return colorArray;
		}

		public static bool TestColorConversion() {
			for (var i = 0; i <= 0xffffff; i++) {
				double num5;
				double num6;
				double num7;
				var r = (byte)(i & 0xff);
				var g = (byte)((i & 0xff00) >> 8);
				var b = (byte)((i & 0xff0000) >> 0x10);
				var color = Color.FromRgb(r, g, b);
				ConvertRgbToHsv(color, out num5, out num6, out num7);
				var color2 = ConvertHsvToRgb(num5, num6, num7);
				if (color != color2)
					return false;
			}
			return true;
		}
	}
}

