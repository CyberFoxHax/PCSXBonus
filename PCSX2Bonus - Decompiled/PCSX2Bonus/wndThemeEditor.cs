namespace PCSX2Bonus
{
    using ColorPicker;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    public sealed class wndThemeEditor : Window, IComponentConnector
    {
        private bool _contentLoaded;
        private EditableTheme _theme = new EditableTheme();
        internal ComboBox cbEditorType;
        internal GridViewColumn gvcSerial;
        internal ListView lvGames;
        internal StackPanel spEditor;
        internal wndThemeEditor wndEditor;

        public wndThemeEditor()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndThemeEditor_Loaded);
        }

        private void AddItem(string name, object value, string bindingName)
        {
            Func<ComboBoxItem, bool> predicate = null;
            Type type = value.GetType();
            Grid element = new Grid {
                Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
            };
            ColumnDefinition definition4 = new ColumnDefinition {
                Width = new GridLength(126.0)
            };
            element.ColumnDefinitions.Add(definition4);
            ColumnDefinition definition5 = new ColumnDefinition {
                Width = new GridLength(1.0, GridUnitType.Star)
            };
            element.ColumnDefinitions.Add(definition5);
            TextBlock block = new TextBlock {
                Text = name,
                Foreground = System.Windows.Media.Brushes.White
            };
            element.Children.Add(block);
            if (((type == typeof(double)) || (type == typeof(int))) || ((type == typeof(float)) || (type == typeof(Thickness))))
            {
                Binding binding = new Binding(bindingName) {
                    Mode = BindingMode.TwoWay
                };
                TextBox textBox = new TextBox();
                textBox.TextChanged += delegate (object o, TextChangedEventArgs e) {
                    foreach (char ch in textBox.Text)
                    {
                        switch (ch)
                        {
                            case ',':
                            case '.':
                                return;
                        }
                    }
                    textBox.Text = new string((from c in textBox.Text
                        where char.IsDigit(c)
                        select c).ToArray<char>());
                    textBox.SelectionStart = textBox.Text.Length;
                };
                textBox.Text = value.ToString();
                Grid.SetColumn(textBox, 1);
                textBox.SetBinding(TextBox.TextProperty, binding);
                element.Children.Add(textBox);
            }
            else if (type == typeof(SolidColorBrush))
            {
                Binding binding2 = new Binding(bindingName) {
                    Mode = BindingMode.TwoWay,
                    Converter = base.Resources["brushToColorConverter"] as BrushToColorConverter
                };
                ColorComboBox box = new ColorComboBox {
                    Margin = new Thickness(0.0, -4.0, 0.0, 4.0)
                };
                Grid.SetColumn(box, 1);
                box.SetBinding(ColorComboBox.SelectedColorProperty, binding2);
                element.Children.Add(box);
            }
            else
            {
                if (type == typeof(string))
                {
                    Grid grid2 = new Grid {
                        Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
                    };
                    ColumnDefinition definition = new ColumnDefinition {
                        Width = new GridLength(0.3, GridUnitType.Star)
                    };
                    grid2.ColumnDefinitions.Add(definition);
                    ColumnDefinition definition2 = new ColumnDefinition {
                        Width = new GridLength(1.0, GridUnitType.Auto)
                    };
                    grid2.ColumnDefinitions.Add(definition2);
                    ColumnDefinition definition3 = new ColumnDefinition {
                        Width = new GridLength(0.3, GridUnitType.Star)
                    };
                    grid2.ColumnDefinitions.Add(definition3);
                    Grid grid3 = new Grid {
                        Height = 1.0,
                        Background = System.Windows.Media.Brushes.White
                    };
                    Grid.SetColumn(grid3, 0);
                    Grid grid4 = new Grid {
                        Height = 1.0,
                        Background = System.Windows.Media.Brushes.White
                    };
                    Grid.SetColumn(grid4, 2);
                    TextBlock block2 = new TextBlock {
                        Text = value.ToString(),
                        Foreground = System.Windows.Media.Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5.0, 0.0, 5.0, 0.0)
                    };
                    Grid.SetColumn(block2, 1);
                    grid2.Children.Add(grid3);
                    grid2.Children.Add(grid4);
                    grid2.Children.Add(block2);
                    this.spEditor.Children.Add(grid2);
                    return;
                }
                if (type == typeof(System.Windows.Media.FontFamily))
                {
                    Binding binding3 = new Binding(bindingName) {
                        Mode = BindingMode.TwoWay
                    };
                    List<ComboBoxItem> list = (from f in new InstalledFontCollection().Families select new ComboBoxItem { Content = f.Name, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch, FontFamily = new System.Windows.Media.FontFamily(f.Name) }).ToList<ComboBoxItem>();
                    ComboBox combo = new ComboBox {
                        ItemsSource = list
                    };
                    combo.SetBinding(Control.FontFamilyProperty, binding3);
                    combo.SelectionChanged += delegate (object o, SelectionChangedEventArgs e) {
                        if (combo.SelectedItem != null)
                        {
                            ComboBoxItem selectedItem = (ComboBoxItem) combo.SelectedItem;
                            combo.FontFamily = selectedItem.FontFamily;
                        }
                    };
                    if (predicate == null)
                    {
                        predicate = c => c.FontFamily.ToString() == value.ToString();
                    }
                    combo.SelectedItem = combo.Items.Cast<ComboBoxItem>().FirstOrDefault<ComboBoxItem>(predicate);
                    Grid.SetColumn(combo, 1);
                    element.Children.Add(combo);
                }
                else if (type == typeof(FontWeight))
                {
                    Binding binding4 = new Binding(bindingName) {
                        Mode = BindingMode.TwoWay
                    };
                    ComboBox combo = new ComboBox {
                        ItemsSource = base.Resources["fontWeights"] as FontWeight[]
                    };
                    combo.SetBinding(Control.FontWeightProperty, binding4);
                    combo.SelectionChanged += delegate (object o, SelectionChangedEventArgs e) {
                        if (combo.SelectedItem != null)
                        {
                            combo.FontWeight = (FontWeight) combo.SelectedItem;
                        }
                    };
                    combo.SelectedItem = (FontWeight) value;
                    Grid.SetColumn(combo, 1);
                    element.Children.Add(combo);
                }
            }
            this.spEditor.Children.Add(element);
        }

        private void cbEditorType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((this.cbEditorType.SelectedItem != null) && (this.spEditor != null))
            {
                this.spEditor.Children.Clear();
                switch (this.cbEditorType.SelectedIndex)
                {
                    case 0:
                        this.LoadMenuEdits();
                        return;

                    case 1:
                        this.LoadListViewEdits();
                        return;

                    case 2:
                        this.LoadTextBoxEdits();
                        return;

                    case 3:
                        this.LoadButtonEdits();
                        return;

                    case 4:
                        this.LoadCheckBoxEdits();
                        return;

                    case 5:
                        this.LoadTabEdits();
                        return;
                }
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndthemeeditor.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void LoadButtonEdits()
        {
            this.AddItem("", "Common", "");
            this.AddItem("Back Color:", this.Theme.ButtonBackgroundBrush, "ButtonBackgroundBrush");
            this.AddItem("Mouse Over Color:", this.Theme.ButtonMouseOverBrush, "ButtonMouseOverBrush");
            this.AddItem("Pressed Color:", this.Theme.ButtonPressedBrush, "ButtonPressedBrush");
            this.AddItem("Disabled Color:", this.Theme.ButtonDisabledBrush, "ButtonDisabledBrush");
            this.AddItem("", "Font", "");
            this.AddItem("Font Family:", this.Theme.ButtonFontFamily, "ButtonFontFamily");
            this.AddItem("Font Weight:", this.Theme.ButtonFontWeight, "ButtonFontWeight");
            this.AddItem("Font Size:", this.Theme.ButtonFontSize, "ButtonFontSize");
            this.AddItem("Fore Color:", this.Theme.ButtonFontBrush, "ButtonFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.ButtonFontMouseOverBrush, "ButtonFontMouseOverBrush");
            this.AddItem("Disabled Color:", this.Theme.ButtonFontDisabledBrush, "ButtonFontDisabledBrush");
        }

        private void LoadCheckBoxEdits()
        {
            this.AddItem("", "Common", "");
            this.AddItem("Back Color:", this.Theme.CheckBoxBackgroundBrush, "CheckBoxBackgroundBrush");
            this.AddItem("Mouse Over Color:", this.Theme.CheckBoxMouseOverBrush, "CheckBoxMouseOverBrush");
            this.AddItem("Border Thickness:", this.Theme.TextBoxBorderThickness, "TextBoxBorderThickness");
            this.AddItem("Border Color:", this.Theme.TextBoxBorderBrush, "TextBoxBorderBrush");
            this.AddItem("", "Font", "");
            this.AddItem("Font Family:", this.Theme.CheckBoxFontFamily, "CheckBoxFontFamily");
            this.AddItem("Font Weight:", this.Theme.CheckBoxFontWeight, "CheckBoxFontWeight");
            this.AddItem("Font Size:", this.Theme.CheckBoxFontSize, "CheckBoxFontSize");
            this.AddItem("Fore Color:", this.Theme.CheckBoxFontBrush, "CheckBoxFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.CheckBoxFontMouseOverBrush, "CheckBoxFontMouseOverBrush");
        }

        private void LoadListViewEdits()
        {
            this.lvGames.Items.Clear();
            DummyGame newItem = new DummyGame {
                Title = "DUMMY GAME",
                Region = "DUMMY REGION",
                Serial = "DUMMY SERIAL",
                Compatibility = 5,
                ImagePath = ""
            };
            this.lvGames.Items.Add(newItem);
            DummyGame game2 = new DummyGame {
                Title = "DUMMY GAME",
                Region = "DUMMY REGION",
                Serial = "DUMMY SERIAL",
                Compatibility = 5,
                ImagePath = ""
            };
            this.lvGames.Items.Add(game2);
            DummyGame game3 = new DummyGame {
                Title = "DUMMY GAME",
                Region = "DUMMY REGION",
                Serial = "DUMMY SERIAL",
                Compatibility = 5,
                ImagePath = ""
            };
            this.lvGames.Items.Add(game3);
            DummyGame game4 = new DummyGame {
                Title = "DUMMY GAME",
                Region = "DUMMY REGION",
                Serial = "DUMMY SERIAL",
                Compatibility = 5,
                ImagePath = ""
            };
            this.lvGames.Items.Add(game4);
            this.AddItem("", "Common", "");
            this.AddItem("Image Height:", this.Theme.ListViewImageHeight, "ListViewImageHeight");
            this.AddItem("Image Width:", this.Theme.ListViewImageHeight, "ListViewImageWidth");
            this.AddItem("Tile Image Height:", this.Theme.TileViewImageHeight, "TileViewImageHeight");
            this.AddItem("Time Image Width:", this.Theme.TileViewImageWidth, "TileViewImageWidth");
            this.AddItem("Back Color:", this.Theme.ListViewBackgroundBrush, "ListViewBackgroundBrush");
            this.AddItem("Selection Color:", this.Theme.ListViewSelectionBrush, "ListViewSelectionBrush");
            this.AddItem("Alternate Row Color:", this.Theme.ListViewAlternateRowBrush, "ListViewAlternateRowBrush");
            this.AddItem("", "Font", "");
            this.AddItem("Font Family:", this.Theme.ListViewFontFamily, "ListViewFontFamily");
            this.AddItem("Font Weight:", this.Theme.ListViewFontWeight, "ListViewFontWeight");
            this.AddItem("Font Size:", this.Theme.ListViewFontSize, "ListViewFontSize");
            this.AddItem("Fore Color:", this.Theme.ListViewFontBrush, "ListViewFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.ListViewFontMouseOverBrush, "ListViewFontMouseOverBrush");
            this.AddItem("Selection Color:", this.Theme.ListViewFontSelectionBrush, "ListViewFontSelectionBrush");
            this.AddItem("", "Column", "");
            this.AddItem("Back Color:", this.Theme.GridViewHeaderBackgroundBrush, "GridViewHeaderBackgroundBrush");
            this.AddItem("Border Color:", this.Theme.GridViewHeaderBorderBrush, "GridViewHeaderBorderBrush");
            this.AddItem("Border Thickness:", this.Theme.GridViewHeaderBorderThickness, "GridViewHeaderBorderThickness");
            this.AddItem("", "Column Font", "");
            this.AddItem("Font Family:", this.Theme.GridViewHeaderFontFamily, "GridViewHeaderFontFamily");
            this.AddItem("Font Weight:", this.Theme.GridViewHeaderFontWeight, "GridViewHeaderFontWeight");
            this.AddItem("Font Size:", this.Theme.GridViewHeaderFontSize, "GridViewHeaderFontSize");
            this.AddItem("Fore Color:", this.Theme.GridViewHeaderFontBrush, "GridViewHeaderFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.GridViewMouseOverBrush, "GridViewMouseOverBrush");
        }

        private void LoadMenuEdits()
        {
            this.AddItem("", "Sub Items", "");
            this.AddItem("Back Color:", this.Theme.MenuSubBackgroundBrush, "MenuSubBackgroundBrush");
            this.AddItem("Mouse Over Color:", this.Theme.MenuSubMouseOverBrush, "MenuSubMouseOverBrush");
            this.AddItem("", "Sub Items Font", "");
            this.AddItem("Font Family:", this.Theme.MenuSubFontFamily, "MenuSubFontFamily");
            this.AddItem("Font Weight:", this.Theme.MenuSubFontWeight, "MenuSubFontWeight");
            this.AddItem("Font Size:", this.Theme.MenuSubFontSize, "MenuSubFontSize");
            this.AddItem("Fore Color:", this.Theme.MenuSubFontBrush, "MenuSubFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.MenuSubFontMouseOverBrush, "MenuSubFontMouseOverBrush");
        }

        private void LoadTabEdits()
        {
            this.AddItem("", "Common", "");
            this.AddItem("Back Color:", this.Theme.TabBackgroundBrush, "TabBackgroundBrush");
            this.AddItem("", "Font", "");
            this.AddItem("Font Family:", this.Theme.TabFontFamily, "TabFontFamily");
            this.AddItem("Font Weight:", this.Theme.TabFontWeight, "TabFontWeight");
            this.AddItem("Font Size:", this.Theme.TabFontSize, "TabFontSize");
            this.AddItem("Fore Color:", this.Theme.TabFontBrush, "TabFontBrush");
            this.AddItem("", "Sub Items", "");
            this.AddItem("Back Color:", this.Theme.TabSubBackgroundBrush, "TabSubBackgroundBrush");
            this.AddItem("Selection Color:", this.Theme.TabSubSelectionBrush, "TabSubSelectionBrush");
            this.AddItem("", "Sub Items Font", "");
            this.AddItem("Font Family:", this.Theme.TabSubFontFamily, "TabSubFontFamily");
            this.AddItem("Font Weight:", this.Theme.TabSubFontWeight, "TabSubFontWeight");
            this.AddItem("Font Size:", this.Theme.TabSubFontSize, "TabSubFontSize");
            this.AddItem("Fore Color:", this.Theme.TabSubFontBrush, "TabSubFontBrush");
            this.AddItem("Mouse Over Color:", this.Theme.TabSubFontMouseOverBrush, "TabSubFontMouseOverBrush");
        }

        private void LoadTextBoxEdits()
        {
            this.AddItem("", "Common", "");
            this.AddItem("Back Color:", this.Theme.TextBoxBackgroundBrush, "TextBoxBackgroundBrush");
            this.AddItem("Border Thickness:", this.Theme.TextBoxBorderThickness, "TextBoxBorderThickness");
            this.AddItem("Border Color:", this.Theme.TextBoxBorderBrush, "TextBoxBorderBrush");
            this.AddItem("", "Font", "");
            this.AddItem("Font Family:", this.Theme.TextBoxFontFamily, "TextBoxFontFamily");
            this.AddItem("Font Weight:", this.Theme.TextBoxFontWeight, "TextBoxFontWeight");
            this.AddItem("Font Size:", this.Theme.TextBoxFontSize, "TextBoxFontSize");
            this.AddItem("Fore Color:", this.Theme.TextBoxFontBrush, "TextBoxFontBrush");
        }

        private void Setup()
        {
            base.Closing += new CancelEventHandler(this.wndThemeEditor_Closing);
            this.Theme.FilePath = base.Tag.ToString();
            this.LoadListViewEdits();
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.wndEditor = (wndThemeEditor) target;
                    return;

                case 2:
                    this.cbEditorType = (ComboBox) target;
                    this.cbEditorType.SelectionChanged += new SelectionChangedEventHandler(this.cbEditorType_SelectionChanged);
                    return;

                case 3:
                    this.lvGames = (ListView) target;
                    return;

                case 4:
                    this.gvcSerial = (GridViewColumn) target;
                    return;

                case 5:
                    this.spEditor = (StackPanel) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndThemeEditor_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Save changes?", "PCSX2Bonus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Theme.Save(this.Theme.FilePath);
                Tools.ShowMessage("Theme saved! Changes will be applied on next launch", MessageType.Info);
            }
        }

        private void wndThemeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }

        public EditableTheme Theme
        {
            get
            {
                return this._theme;
            }
            set
            {
                this._theme = value;
            }
        }
    }
}

