namespace PCSX2Bonus
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    public sealed class EditableTheme
    {
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

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Save(string path)
        {
            this.SetValue("Menu_Font", "font", this.MenuFontFamily);
            this.SetValue("Menu_Font", "fontWeight", this.MenuFontWeight);
            this.SetValue("Menu_Font", "fontSize", this.MenuFontSize);
            this.SetValue("Menu_Font", "foreColor", this.MenuFontBrush);
            this.SetValue("Menu_Font", "mouseOverColor", this.MenuFontMouseOverBrush);
            this.SetValue("Menu_Subitems", "backColor", this.MenuSubBackgroundBrush);
            this.SetValue("Menu_Subitems", "mouseOverColor", this.MenuSubMouseOverBrush);
            this.SetValue("Menu_Subitems_Font", "font", this.MenuSubFontFamily);
            this.SetValue("Menu_Subitems_Font", "fontWeight", this.MenuSubFontWeight);
            this.SetValue("Menu_Subitems_Font", "fontSize", this.MenuSubFontSize);
            this.SetValue("Menu_Subitems_Font", "foreColor", this.MenuSubFontBrush);
            this.SetValue("Menu_Subitems_Font", "mouseOverColor", this.MenuSubFontMouseOverBrush);
            this.SetValue("ListView", "imageHeight", this.ListViewImageHeight);
            this.SetValue("ListView", "imageWidth", this.ListViewImageWidth);
            this.SetValue("ListView", "backColor", this.ListViewBackgroundBrush);
            this.SetValue("ListView", "selectionColor", this.ListViewSelectionBrush);
            this.SetValue("ListView", "alternateRowColor", this.ListViewAlternateRowBrush);
            this.SetValue("ListView_Font", "font", this.ListViewFontFamily);
            this.SetValue("ListView_Font", "fontWeight", this.ListViewFontWeight);
            this.SetValue("ListView_Font", "fontSize", this.ListViewFontSize);
            this.SetValue("ListView_Font", "foreColor", this.ListViewFontBrush);
            this.SetValue("ListView_Font", "mouseOverColor", this.ListViewFontMouseOverBrush);
            this.SetValue("ListView_Font", "selectionColor", this.ListViewFontSelectionBrush);
            this.SetValue("ListView_Columns", "backColor", this.GridViewHeaderBackgroundBrush);
            this.SetValue("ListView_Columns", "borderColor", this.GridViewHeaderBorderBrush);
            this.SetValue("ListView_Columns", "borderThickness", this.GridViewHeaderBorderThickness);
            this.SetValue("ListView_Columns_Font", "mouseOverColor", this.GridViewMouseOverBrush);
            this.SetValue("ListView_Columns_Font", "font", this.GridViewHeaderFontFamily);
            this.SetValue("ListView_Columns_Font", "fontWeight", this.GridViewHeaderFontWeight);
            this.SetValue("ListView_Columns_Font", "fontSize", this.GridViewHeaderFontSize);
            this.SetValue("ListView_Columns_Font", "foreColor", this.GridViewHeaderFontBrush);
            this.SetValue("TileView", "imageHeight", this.TileViewImageHeight);
            this.SetValue("TileView", "imageWidth", this.TileViewImageWidth);
            this.SetValue("Textbox", "backColor", this.TextBoxBackgroundBrush);
            this.SetValue("Textbox", "borderThickness", this.TextBoxBorderThickness);
            this.SetValue("Textbox", "borderColor", this.TextBoxBorderBrush);
            this.SetValue("Textbox_Font", "font", this.TextBoxFontFamily);
            this.SetValue("Textbox_Font", "fontWeight", this.TextBoxFontWeight);
            this.SetValue("Textbox_Font", "fontSize", this.TextBoxFontSize);
            this.SetValue("Textbox_Font", "foreColor", this.TextBoxFontBrush);
            this.SetValue("Button", "backColor", this.ButtonBackgroundBrush);
            this.SetValue("Button", "mouseOverColor", this.ButtonMouseOverBrush);
            this.SetValue("Button", "pressedColor", this.ButtonPressedBrush);
            this.SetValue("Button", "disabledBackColor", this.ButtonDisabledBrush);
            this.SetValue("Button_Font", "font", this.ButtonFontFamily);
            this.SetValue("Button_Font", "fontWeight", this.ButtonFontWeight);
            this.SetValue("Button_Font", "fontSize", this.ButtonFontSize);
            this.SetValue("Button_Font", "foreColor", this.ButtonFontBrush);
            this.SetValue("Button_Font", "disabledForeColor", this.ButtonFontDisabledBrush);
            this.SetValue("Button_Font", "mouseOverColor", this.ButtonFontMouseOverBrush);
            this.SetValue("Checkbox", "backColor", this.CheckBoxBackgroundBrush);
            this.SetValue("Checkbox", "borderThickness", this.CheckBoxBorderThickness);
            this.SetValue("Checkbox", "borderColor", this.CheckBoxBorderBrush);
            this.SetValue("Checkbox", "mouseOverColor", this.CheckBoxMouseOverBrush);
            this.SetValue("Checkbox_Font", "font", this.CheckBoxFontFamily);
            this.SetValue("Checkbox_Font", "fontWeight", this.CheckBoxFontWeight);
            this.SetValue("Checkbox_Font", "fontSize", this.CheckBoxFontSize);
            this.SetValue("Checkbox_Font", "mouseOverColor", this.CheckBoxFontMouseOverBrush);
            this.SetValue("Checkbox_Font", "foreColor", this.CheckBoxFontBrush);
            this.SetValue("Tab", "backColor", this.TabBackgroundBrush);
            this.SetValue("Tab_Font", "font", this.TabFontFamily);
            this.SetValue("Tab_Font", "fontWeight", this.TabFontWeight);
            this.SetValue("Tab_Font", "fontSize", this.TabFontSize);
            this.SetValue("Tab_Font", "mouseOverColor", this.TabFontMouseOverBrush);
            this.SetValue("Tab_Font", "foreColor", this.TabFontBrush);
            this.SetValue("Tab_Subitems", "backColor", this.TabSubBackgroundBrush);
            this.SetValue("Tab_Subitems", "selectionColor", this.TabSubSelectionBrush);
            this.SetValue("Tab_Subitems_Font", "font", this.TabSubFontFamily);
            this.SetValue("Tab_Subitems_Font", "fontWeight", this.TabSubFontWeight);
            this.SetValue("Tab_Subitems_Font", "fontSize", this.TabSubFontSize);
            this.SetValue("Tab_Subitems_Font", "mouseOverColor", this.TabSubFontMouseOverBrush);
            this.SetValue("Tab_Subitems_Font", "foreColor", this.TabSubFontBrush);
            this._xel.Save(path);
        }

        private void SetValue(string element, string attribute, object value)
        {
            if (this._xel == null)
            {
                this._xel = Tools.TryLoad(this.FilePath);
            }
            try
            {
                this._xel.Element(element).Attribute(attribute).Value = value.ToString();
            }
            catch
            {
            }
        }

        public SolidColorBrush ButtonBackgroundBrush
        {
            get
            {
                return this._ButtonBackgroundBrush;
            }
            set
            {
                this._ButtonBackgroundBrush = value;
                this.OnPropertyChanged("ButtonBackgroundBrush");
            }
        }

        public SolidColorBrush ButtonDisabledBrush
        {
            get
            {
                return this._ButtonDisabledBrush;
            }
            set
            {
                this._ButtonDisabledBrush = value;
                this.OnPropertyChanged("ButtonDisabledBrush");
            }
        }

        public SolidColorBrush ButtonFontBrush
        {
            get
            {
                return this._ButtonFontBrush;
            }
            set
            {
                this._ButtonFontBrush = value;
                this.OnPropertyChanged("ButtonFontBrush");
            }
        }

        public SolidColorBrush ButtonFontDisabledBrush
        {
            get
            {
                return this._ButtonFontDisabledBrush;
            }
            set
            {
                this._ButtonFontDisabledBrush = value;
                this.OnPropertyChanged("ButtonFontDisabledBrush");
            }
        }

        public FontFamily ButtonFontFamily
        {
            get
            {
                return this._ButtonFontFamily;
            }
            set
            {
                this._ButtonFontFamily = value;
                this.OnPropertyChanged("ButtonFontFamily");
            }
        }

        public SolidColorBrush ButtonFontMouseOverBrush
        {
            get
            {
                return this._ButtonFontMouseOverBrush;
            }
            set
            {
                this._ButtonFontMouseOverBrush = value;
                this.OnPropertyChanged("ButtonFontMouseOverBrush");
            }
        }

        public double ButtonFontSize
        {
            get
            {
                return this._ButtonFontSize;
            }
            set
            {
                this._ButtonFontSize = value;
                this.OnPropertyChanged("ButtonFontSize");
            }
        }

        public FontWeight ButtonFontWeight
        {
            get
            {
                return this._ButtonFontWeight;
            }
            set
            {
                this._ButtonFontWeight = value;
                this.OnPropertyChanged("ButtonFontWeight");
            }
        }

        public SolidColorBrush ButtonMouseOverBrush
        {
            get
            {
                return this._ButtonMouseOverBrush;
            }
            set
            {
                this._ButtonMouseOverBrush = value;
                this.OnPropertyChanged("ButtonMouseOverBrush");
            }
        }

        public SolidColorBrush ButtonPressedBrush
        {
            get
            {
                return this._ButtonPressedBrush;
            }
            set
            {
                this._ButtonPressedBrush = value;
                this.OnPropertyChanged("ButtonPressedBrush");
            }
        }

        public SolidColorBrush CheckBoxBackgroundBrush
        {
            get
            {
                return this._CheckBoxBackgroundBrush;
            }
            set
            {
                this._CheckBoxBackgroundBrush = value;
                this.OnPropertyChanged("CheckBoxBackgroundBrush");
            }
        }

        public SolidColorBrush CheckBoxBorderBrush
        {
            get
            {
                return this._CheckBoxBorderBrush;
            }
            set
            {
                this._CheckBoxBorderBrush = value;
                this.OnPropertyChanged("CheckBoxBorderBrush");
            }
        }

        public Thickness CheckBoxBorderThickness
        {
            get
            {
                return this._CheckBoxBorderThickness;
            }
            set
            {
                this._CheckBoxBorderThickness = value;
                this.OnPropertyChanged("CheckBoxBorderThickness");
            }
        }

        public SolidColorBrush CheckBoxFontBrush
        {
            get
            {
                return this._CheckBoxFontBrush;
            }
            set
            {
                this._CheckBoxFontBrush = value;
                this.OnPropertyChanged("CheckBoxFontBrush");
            }
        }

        public FontFamily CheckBoxFontFamily
        {
            get
            {
                return this._CheckBoxFontFamily;
            }
            set
            {
                this._CheckBoxFontFamily = value;
                this.OnPropertyChanged("CheckBoxFontFamily");
            }
        }

        public SolidColorBrush CheckBoxFontMouseOverBrush
        {
            get
            {
                return this._CheckBoxFontMouseOverBrush;
            }
            set
            {
                this._CheckBoxFontMouseOverBrush = value;
                this.OnPropertyChanged("CheckBoxFontMouseOverBrush");
            }
        }

        public double CheckBoxFontSize
        {
            get
            {
                return this._CheckBoxFontSize;
            }
            set
            {
                this._CheckBoxFontSize = value;
                this.OnPropertyChanged("CheckBoxFontSize");
            }
        }

        public FontWeight CheckBoxFontWeight
        {
            get
            {
                return this._CheckBoxFontWeight;
            }
            set
            {
                this._CheckBoxFontWeight = value;
                this.OnPropertyChanged("CheckBoxFontWeight");
            }
        }

        public SolidColorBrush CheckBoxMouseOverBrush
        {
            get
            {
                return this._CheckBoxMouseOverBrush;
            }
            set
            {
                this._CheckBoxMouseOverBrush = value;
                this.OnPropertyChanged("CheckBoxMouseOverBrush");
            }
        }

        public string FilePath { get; set; }

        public SolidColorBrush GridViewHeaderBackgroundBrush
        {
            get
            {
                return this._GridViewHeaderBackgroundBrush;
            }
            set
            {
                this._GridViewHeaderBackgroundBrush = value;
                this.OnPropertyChanged("GridViewHeaderBackgroundBrush");
            }
        }

        public SolidColorBrush GridViewHeaderBorderBrush
        {
            get
            {
                return this._GridViewHeaderBorderBrush;
            }
            set
            {
                this._GridViewHeaderBorderBrush = value;
                this.OnPropertyChanged("GridViewHeaderBorderBrush");
            }
        }

        public Thickness GridViewHeaderBorderThickness
        {
            get
            {
                return this._GridViewHeaderBorderThickness;
            }
            set
            {
                this._GridViewHeaderBorderThickness = value;
                this.OnPropertyChanged("GridViewHeaderBorderThickness");
            }
        }

        public SolidColorBrush GridViewHeaderFontBrush
        {
            get
            {
                return this._GridViewHeaderFontBrush;
            }
            set
            {
                this._GridViewHeaderFontBrush = value;
                this.OnPropertyChanged("GridViewHeaderFontBrush");
            }
        }

        public FontFamily GridViewHeaderFontFamily
        {
            get
            {
                return this._GridViewHeaderFontFamily;
            }
            set
            {
                this._GridViewHeaderFontFamily = value;
                this.OnPropertyChanged("GridViewHeaderFontFamily");
            }
        }

        public double GridViewHeaderFontSize
        {
            get
            {
                return this._GridViewHeaderFontSize;
            }
            set
            {
                this._GridViewHeaderFontSize = value;
                this.OnPropertyChanged("GridViewHeaderFontSize");
            }
        }

        public FontWeight GridViewHeaderFontWeight
        {
            get
            {
                return this._GridViewHeaderFontWeight;
            }
            set
            {
                this._GridViewHeaderFontWeight = value;
                this.OnPropertyChanged("GridViewHeaderFontWeight");
            }
        }

        public SolidColorBrush GridViewMouseOverBrush
        {
            get
            {
                return this._GridViewMouseOverBrush;
            }
            set
            {
                this._GridViewMouseOverBrush = value;
                this.OnPropertyChanged("GridViewMouseOverBrush");
            }
        }

        public SolidColorBrush ListViewAlternateRowBrush
        {
            get
            {
                return this._ListViewAlternateRowBrush;
            }
            set
            {
                this._ListViewAlternateRowBrush = value;
                this.OnPropertyChanged("ListViewAlternateRowBrush");
            }
        }

        public SolidColorBrush ListViewBackgroundBrush
        {
            get
            {
                return this._ListViewBackgroundBrush;
            }
            set
            {
                this._ListViewBackgroundBrush = value;
                this.OnPropertyChanged("ListViewBackgroundBrush");
            }
        }

        public SolidColorBrush ListViewFontBrush
        {
            get
            {
                return this._ListViewFontBrush;
            }
            set
            {
                this._ListViewFontBrush = value;
                this.OnPropertyChanged("ListViewFontBrush");
            }
        }

        public FontFamily ListViewFontFamily
        {
            get
            {
                return this._ListViewFontFamily;
            }
            set
            {
                this._ListViewFontFamily = value;
                this.OnPropertyChanged("ListViewFontFamily");
            }
        }

        public SolidColorBrush ListViewFontMouseOverBrush
        {
            get
            {
                return this._ListViewFontMouseOverBrush;
            }
            set
            {
                this._ListViewFontMouseOverBrush = value;
                this.OnPropertyChanged("ListViewFontMouseOverBrush");
            }
        }

        public SolidColorBrush ListViewFontSelectionBrush
        {
            get
            {
                return this._ListViewFontSelectionBrush;
            }
            set
            {
                this._ListViewFontSelectionBrush = value;
                this.OnPropertyChanged("ListViewFontSelectionBrush");
            }
        }

        public double ListViewFontSize
        {
            get
            {
                return this._ListViewFontSize;
            }
            set
            {
                this._ListViewFontSize = value;
                this.OnPropertyChanged("ListViewFontSize");
            }
        }

        public FontWeight ListViewFontWeight
        {
            get
            {
                return this._ListViewFontWeight;
            }
            set
            {
                this._ListViewFontWeight = value;
                this.OnPropertyChanged("ListViewFontWeight");
            }
        }

        public double ListViewImageHeight
        {
            get
            {
                return this._ListViewImageHeight;
            }
            set
            {
                this._ListViewImageHeight = value;
                this.OnPropertyChanged("ListViewImageHeight");
            }
        }

        public Size ListViewImageSize
        {
            get
            {
                return this._ListViewImageSize;
            }
            set
            {
                this._ListViewImageSize = value;
                this.OnPropertyChanged("ListViewImageSize");
            }
        }

        public double ListViewImageWidth
        {
            get
            {
                return this._ListViewImageWidth;
            }
            set
            {
                this._ListViewImageWidth = value;
                this.OnPropertyChanged("ListViewImageWidth");
            }
        }

        public SolidColorBrush ListViewSelectionBrush
        {
            get
            {
                return this._ListViewSelectionBrush;
            }
            set
            {
                this._ListViewSelectionBrush = value;
                this.OnPropertyChanged("ListViewSelectionBrush");
            }
        }

        public SolidColorBrush MenuFontBrush
        {
            get
            {
                return this._MenuFontBrush;
            }
            set
            {
                this._MenuFontBrush = value;
                this.OnPropertyChanged("MenuFontBrush");
            }
        }

        public FontFamily MenuFontFamily
        {
            get
            {
                return this._MenuFontFamily;
            }
            set
            {
                this._MenuFontFamily = value;
                this.OnPropertyChanged("MenuFontFamily");
            }
        }

        public SolidColorBrush MenuFontMouseOverBrush
        {
            get
            {
                return this._MenuFontMouseOverBrush;
            }
            set
            {
                this._MenuFontMouseOverBrush = value;
                this.OnPropertyChanged("MenuFontMouseOverBrush");
            }
        }

        public double MenuFontSize
        {
            get
            {
                return this._MenuFontSize;
            }
            set
            {
                this._MenuFontSize = value;
                this.OnPropertyChanged("MenuFontSize");
            }
        }

        public FontWeight MenuFontWeight
        {
            get
            {
                return this._MenuFontWeight;
            }
            set
            {
                this._MenuFontWeight = value;
                this.OnPropertyChanged("MenuFontWeight");
            }
        }

        public SolidColorBrush MenuSubBackgroundBrush
        {
            get
            {
                return this._MenuSubBackgroundBrush;
            }
            set
            {
                this._MenuSubBackgroundBrush = value;
                this.OnPropertyChanged("MenuSubBackgroundBrush");
            }
        }

        public SolidColorBrush MenuSubFontBrush
        {
            get
            {
                return this._MenuSubFontBrush;
            }
            set
            {
                this._MenuSubFontBrush = value;
                this.OnPropertyChanged("MenuSubFontBrush");
            }
        }

        public FontFamily MenuSubFontFamily
        {
            get
            {
                return this._MenuSubFontFamily;
            }
            set
            {
                this._MenuSubFontFamily = value;
                this.OnPropertyChanged("MenuSubFontFamily");
            }
        }

        public SolidColorBrush MenuSubFontMouseOverBrush
        {
            get
            {
                return this._MenuSubFontMouseOverBrush;
            }
            set
            {
                this._MenuSubFontMouseOverBrush = value;
                this.OnPropertyChanged("MenuSubFontMouseOverBrush");
            }
        }

        public double MenuSubFontSize
        {
            get
            {
                return this._MenuSubFontSize;
            }
            set
            {
                this._MenuSubFontSize = value;
                this.OnPropertyChanged("MenuSubFontSize");
            }
        }

        public FontWeight MenuSubFontWeight
        {
            get
            {
                return this._MenuSubFontWeight;
            }
            set
            {
                this._MenuSubFontWeight = value;
                this.OnPropertyChanged("MenuSubFontWeight");
            }
        }

        public SolidColorBrush MenuSubMouseOverBrush
        {
            get
            {
                return this._MenuSubMouseOverBrush;
            }
            set
            {
                this._MenuSubMouseOverBrush = value;
                this.OnPropertyChanged("MenuSubMouseOverBrush");
            }
        }

        public SolidColorBrush TabBackgroundBrush
        {
            get
            {
                return this._TabBackgroundBrush;
            }
            set
            {
                this._TabBackgroundBrush = value;
                this.OnPropertyChanged("TabBackgroundBrush");
            }
        }

        public SolidColorBrush TabFontBrush
        {
            get
            {
                return this._TabFontBrush;
            }
            set
            {
                this._TabFontBrush = value;
                this.OnPropertyChanged("TabFontBrush");
            }
        }

        public FontFamily TabFontFamily
        {
            get
            {
                return this._TabFontFamily;
            }
            set
            {
                this._TabFontFamily = value;
                this.OnPropertyChanged("TabFontFamily");
            }
        }

        public SolidColorBrush TabFontMouseOverBrush
        {
            get
            {
                return this._TabFontMouseOverBrush;
            }
            set
            {
                this._TabFontMouseOverBrush = value;
                this.OnPropertyChanged("TabFontMouseOverBrush");
            }
        }

        public double TabFontSize
        {
            get
            {
                return this._TabFontSize;
            }
            set
            {
                this._TabFontSize = value;
                this.OnPropertyChanged("TabFontSize");
            }
        }

        public FontWeight TabFontWeight
        {
            get
            {
                return this._TabFontWeight;
            }
            set
            {
                this._TabFontWeight = value;
                this.OnPropertyChanged("TabFontWeight");
            }
        }

        public SolidColorBrush TabSubBackgroundBrush
        {
            get
            {
                return this._TabSubBackgroundBrush;
            }
            set
            {
                this._TabSubBackgroundBrush = value;
                this.OnPropertyChanged("TabSubBackgroundBrush");
            }
        }

        public SolidColorBrush TabSubFontBrush
        {
            get
            {
                return this._TabSubFontBrush;
            }
            set
            {
                this._TabSubFontBrush = value;
                this.OnPropertyChanged("TabSubFontBrush");
            }
        }

        public FontFamily TabSubFontFamily
        {
            get
            {
                return this._TabSubFontFamily;
            }
            set
            {
                this._TabSubFontFamily = value;
                this.OnPropertyChanged("TabSubFontFamily");
            }
        }

        public SolidColorBrush TabSubFontMouseOverBrush
        {
            get
            {
                return this._TabSubFontMouseOverBrush;
            }
            set
            {
                this._TabSubFontMouseOverBrush = value;
                this.OnPropertyChanged("TabSubFontMouseOverBrush");
            }
        }

        public double TabSubFontSize
        {
            get
            {
                return this._TabSubFontSize;
            }
            set
            {
                this._TabSubFontSize = value;
                this.OnPropertyChanged("TabSubFontSize");
            }
        }

        public FontWeight TabSubFontWeight
        {
            get
            {
                return this._TabSubFontWeight;
            }
            set
            {
                this._TabSubFontWeight = value;
                this.OnPropertyChanged("TabSubFontWeight");
            }
        }

        public SolidColorBrush TabSubSelectionBrush
        {
            get
            {
                return this._TabSubSelectionBrush;
            }
            set
            {
                this._TabSubSelectionBrush = value;
                this.OnPropertyChanged("TabSubSelectionBrush");
            }
        }

        public SolidColorBrush TextBoxBackgroundBrush
        {
            get
            {
                return this._TextBoxBackgroundBrush;
            }
            set
            {
                this._TextBoxBackgroundBrush = value;
                this.OnPropertyChanged("TextBoxBackgroundBrush");
            }
        }

        public SolidColorBrush TextBoxBorderBrush
        {
            get
            {
                return this._TextBoxBorderBrush;
            }
            set
            {
                this._TextBoxBorderBrush = value;
                this.OnPropertyChanged("TextBoxBorderBrush");
            }
        }

        public Thickness TextBoxBorderThickness
        {
            get
            {
                return this._TextBoxBorderThickness;
            }
            set
            {
                this._TextBoxBorderThickness = value;
                this.OnPropertyChanged("TextBoxBorderThickness");
            }
        }

        public SolidColorBrush TextBoxFontBrush
        {
            get
            {
                return this._TextBoxFontBrush;
            }
            set
            {
                this._TextBoxFontBrush = value;
                this.OnPropertyChanged("TextBoxFontBrush");
            }
        }

        public FontFamily TextBoxFontFamily
        {
            get
            {
                return this._TextBoxFontFamily;
            }
            set
            {
                this._TextBoxFontFamily = value;
                this.OnPropertyChanged("TextBoxFontFamily");
            }
        }

        public double TextBoxFontSize
        {
            get
            {
                return this._TextBoxFontSize;
            }
            set
            {
                this._TextBoxFontSize = value;
                this.OnPropertyChanged("TextBoxFontSize");
            }
        }

        public FontWeight TextBoxFontWeight
        {
            get
            {
                return this._TextBoxFontWeight;
            }
            set
            {
                this._TextBoxFontWeight = value;
                this.OnPropertyChanged("TextBoxFontWeight");
            }
        }

        public double TileViewImageHeight
        {
            get
            {
                return this._TileViewImageHeight;
            }
            set
            {
                this._TileViewImageHeight = value;
                this.OnPropertyChanged("TileViewImageHeight");
            }
        }

        public double TileViewImageWidth
        {
            get
            {
                return this._TileViewImageWidth;
            }
            set
            {
                this._TileViewImageWidth = value;
                this.OnPropertyChanged("TileViewImageWidth");
            }
        }
    }
}

