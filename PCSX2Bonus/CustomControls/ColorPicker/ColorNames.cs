using System.Collections.Generic;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public static class ColorNames {
		private static readonly Dictionary<Color, string> MColorNames = new Dictionary<Color, string>();

		static ColorNames() {
			FillColorNames();
		}

		public static void FillColorNames() {
			var type = typeof(Colors);
			foreach (var info in type.GetProperties()) {
				var name = info.Name;
				var key = (Color)info.GetValue(null, null);
				switch (name) {
					case "Aqua":
						key.R = (byte)(key.R + 1);
						break;

					case "Fuchsia":
						key.G = (byte)(key.G + 1);
						break;
				}
				MColorNames.Add(key, name);
			}
		}

		public static string GetColorName(Color colorToSeek) {
			return MColorNames.ContainsKey(colorToSeek) ? MColorNames[colorToSeek] : colorToSeek.ToString();
		}
	}
}

