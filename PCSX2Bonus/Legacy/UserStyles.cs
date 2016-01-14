using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace PCSX2Bonus.Legacy {
	public sealed class UserStyles : INotifyPropertyChanged {
		private static readonly FontFamilyConverter _fontConverter = new FontFamilyConverter();
		private static readonly FontWeightConverter _fontWeightConverter = new FontWeightConverter();
		private static SizeConverter _sizeConverter = new SizeConverter();
		private static XElement _themeXel = Tools.TryLoad(Path.Combine(UserSettings.ThemesDir, Properties.Settings.Default.defaultTheme));
		private static readonly ThicknessConverter _thicknessConverter = new ThicknessConverter();
		public static SolidColorBrush ButtonBackgroundBrush = GetThemeBrush("Button", "backColor", DefaultStyles.ButtonBackgroundBrush);
		public static SolidColorBrush ButtonDisabledBrush = GetThemeBrush("Button", "disabledBackColor", DefaultStyles.ButtonDisabledBrush);
		public static SolidColorBrush ButtonFontBrush = GetThemeBrush("Button_Font", "foreColor", DefaultStyles.ButtonFontBrush);
		public static SolidColorBrush ButtonFontDisabledBrush = GetThemeBrush("Button_Font", "disabledForeColor", DefaultStyles.ButtonFontDisabledBrush);
		public static FontFamily ButtonFontFamily = GetThemeFont("Button_Font", "font", DefaultStyles.ButtonFontFamily);
		public static SolidColorBrush ButtonFontMouseOverBrush = GetThemeBrush("Button_Font", "mouseOverColor", DefaultStyles.ButtonFontMouseOverBrush);
		public static double ButtonFontSize = GetThemeDouble("Button_Font", "fontSize", DefaultStyles.ButtonFontSize);
		public static FontWeight ButtonFontWeight = GetThemeFontWeight("Button_Font", "fontWeight", DefaultStyles.ButtonFontWeight);
		public static SolidColorBrush ButtonMouseOverBrush = GetThemeBrush("Button", "mouseOverColor", DefaultStyles.ButtonMouseOverBrush);
		public static SolidColorBrush ButtonPressedBrush = GetThemeBrush("Button", "pressedColor", DefaultStyles.ButtonPressedBrush);
		public static SolidColorBrush CheckBoxBackgroundBrush = GetThemeBrush("Checkbox", "backColor", DefaultStyles.CheckBoxBackgroundBrush);
		public static SolidColorBrush CheckBoxBorderBrush = GetThemeBrush("Checkbox", "borderColor", DefaultStyles.CheckBoxBorderBrush);
		public static Thickness CheckBoxBorderThickness = GetThemeThickness("Checkbox", "borderThickness", DefaultStyles.CheckBoxBorderThickness);
		public static SolidColorBrush CheckBoxFontBrush = GetThemeBrush("Checkbox_Font", "foreColor", DefaultStyles.CheckBoxFontBrush);
		public static FontFamily CheckBoxFontFamily = GetThemeFont("Checkbox_Font", "font", DefaultStyles.CheckBoxFontFamily);
		public static SolidColorBrush CheckBoxFontMouseOverBrush = GetThemeBrush("Checkbox_Font", "mouseOverColor", DefaultStyles.CheckBoxFontMouseOverBrush);
		public static double CheckBoxFontSize = GetThemeDouble("Checkbox_Font", "fontSize", DefaultStyles.CheckBoxFontSize);
		public static FontWeight CheckBoxFontWeight = GetThemeFontWeight("Checkbox_Font", "fontWeight", DefaultStyles.CheckBoxFontWeight);
		public static SolidColorBrush CheckBoxMouseOverBrush = GetThemeBrush("Checkbox", "mouseOverColor", DefaultStyles.CheckBoxMouseOverBrush);
		public static SolidColorBrush ComboBoxBackgroundBrush = GetThemeBrush("DropdownBox", "backColor", DefaultStyles.ComboBoxBackgroundBrush);
		public static SolidColorBrush ComboBoxBorderBrush = GetThemeBrush("DropdownBox", "borderColor", DefaultStyles.ComboBoxBorderBrush);
		public static Thickness ComboBoxBorderThickness = GetThemeThickness("DropdownBox", "borderThickness", DefaultStyles.ComboBoxBorderThickness);
		public static SolidColorBrush ComboBoxFontBrush = GetThemeBrush("DropdownBox_Font", "foreColor", DefaultStyles.ComboBoxFontBrush);
		public static FontFamily ComboBoxFontFamily = GetThemeFont("DropdownBox_Font", "font", DefaultStyles.ComboBoxFontFamily);
		public static SolidColorBrush ComboBoxFontMouseOverBrush = GetThemeBrush("DropdownBox_Font", "mouseOverColor", DefaultStyles.ComboBoxFontMouseOverBrush);
		public static double ComboBoxFontSize = GetThemeDouble("DropdownBox_Font", "fontSize", DefaultStyles.ComboBoxFontSize);
		public static FontWeight ComboBoxFontWeight = GetThemeFontWeight("DropdownBox_Font", "fontWeight", DefaultStyles.ComboBoxFontWeight);
		public static SolidColorBrush ComboBoxMouseOverBrush = GetThemeBrush("DropdownBox", "mouseOverColor", DefaultStyles.ComboBoxMouseOverBrush);
		public static SolidColorBrush ComboBoxSubBackgroundBrush = GetThemeBrush("DropdownBox_Subitems", "backColor", DefaultStyles.ComboBoxSubBackgroundBrush);
		public static SolidColorBrush ComboBoxSubFontBrush = GetThemeBrush("DropdownBox_Subitems_Font", "foreColor", DefaultStyles.ComboBoxSubFontBrush);
		public static FontFamily ComboBoxSubFontFamily = GetThemeFont("DropdownBox_Subitems_Font", "font", DefaultStyles.ComboBoxSubFontFamily);
		public static SolidColorBrush ComboBoxSubFontMouseOverBrush = GetThemeBrush("DropdownBox_Subitems_Font", "mouseOverColor", DefaultStyles.ComboBoxSubFontMouseOverBrush);
		public static double ComboBoxSubFontSize = GetThemeDouble("DropdownBox_Subitems_Font", "fontSize", DefaultStyles.ComboBoxSubFontSize);
		public static FontWeight ComboBoxSubFontWeight = GetThemeFontWeight("DropdownBox_Subitems_Font", "fontWeight", DefaultStyles.ComboBoxSubFontWeight);
		public static SolidColorBrush ComboBoxSubMouseOverBrush = GetThemeBrush("DropdownBox_Subitems", "mouseOverColor", DefaultStyles.ComboBoxSubMouseOverBrush);
		public static SolidColorBrush GridViewHeaderBackgroundBrush = GetThemeBrush("ListView_Columns", "backColor", DefaultStyles.GridViewHeaderBackgroundBrush);
		public static SolidColorBrush GridViewHeaderBorderBrush = GetThemeBrush("ListView_Columns", "borderColor", DefaultStyles.GridViewHeaderBorderBrush);
		public static Thickness GridViewHeaderBorderThickness = GetThemeThickness("ListView_Columns", "borderThickness", DefaultStyles.GridViewHeaderBorderThickness);
		public static SolidColorBrush GridViewHeaderFontBrush = GetThemeBrush("ListView_Columns_Font", "foreColor", DefaultStyles.GridViewHeaderFontBrush);
		public static FontFamily GridViewHeaderFontFamily = GetThemeFont("ListView_Columns_Font", "font", DefaultStyles.GridViewHeaderFontFamily);
		public static double GridViewHeaderFontSize = GetThemeDouble("ListView_Columns_Font", "fontSize", DefaultStyles.GridViewHeaderFontSize);
		public static FontWeight GridViewHeaderFontWeight = GetThemeFontWeight("ListView_Columns_Font", "fontWeight", DefaultStyles.GridViewHeaderFontWeight);
		public static SolidColorBrush GridViewMouseOverBrush = GetThemeBrush("ListView_Columns_Font", "mouseOverColor", DefaultStyles.GridViewHeaderFontMouseOverBrush);
		public static SolidColorBrush ListViewAlternateRowBrush = GetThemeBrush("ListView", "alternateRowColor", DefaultStyles.ListViewAlternateRowBrush);
		public static SolidColorBrush ListViewBackgroundBrush = GetThemeBrush("ListView", "backColor", DefaultStyles.ListViewBackgroundBrush);
		public static SolidColorBrush ListViewFontBrush = GetThemeBrush("ListView_Font", "foreColor", DefaultStyles.ListViewFontBrush);
		public static FontFamily ListViewFontFamily = GetThemeFont("ListView_Font", "font", DefaultStyles.ListViewFontFamily);
		public static SolidColorBrush ListViewFontMouseOverBrush = GetThemeBrush("ListView_Font", "mouseOverColor", DefaultStyles.ListViewFontMouseOverBrush);
		public static SolidColorBrush ListViewFontSelectionBrush = GetThemeBrush("ListView_Font", "selectionColor", DefaultStyles.ListViewFontSelectionBrush);
		public static double ListViewFontSize = GetThemeDouble("ListView_Font", "fontSize", DefaultStyles.ListViewFontSize);
		public static FontWeight ListViewFontWeight = GetThemeFontWeight("ListView_Font", "fontWeight", DefaultStyles.ListViewFontWeight);
		public static double ListViewImageHeight = GetThemeDouble("ListView", "imageHeight", DefaultStyles.ListViewImageHeight);
		public static Size ListViewImageSize = new Size(ListViewImageWidth, ListViewImageHeight);
		public static double ListViewImageWidth = GetThemeDouble("ListView", "imageWidth", DefaultStyles.ListViewImageWidth);
		public static SolidColorBrush ListViewSelectionBrush = GetThemeBrush("ListView", "selectionColor", DefaultStyles.ListViewSelectionBrush);
		public static SolidColorBrush MenuFontBrush = GetThemeBrush("Menu_Font", "foreColor", DefaultStyles.MenuFontBrush);
		public static FontFamily MenuFontFamily = GetThemeFont("Menu_Font", "font", DefaultStyles.MenuFontFamily);
		public static SolidColorBrush MenuFontMouseOverBrush = GetThemeBrush("Menu_Font", "mouseOverColor", DefaultStyles.MenuFontMouseOverBrush);
		public static double MenuFontSize = GetThemeDouble("Menu_Font", "fontSize", DefaultStyles.MenuFontSize);
		public static FontWeight MenuFontWeight = GetThemeFontWeight("Menu_Font", "fontWeight", DefaultStyles.MenuFontWeight);
		public static SolidColorBrush MenuSubBackgroundBrush = GetThemeBrush("Menu_Subitems", "backColor", DefaultStyles.MenuSubBackgroundBrush);
		public static SolidColorBrush MenuSubFontBrush = GetThemeBrush("Menu_Subitems_Font", "foreColor", DefaultStyles.MenuSubFontBrush);
		public static FontFamily MenuSubFontFamily = GetThemeFont("Menu_Subitems_Font", "font", DefaultStyles.MenuSubFontFamily);
		public static SolidColorBrush MenuSubFontMouseOverBrush = GetThemeBrush("Menu_Subitems_Font", "mouseOverColor", DefaultStyles.MenuSubFontMouseOverBrush);
		public static double MenuSubFontSize = GetThemeDouble("Menu_Subitems_Font", "fontSize", DefaultStyles.MenuSubFontSize);
		public static FontWeight MenuSubFontWeight = GetThemeFontWeight("Menu_Subitems_Font", "fontWeight", DefaultStyles.MenuSubFontWeight);
		public static SolidColorBrush MenuSubMouseOverBrush = GetThemeBrush("Menu_Subitems", "mouseOverColor", DefaultStyles.MenuSubMouseOverBrush);
		public static SolidColorBrush TabBackgroundBrush = GetThemeBrush("Tab", "backColor", DefaultStyles.TabBackgroundBrush);
		public static SolidColorBrush TabFontBrush = GetThemeBrush("Tab_Font", "foreColor", DefaultStyles.TabFontBrush);
		public static FontFamily TabFontFamily = GetThemeFont("Tab_Font", "font", DefaultStyles.TabFontFamily);
		public static SolidColorBrush TabFontMouseOverBrush = GetThemeBrush("Tab_Font", "mouseOverColor", DefaultStyles.TabFontMouseOverBrush);
		public static double TabFontSize = GetThemeDouble("Tab_Font", "fontSize", DefaultStyles.TabFontSize);
		public static FontWeight TabFontWeight = GetThemeFontWeight("Tab_Font", "fontWeight", DefaultStyles.TabFontWeight);
		public static SolidColorBrush TabSubBackgroundBrush = GetThemeBrush("Tab_Subitems", "backColor", DefaultStyles.TabSubBackgroundBrush);
		public static SolidColorBrush TabSubFontBrush = GetThemeBrush("Tab_Subitems_Font", "foreColor", DefaultStyles.TabSubFontBrush);
		public static FontFamily TabSubFontFamily = GetThemeFont("Tab_Subitems_Font", "font", DefaultStyles.TabSubFontFamily);
		public static SolidColorBrush TabSubFontMouseOverBrush = GetThemeBrush("Tab_Subitems_Font", "mouseOverColor", DefaultStyles.TabSubFontMouseOverBrush);
		public static double TabSubFontSize = GetThemeDouble("Tab_Subitems_Font", "fontSize", DefaultStyles.TabSubFontSize);
		public static FontWeight TabSubFontWeight = GetThemeFontWeight("Tab_Subitems_Font", "fontWeight", DefaultStyles.TabSubFontWeight);
		public static SolidColorBrush TabSubSelectionBrush = GetThemeBrush("Tab_Subitems", "selectionColor", DefaultStyles.TabSubSelectionBrush);
		public static SolidColorBrush TextBoxBackgroundBrush = GetThemeBrush("Textbox", "backColor", DefaultStyles.TextBoxBackgroundBrush);
		public static SolidColorBrush TextBoxBorderBrush = GetThemeBrush("Textbox", "borderColor", DefaultStyles.TextBoxBorderBrush);
		public static Thickness TextBoxBorderThickness = GetThemeThickness("Textbox", "borderThickness", DefaultStyles.TextBoxBorderThickness);
		public static SolidColorBrush TextBoxFontBrush = GetThemeBrush("Textbox_Font", "foreColor", DefaultStyles.TextBoxFontBrush);
		public static FontFamily TextBoxFontFamily = GetThemeFont("Textbox_Font", "font", DefaultStyles.TextBoxFontFamily);
		public static double TextBoxFontSize = GetThemeDouble("Textbox_Font", "fontSize", DefaultStyles.TextBoxFontSize);
		public static FontWeight TextBoxFontWeight = GetThemeFontWeight("Textbox_Font", "fontWeight", DefaultStyles.TextBoxFontWeight);
		public static double TileViewImageHeight = GetThemeDouble("TileView", "imageHeight", DefaultStyles.ListViewImageHeight);
		public static Size TileViewImageSize = new Size(TileViewImageWidth, TileViewImageHeight);
		public static double TileViewImageWidth = GetThemeDouble("TileView", "imageWidth", DefaultStyles.ListViewImageWidth);
		public static SolidColorBrush WindowBackgroundBrush = GetThemeBrush("Window", "backColor", DefaultStyles.WindowBackgroundBrush);

		public event PropertyChangedEventHandler PropertyChanged;

		private static SolidColorBrush GetThemeBrush(string element, string attribute, SolidColorBrush fallback) {
			try {
				return new SolidColorBrush((Color)ColorConverter.ConvertFromString(GetThemeValue(element, attribute)));
			}
			catch {
				return fallback;
			}
		}

		private static double GetThemeDouble(string element, string attribute, double fallback) {
			double result;
			return (double.TryParse(GetThemeValue(element, attribute), out result) ? result : fallback);
		}

		private static FontFamily GetThemeFont(string element, string attribute, FontFamily fallback) {
			try {
				return (FontFamily)_fontConverter.ConvertFromString(GetThemeValue(element, attribute));
			}
			catch {
				return fallback;
			}
		}

		private static FontWeight GetThemeFontWeight(string element, string attribute, FontWeight fallback) {
			try {
				return (FontWeight)_fontWeightConverter.ConvertFromString(GetThemeValue(element, attribute));
			}
			catch {
				return fallback;
			}
		}

		private static Thickness GetThemeThickness(string element, string attribute, Thickness fallback) {
			try {
				return (Thickness)_thicknessConverter.ConvertFromString(GetThemeValue(element, attribute));
			}
			catch {
				return fallback;
			}
		}

		private static string GetThemeValue(string element, string attribute) {
			var element2 = _themeXel.Element(element);
			if (element2 == null) {
				return "";
			}
			return element2.Attribute(attribute) == null ? "" : _themeXel.Element(element).Attribute(attribute).Value;
		}

		public static void LoadTheme(string xmlFile) {
			_themeXel = Tools.TryLoad(Path.Combine(UserSettings.ThemesDir, xmlFile));
			FilePath = Path.Combine(UserSettings.ThemesDir, xmlFile);
		}

		private void OnPropertyChanged(string property) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public static string FilePath { get; set; }
	}
}

