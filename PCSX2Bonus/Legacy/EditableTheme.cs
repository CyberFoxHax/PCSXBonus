using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace PCSX2Bonus.Legacy {
	public sealed class EditableTheme {
		private SolidColorBrush _ButtonBackgroundBrush = UserStyles.ButtonBackgroundBrush;
		private SolidColorBrush _ButtonDisabledBrush = UserStyles.ButtonDisabledBrush;
		private SolidColorBrush _ButtonFontBrush = UserStyles.ButtonFontBrush;
		private SolidColorBrush _ButtonFontDisabledBrush = UserStyles.ButtonFontDisabledBrush;
		private FontFamily _ButtonFontFamily = UserStyles.ButtonFontFamily;
		private SolidColorBrush _ButtonFontMouseOverBrush = UserStyles.ButtonFontMouseOverBrush;
		private double _ButtonFontSize = UserStyles.ButtonFontSize;
		private FontWeight _ButtonFontWeight = UserStyles.ButtonFontWeight;
		private SolidColorBrush _ButtonMouseOverBrush = UserStyles.ButtonMouseOverBrush;
		private SolidColorBrush _ButtonPressedBrush = UserStyles.ButtonPressedBrush;
		private SolidColorBrush _CheckBoxBackgroundBrush = UserStyles.CheckBoxBackgroundBrush;
		private SolidColorBrush _CheckBoxBorderBrush = UserStyles.CheckBoxBorderBrush;
		private Thickness _CheckBoxBorderThickness = UserStyles.CheckBoxBorderThickness;
		private SolidColorBrush _CheckBoxFontBrush = UserStyles.CheckBoxFontBrush;
		private FontFamily _CheckBoxFontFamily = UserStyles.CheckBoxFontFamily;
		private SolidColorBrush _CheckBoxFontMouseOverBrush = UserStyles.CheckBoxFontMouseOverBrush;
		private double _CheckBoxFontSize = UserStyles.CheckBoxFontSize;
		private FontWeight _CheckBoxFontWeight = UserStyles.CheckBoxFontWeight;
		private SolidColorBrush _CheckBoxMouseOverBrush = UserStyles.CheckBoxMouseOverBrush;
		private SolidColorBrush _GridViewHeaderBackgroundBrush = UserStyles.GridViewHeaderBackgroundBrush;
		private SolidColorBrush _GridViewHeaderBorderBrush = UserStyles.GridViewHeaderBorderBrush;
		private Thickness _GridViewHeaderBorderThickness = UserStyles.GridViewHeaderBorderThickness;
		private SolidColorBrush _GridViewHeaderFontBrush = UserStyles.GridViewHeaderFontBrush;
		private FontFamily _GridViewHeaderFontFamily = UserStyles.GridViewHeaderFontFamily;
		private double _GridViewHeaderFontSize = UserStyles.GridViewHeaderFontSize;
		private FontWeight _GridViewHeaderFontWeight = UserStyles.GridViewHeaderFontWeight;
		private SolidColorBrush _GridViewMouseOverBrush = UserStyles.GridViewMouseOverBrush;
		private SolidColorBrush _ListViewAlternateRowBrush = UserStyles.ListViewAlternateRowBrush;
		private SolidColorBrush _ListViewBackgroundBrush = UserStyles.ListViewBackgroundBrush;
		private SolidColorBrush _ListViewFontBrush = UserStyles.ListViewFontBrush;
		private FontFamily _ListViewFontFamily = UserStyles.ListViewFontFamily;
		private SolidColorBrush _ListViewFontMouseOverBrush = UserStyles.ListViewFontMouseOverBrush;
		private SolidColorBrush _ListViewFontSelectionBrush = UserStyles.ListViewFontSelectionBrush;
		private double _ListViewFontSize = UserStyles.ListViewFontSize;
		private FontWeight _ListViewFontWeight = UserStyles.ListViewFontWeight;
		private double _ListViewImageHeight = UserStyles.ListViewImageHeight;
		private Size _ListViewImageSize = UserStyles.ListViewImageSize;
		private double _ListViewImageWidth = UserStyles.ListViewImageWidth;
		private SolidColorBrush _ListViewSelectionBrush = UserStyles.ListViewSelectionBrush;
		private SolidColorBrush _MenuFontBrush = UserStyles.MenuFontBrush;
		private FontFamily _MenuFontFamily = UserStyles.MenuFontFamily;
		private SolidColorBrush _MenuFontMouseOverBrush = UserStyles.MenuFontMouseOverBrush;
		private double _MenuFontSize = UserStyles.MenuFontSize;
		private FontWeight _MenuFontWeight = UserStyles.MenuFontWeight;
		private SolidColorBrush _MenuSubBackgroundBrush = UserStyles.MenuSubBackgroundBrush;
		private SolidColorBrush _MenuSubFontBrush = UserStyles.MenuSubFontBrush;
		private FontFamily _MenuSubFontFamily = UserStyles.MenuSubFontFamily;
		private SolidColorBrush _MenuSubFontMouseOverBrush = UserStyles.MenuSubFontMouseOverBrush;
		private double _MenuSubFontSize = UserStyles.MenuSubFontSize;
		private FontWeight _MenuSubFontWeight = UserStyles.MenuSubFontWeight;
		private SolidColorBrush _MenuSubMouseOverBrush = UserStyles.MenuSubMouseOverBrush;
		private SolidColorBrush _TabBackgroundBrush = UserStyles.TabBackgroundBrush;
		private SolidColorBrush _TabFontBrush = UserStyles.TabFontBrush;
		private FontFamily _TabFontFamily = UserStyles.TabFontFamily;
		private SolidColorBrush _TabFontMouseOverBrush = UserStyles.TabFontMouseOverBrush;
		private double _TabFontSize = UserStyles.TabFontSize;
		private FontWeight _TabFontWeight = UserStyles.TabFontWeight;
		private SolidColorBrush _TabSubBackgroundBrush = UserStyles.TabSubBackgroundBrush;
		private SolidColorBrush _TabSubFontBrush = UserStyles.TabSubFontBrush;
		private FontFamily _TabSubFontFamily = UserStyles.TabSubFontFamily;
		private SolidColorBrush _TabSubFontMouseOverBrush = UserStyles.TabSubFontMouseOverBrush;
		private double _TabSubFontSize = UserStyles.TabSubFontSize;
		private FontWeight _TabSubFontWeight = UserStyles.TabSubFontWeight;
		private SolidColorBrush _TabSubSelectionBrush = UserStyles.TabSubSelectionBrush;
		private SolidColorBrush _TextBoxBackgroundBrush = UserStyles.TextBoxBackgroundBrush;
		private SolidColorBrush _TextBoxBorderBrush = UserStyles.TextBoxBorderBrush;
		private Thickness _TextBoxBorderThickness = UserStyles.TextBoxBorderThickness;
		private SolidColorBrush _TextBoxFontBrush = UserStyles.TextBoxFontBrush;
		private FontFamily _TextBoxFontFamily = UserStyles.TextBoxFontFamily;
		private double _TextBoxFontSize = UserStyles.TextBoxFontSize;
		private FontWeight _TextBoxFontWeight = UserStyles.TextBoxFontWeight;
		private double _TileViewImageHeight = UserStyles.TileViewImageHeight;
		private double _TileViewImageWidth = UserStyles.TileViewImageWidth;
		private XElement _xel;

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string property) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public void Save(string path) {
			SetValue("Menu_Font", "font", MenuFontFamily);
			SetValue("Menu_Font", "fontWeight", MenuFontWeight);
			SetValue("Menu_Font", "fontSize", MenuFontSize);
			SetValue("Menu_Font", "foreColor", MenuFontBrush);
			SetValue("Menu_Font", "mouseOverColor", MenuFontMouseOverBrush);
			SetValue("Menu_Subitems", "backColor", MenuSubBackgroundBrush);
			SetValue("Menu_Subitems", "mouseOverColor", MenuSubMouseOverBrush);
			SetValue("Menu_Subitems_Font", "font", MenuSubFontFamily);
			SetValue("Menu_Subitems_Font", "fontWeight", MenuSubFontWeight);
			SetValue("Menu_Subitems_Font", "fontSize", MenuSubFontSize);
			SetValue("Menu_Subitems_Font", "foreColor", MenuSubFontBrush);
			SetValue("Menu_Subitems_Font", "mouseOverColor", MenuSubFontMouseOverBrush);
			SetValue("ListView", "imageHeight", ListViewImageHeight);
			SetValue("ListView", "imageWidth", ListViewImageWidth);
			SetValue("ListView", "backColor", ListViewBackgroundBrush);
			SetValue("ListView", "selectionColor", ListViewSelectionBrush);
			SetValue("ListView", "alternateRowColor", ListViewAlternateRowBrush);
			SetValue("ListView_Font", "font", ListViewFontFamily);
			SetValue("ListView_Font", "fontWeight", ListViewFontWeight);
			SetValue("ListView_Font", "fontSize", ListViewFontSize);
			SetValue("ListView_Font", "foreColor", ListViewFontBrush);
			SetValue("ListView_Font", "mouseOverColor", ListViewFontMouseOverBrush);
			SetValue("ListView_Font", "selectionColor", ListViewFontSelectionBrush);
			SetValue("ListView_Columns", "backColor", GridViewHeaderBackgroundBrush);
			SetValue("ListView_Columns", "borderColor", GridViewHeaderBorderBrush);
			SetValue("ListView_Columns", "borderThickness", GridViewHeaderBorderThickness);
			SetValue("ListView_Columns_Font", "mouseOverColor", GridViewMouseOverBrush);
			SetValue("ListView_Columns_Font", "font", GridViewHeaderFontFamily);
			SetValue("ListView_Columns_Font", "fontWeight", GridViewHeaderFontWeight);
			SetValue("ListView_Columns_Font", "fontSize", GridViewHeaderFontSize);
			SetValue("ListView_Columns_Font", "foreColor", GridViewHeaderFontBrush);
			SetValue("TileView", "imageHeight", TileViewImageHeight);
			SetValue("TileView", "imageWidth", TileViewImageWidth);
			SetValue("Textbox", "backColor", TextBoxBackgroundBrush);
			SetValue("Textbox", "borderThickness", TextBoxBorderThickness);
			SetValue("Textbox", "borderColor", TextBoxBorderBrush);
			SetValue("Textbox_Font", "font", TextBoxFontFamily);
			SetValue("Textbox_Font", "fontWeight", TextBoxFontWeight);
			SetValue("Textbox_Font", "fontSize", TextBoxFontSize);
			SetValue("Textbox_Font", "foreColor", TextBoxFontBrush);
			SetValue("Button", "backColor", ButtonBackgroundBrush);
			SetValue("Button", "mouseOverColor", ButtonMouseOverBrush);
			SetValue("Button", "pressedColor", ButtonPressedBrush);
			SetValue("Button", "disabledBackColor", ButtonDisabledBrush);
			SetValue("Button_Font", "font", ButtonFontFamily);
			SetValue("Button_Font", "fontWeight", ButtonFontWeight);
			SetValue("Button_Font", "fontSize", ButtonFontSize);
			SetValue("Button_Font", "foreColor", ButtonFontBrush);
			SetValue("Button_Font", "disabledForeColor", ButtonFontDisabledBrush);
			SetValue("Button_Font", "mouseOverColor", ButtonFontMouseOverBrush);
			SetValue("Checkbox", "backColor", CheckBoxBackgroundBrush);
			SetValue("Checkbox", "borderThickness", CheckBoxBorderThickness);
			SetValue("Checkbox", "borderColor", CheckBoxBorderBrush);
			SetValue("Checkbox", "mouseOverColor", CheckBoxMouseOverBrush);
			SetValue("Checkbox_Font", "font", CheckBoxFontFamily);
			SetValue("Checkbox_Font", "fontWeight", CheckBoxFontWeight);
			SetValue("Checkbox_Font", "fontSize", CheckBoxFontSize);
			SetValue("Checkbox_Font", "mouseOverColor", CheckBoxFontMouseOverBrush);
			SetValue("Checkbox_Font", "foreColor", CheckBoxFontBrush);
			SetValue("Tab", "backColor", TabBackgroundBrush);
			SetValue("Tab_Font", "font", TabFontFamily);
			SetValue("Tab_Font", "fontWeight", TabFontWeight);
			SetValue("Tab_Font", "fontSize", TabFontSize);
			SetValue("Tab_Font", "mouseOverColor", TabFontMouseOverBrush);
			SetValue("Tab_Font", "foreColor", TabFontBrush);
			SetValue("Tab_Subitems", "backColor", TabSubBackgroundBrush);
			SetValue("Tab_Subitems", "selectionColor", TabSubSelectionBrush);
			SetValue("Tab_Subitems_Font", "font", TabSubFontFamily);
			SetValue("Tab_Subitems_Font", "fontWeight", TabSubFontWeight);
			SetValue("Tab_Subitems_Font", "fontSize", TabSubFontSize);
			SetValue("Tab_Subitems_Font", "mouseOverColor", TabSubFontMouseOverBrush);
			SetValue("Tab_Subitems_Font", "foreColor", TabSubFontBrush);
			_xel.Save(path);
		}

		private void SetValue(string element, string attribute, object value) {
			if (_xel == null)
				_xel = Tools.TryLoad(FilePath);
			var xElement = _xel.Element(element);
			if (xElement != null) xElement.Attribute(attribute).Value = value.ToString();
		}

		public SolidColorBrush ButtonBackgroundBrush {
			get {
				return _ButtonBackgroundBrush;
			}
			set {
				_ButtonBackgroundBrush = value;
				OnPropertyChanged("ButtonBackgroundBrush");
			}
		}

		public SolidColorBrush ButtonDisabledBrush {
			get {
				return _ButtonDisabledBrush;
			}
			set {
				_ButtonDisabledBrush = value;
				OnPropertyChanged("ButtonDisabledBrush");
			}
		}

		public SolidColorBrush ButtonFontBrush {
			get {
				return _ButtonFontBrush;
			}
			set {
				_ButtonFontBrush = value;
				OnPropertyChanged("ButtonFontBrush");
			}
		}

		public SolidColorBrush ButtonFontDisabledBrush {
			get {
				return _ButtonFontDisabledBrush;
			}
			set {
				_ButtonFontDisabledBrush = value;
				OnPropertyChanged("ButtonFontDisabledBrush");
			}
		}

		public FontFamily ButtonFontFamily {
			get {
				return _ButtonFontFamily;
			}
			set {
				_ButtonFontFamily = value;
				OnPropertyChanged("ButtonFontFamily");
			}
		}

		public SolidColorBrush ButtonFontMouseOverBrush {
			get {
				return _ButtonFontMouseOverBrush;
			}
			set {
				_ButtonFontMouseOverBrush = value;
				OnPropertyChanged("ButtonFontMouseOverBrush");
			}
		}

		public double ButtonFontSize {
			get {
				return _ButtonFontSize;
			}
			set {
				_ButtonFontSize = value;
				OnPropertyChanged("ButtonFontSize");
			}
		}

		public FontWeight ButtonFontWeight {
			get {
				return _ButtonFontWeight;
			}
			set {
				_ButtonFontWeight = value;
				OnPropertyChanged("ButtonFontWeight");
			}
		}

		public SolidColorBrush ButtonMouseOverBrush {
			get {
				return _ButtonMouseOverBrush;
			}
			set {
				_ButtonMouseOverBrush = value;
				OnPropertyChanged("ButtonMouseOverBrush");
			}
		}

		public SolidColorBrush ButtonPressedBrush {
			get {
				return _ButtonPressedBrush;
			}
			set {
				_ButtonPressedBrush = value;
				OnPropertyChanged("ButtonPressedBrush");
			}
		}

		public SolidColorBrush CheckBoxBackgroundBrush {
			get {
				return _CheckBoxBackgroundBrush;
			}
			set {
				_CheckBoxBackgroundBrush = value;
				OnPropertyChanged("CheckBoxBackgroundBrush");
			}
		}

		public SolidColorBrush CheckBoxBorderBrush {
			get {
				return _CheckBoxBorderBrush;
			}
			set {
				_CheckBoxBorderBrush = value;
				OnPropertyChanged("CheckBoxBorderBrush");
			}
		}

		public Thickness CheckBoxBorderThickness {
			get {
				return _CheckBoxBorderThickness;
			}
			set {
				_CheckBoxBorderThickness = value;
				OnPropertyChanged("CheckBoxBorderThickness");
			}
		}

		public SolidColorBrush CheckBoxFontBrush {
			get {
				return _CheckBoxFontBrush;
			}
			set {
				_CheckBoxFontBrush = value;
				OnPropertyChanged("CheckBoxFontBrush");
			}
		}

		public FontFamily CheckBoxFontFamily {
			get {
				return _CheckBoxFontFamily;
			}
			set {
				_CheckBoxFontFamily = value;
				OnPropertyChanged("CheckBoxFontFamily");
			}
		}

		public SolidColorBrush CheckBoxFontMouseOverBrush {
			get {
				return _CheckBoxFontMouseOverBrush;
			}
			set {
				_CheckBoxFontMouseOverBrush = value;
				OnPropertyChanged("CheckBoxFontMouseOverBrush");
			}
		}

		public double CheckBoxFontSize {
			get {
				return _CheckBoxFontSize;
			}
			set {
				_CheckBoxFontSize = value;
				OnPropertyChanged("CheckBoxFontSize");
			}
		}

		public FontWeight CheckBoxFontWeight {
			get {
				return _CheckBoxFontWeight;
			}
			set {
				_CheckBoxFontWeight = value;
				OnPropertyChanged("CheckBoxFontWeight");
			}
		}

		public SolidColorBrush CheckBoxMouseOverBrush {
			get {
				return _CheckBoxMouseOverBrush;
			}
			set {
				_CheckBoxMouseOverBrush = value;
				OnPropertyChanged("CheckBoxMouseOverBrush");
			}
		}

		public string FilePath { get; set; }

		public SolidColorBrush GridViewHeaderBackgroundBrush {
			get {
				return _GridViewHeaderBackgroundBrush;
			}
			set {
				_GridViewHeaderBackgroundBrush = value;
				OnPropertyChanged("GridViewHeaderBackgroundBrush");
			}
		}

		public SolidColorBrush GridViewHeaderBorderBrush {
			get {
				return _GridViewHeaderBorderBrush;
			}
			set {
				_GridViewHeaderBorderBrush = value;
				OnPropertyChanged("GridViewHeaderBorderBrush");
			}
		}

		public Thickness GridViewHeaderBorderThickness {
			get {
				return _GridViewHeaderBorderThickness;
			}
			set {
				_GridViewHeaderBorderThickness = value;
				OnPropertyChanged("GridViewHeaderBorderThickness");
			}
		}

		public SolidColorBrush GridViewHeaderFontBrush {
			get {
				return _GridViewHeaderFontBrush;
			}
			set {
				_GridViewHeaderFontBrush = value;
				OnPropertyChanged("GridViewHeaderFontBrush");
			}
		}

		public FontFamily GridViewHeaderFontFamily {
			get {
				return _GridViewHeaderFontFamily;
			}
			set {
				_GridViewHeaderFontFamily = value;
				OnPropertyChanged("GridViewHeaderFontFamily");
			}
		}

		public double GridViewHeaderFontSize {
			get {
				return _GridViewHeaderFontSize;
			}
			set {
				_GridViewHeaderFontSize = value;
				OnPropertyChanged("GridViewHeaderFontSize");
			}
		}

		public FontWeight GridViewHeaderFontWeight {
			get {
				return _GridViewHeaderFontWeight;
			}
			set {
				_GridViewHeaderFontWeight = value;
				OnPropertyChanged("GridViewHeaderFontWeight");
			}
		}

		public SolidColorBrush GridViewMouseOverBrush {
			get {
				return _GridViewMouseOverBrush;
			}
			set {
				_GridViewMouseOverBrush = value;
				OnPropertyChanged("GridViewMouseOverBrush");
			}
		}

		public SolidColorBrush ListViewAlternateRowBrush {
			get {
				return _ListViewAlternateRowBrush;
			}
			set {
				_ListViewAlternateRowBrush = value;
				OnPropertyChanged("ListViewAlternateRowBrush");
			}
		}

		public SolidColorBrush ListViewBackgroundBrush {
			get {
				return _ListViewBackgroundBrush;
			}
			set {
				_ListViewBackgroundBrush = value;
				OnPropertyChanged("ListViewBackgroundBrush");
			}
		}

		public SolidColorBrush ListViewFontBrush {
			get {
				return _ListViewFontBrush;
			}
			set {
				_ListViewFontBrush = value;
				OnPropertyChanged("ListViewFontBrush");
			}
		}

		public FontFamily ListViewFontFamily {
			get {
				return _ListViewFontFamily;
			}
			set {
				_ListViewFontFamily = value;
				OnPropertyChanged("ListViewFontFamily");
			}
		}

		public SolidColorBrush ListViewFontMouseOverBrush {
			get {
				return _ListViewFontMouseOverBrush;
			}
			set {
				_ListViewFontMouseOverBrush = value;
				OnPropertyChanged("ListViewFontMouseOverBrush");
			}
		}

		public SolidColorBrush ListViewFontSelectionBrush {
			get {
				return _ListViewFontSelectionBrush;
			}
			set {
				_ListViewFontSelectionBrush = value;
				OnPropertyChanged("ListViewFontSelectionBrush");
			}
		}

		public double ListViewFontSize {
			get {
				return _ListViewFontSize;
			}
			set {
				_ListViewFontSize = value;
				OnPropertyChanged("ListViewFontSize");
			}
		}

		public FontWeight ListViewFontWeight {
			get {
				return _ListViewFontWeight;
			}
			set {
				_ListViewFontWeight = value;
				OnPropertyChanged("ListViewFontWeight");
			}
		}

		public double ListViewImageHeight {
			get {
				return _ListViewImageHeight;
			}
			set {
				_ListViewImageHeight = value;
				OnPropertyChanged("ListViewImageHeight");
			}
		}

		public Size ListViewImageSize {
			get {
				return _ListViewImageSize;
			}
			set {
				_ListViewImageSize = value;
				OnPropertyChanged("ListViewImageSize");
			}
		}

		public double ListViewImageWidth {
			get {
				return _ListViewImageWidth;
			}
			set {
				_ListViewImageWidth = value;
				OnPropertyChanged("ListViewImageWidth");
			}
		}

		public SolidColorBrush ListViewSelectionBrush {
			get {
				return _ListViewSelectionBrush;
			}
			set {
				_ListViewSelectionBrush = value;
				OnPropertyChanged("ListViewSelectionBrush");
			}
		}

		public SolidColorBrush MenuFontBrush {
			get {
				return _MenuFontBrush;
			}
			set {
				_MenuFontBrush = value;
				OnPropertyChanged("MenuFontBrush");
			}
		}

		public FontFamily MenuFontFamily {
			get {
				return _MenuFontFamily;
			}
			set {
				_MenuFontFamily = value;
				OnPropertyChanged("MenuFontFamily");
			}
		}

		public SolidColorBrush MenuFontMouseOverBrush {
			get {
				return _MenuFontMouseOverBrush;
			}
			set {
				_MenuFontMouseOverBrush = value;
				OnPropertyChanged("MenuFontMouseOverBrush");
			}
		}

		public double MenuFontSize {
			get {
				return _MenuFontSize;
			}
			set {
				_MenuFontSize = value;
				OnPropertyChanged("MenuFontSize");
			}
		}

		public FontWeight MenuFontWeight {
			get {
				return _MenuFontWeight;
			}
			set {
				_MenuFontWeight = value;
				OnPropertyChanged("MenuFontWeight");
			}
		}

		public SolidColorBrush MenuSubBackgroundBrush {
			get {
				return _MenuSubBackgroundBrush;
			}
			set {
				_MenuSubBackgroundBrush = value;
				OnPropertyChanged("MenuSubBackgroundBrush");
			}
		}

		public SolidColorBrush MenuSubFontBrush {
			get {
				return _MenuSubFontBrush;
			}
			set {
				_MenuSubFontBrush = value;
				OnPropertyChanged("MenuSubFontBrush");
			}
		}

		public FontFamily MenuSubFontFamily {
			get {
				return _MenuSubFontFamily;
			}
			set {
				_MenuSubFontFamily = value;
				OnPropertyChanged("MenuSubFontFamily");
			}
		}

		public SolidColorBrush MenuSubFontMouseOverBrush {
			get {
				return _MenuSubFontMouseOverBrush;
			}
			set {
				_MenuSubFontMouseOverBrush = value;
				OnPropertyChanged("MenuSubFontMouseOverBrush");
			}
		}

		public double MenuSubFontSize {
			get {
				return _MenuSubFontSize;
			}
			set {
				_MenuSubFontSize = value;
				OnPropertyChanged("MenuSubFontSize");
			}
		}

		public FontWeight MenuSubFontWeight {
			get {
				return _MenuSubFontWeight;
			}
			set {
				_MenuSubFontWeight = value;
				OnPropertyChanged("MenuSubFontWeight");
			}
		}

		public SolidColorBrush MenuSubMouseOverBrush {
			get {
				return _MenuSubMouseOverBrush;
			}
			set {
				_MenuSubMouseOverBrush = value;
				OnPropertyChanged("MenuSubMouseOverBrush");
			}
		}

		public SolidColorBrush TabBackgroundBrush {
			get {
				return _TabBackgroundBrush;
			}
			set {
				_TabBackgroundBrush = value;
				OnPropertyChanged("TabBackgroundBrush");
			}
		}

		public SolidColorBrush TabFontBrush {
			get {
				return _TabFontBrush;
			}
			set {
				_TabFontBrush = value;
				OnPropertyChanged("TabFontBrush");
			}
		}

		public FontFamily TabFontFamily {
			get {
				return _TabFontFamily;
			}
			set {
				_TabFontFamily = value;
				OnPropertyChanged("TabFontFamily");
			}
		}

		public SolidColorBrush TabFontMouseOverBrush {
			get {
				return _TabFontMouseOverBrush;
			}
			set {
				_TabFontMouseOverBrush = value;
				OnPropertyChanged("TabFontMouseOverBrush");
			}
		}

		public double TabFontSize {
			get {
				return _TabFontSize;
			}
			set {
				_TabFontSize = value;
				OnPropertyChanged("TabFontSize");
			}
		}

		public FontWeight TabFontWeight {
			get {
				return _TabFontWeight;
			}
			set {
				_TabFontWeight = value;
				OnPropertyChanged("TabFontWeight");
			}
		}

		public SolidColorBrush TabSubBackgroundBrush {
			get {
				return _TabSubBackgroundBrush;
			}
			set {
				_TabSubBackgroundBrush = value;
				OnPropertyChanged("TabSubBackgroundBrush");
			}
		}

		public SolidColorBrush TabSubFontBrush {
			get {
				return _TabSubFontBrush;
			}
			set {
				_TabSubFontBrush = value;
				OnPropertyChanged("TabSubFontBrush");
			}
		}

		public FontFamily TabSubFontFamily {
			get {
				return _TabSubFontFamily;
			}
			set {
				_TabSubFontFamily = value;
				OnPropertyChanged("TabSubFontFamily");
			}
		}

		public SolidColorBrush TabSubFontMouseOverBrush {
			get {
				return _TabSubFontMouseOverBrush;
			}
			set {
				_TabSubFontMouseOverBrush = value;
				OnPropertyChanged("TabSubFontMouseOverBrush");
			}
		}

		public double TabSubFontSize {
			get {
				return _TabSubFontSize;
			}
			set {
				_TabSubFontSize = value;
				OnPropertyChanged("TabSubFontSize");
			}
		}

		public FontWeight TabSubFontWeight {
			get {
				return _TabSubFontWeight;
			}
			set {
				_TabSubFontWeight = value;
				OnPropertyChanged("TabSubFontWeight");
			}
		}

		public SolidColorBrush TabSubSelectionBrush {
			get {
				return _TabSubSelectionBrush;
			}
			set {
				_TabSubSelectionBrush = value;
				OnPropertyChanged("TabSubSelectionBrush");
			}
		}

		public SolidColorBrush TextBoxBackgroundBrush {
			get {
				return _TextBoxBackgroundBrush;
			}
			set {
				_TextBoxBackgroundBrush = value;
				OnPropertyChanged("TextBoxBackgroundBrush");
			}
		}

		public SolidColorBrush TextBoxBorderBrush {
			get {
				return _TextBoxBorderBrush;
			}
			set {
				_TextBoxBorderBrush = value;
				OnPropertyChanged("TextBoxBorderBrush");
			}
		}

		public Thickness TextBoxBorderThickness {
			get {
				return _TextBoxBorderThickness;
			}
			set {
				_TextBoxBorderThickness = value;
				OnPropertyChanged("TextBoxBorderThickness");
			}
		}

		public SolidColorBrush TextBoxFontBrush {
			get {
				return _TextBoxFontBrush;
			}
			set {
				_TextBoxFontBrush = value;
				OnPropertyChanged("TextBoxFontBrush");
			}
		}

		public FontFamily TextBoxFontFamily {
			get {
				return _TextBoxFontFamily;
			}
			set {
				_TextBoxFontFamily = value;
				OnPropertyChanged("TextBoxFontFamily");
			}
		}

		public double TextBoxFontSize {
			get {
				return _TextBoxFontSize;
			}
			set {
				_TextBoxFontSize = value;
				OnPropertyChanged("TextBoxFontSize");
			}
		}

		public FontWeight TextBoxFontWeight {
			get {
				return _TextBoxFontWeight;
			}
			set {
				_TextBoxFontWeight = value;
				OnPropertyChanged("TextBoxFontWeight");
			}
		}

		public double TileViewImageHeight {
			get {
				return _TileViewImageHeight;
			}
			set {
				_TileViewImageHeight = value;
				OnPropertyChanged("TileViewImageHeight");
			}
		}

		public double TileViewImageWidth {
			get {
				return _TileViewImageWidth;
			}
			set {
				_TileViewImageWidth = value;
				OnPropertyChanged("TileViewImageWidth");
			}
		}
	}
}

