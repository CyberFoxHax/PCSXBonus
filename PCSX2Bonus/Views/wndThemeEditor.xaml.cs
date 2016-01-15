using System;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PCSX2Bonus.CustomControls.ColorPicker;

namespace PCSX2Bonus.Views {
	public sealed partial class wndThemeEditor {
		private Legacy.EditableTheme _theme = new Legacy.EditableTheme();
		internal ComboBox cbEditorType;
		internal GridViewColumn gvcSerial;
		internal ListView lvGames;
		internal StackPanel spEditor;
		internal wndThemeEditor wndEditor;

		public wndThemeEditor() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			Loaded += wndThemeEditor_Loaded;
		}

		private void AddItem(string name, object value, string bindingName) {
			Func<ComboBoxItem, bool> predicate = null;
			var type = value.GetType();
			var element = new Grid {
				Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
			};
			var definition4 = new ColumnDefinition {
				Width = new GridLength(126.0)
			};
			element.ColumnDefinitions.Add(definition4);
			var definition5 = new ColumnDefinition {
				Width = new GridLength(1.0, GridUnitType.Star)
			};
			element.ColumnDefinitions.Add(definition5);
			var block = new TextBlock {
				Text = name,
				Foreground = Brushes.White
			};
			element.Children.Add(block);
			if (((type == typeof(double)) || (type == typeof(int))) || ((type == typeof(float)) || (type == typeof(Thickness)))) {
				var binding = new Binding(bindingName) {
					Mode = BindingMode.TwoWay
				};
				var textBox = new TextBox();
				textBox.TextChanged += delegate {
					foreach (var ch in textBox.Text) {
						switch (ch) {
							case ',':
							case '.':
								return;
						}
					}
					textBox.Text = new string((
						from c in textBox.Text
						where char.IsDigit(c)
						select c
					).ToArray());
					textBox.SelectionStart = textBox.Text.Length;
				};
				textBox.Text = value.ToString();
				Grid.SetColumn(textBox, 1);
				textBox.SetBinding(TextBox.TextProperty, binding);
				element.Children.Add(textBox);
			}
			else if (type == typeof(SolidColorBrush)) {
				var binding2 = new Binding(bindingName) {
					Mode = BindingMode.TwoWay,
					Converter = Resources["brushToColorConverter"] as Legacy.BrushToColorConverter
				};
				var box = new ColorComboBox {
					Margin = new Thickness(0.0, -4.0, 0.0, 4.0)
				};
				Grid.SetColumn(box, 1);
				box.SetBinding(ColorComboBox.SelectedColorProperty, binding2);
				element.Children.Add(box);
			}
			else {
				if (type == typeof(string)) {
					var grid2 = new Grid {
						Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
					};
					var definition = new ColumnDefinition {
						Width = new GridLength(0.3, GridUnitType.Star)
					};
					grid2.ColumnDefinitions.Add(definition);
					var definition2 = new ColumnDefinition {
						Width = new GridLength(1.0, GridUnitType.Auto)
					};
					grid2.ColumnDefinitions.Add(definition2);
					var definition3 = new ColumnDefinition {
						Width = new GridLength(0.3, GridUnitType.Star)
					};
					grid2.ColumnDefinitions.Add(definition3);
					var grid3 = new Grid {
						Height = 1.0,
						Background = Brushes.White
					};
					Grid.SetColumn(grid3, 0);
					var grid4 = new Grid {
						Height = 1.0,
						Background = Brushes.White
					};
					Grid.SetColumn(grid4, 2);
					var block2 = new TextBlock {
						Text = value.ToString(),
						Foreground = Brushes.White,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Margin = new Thickness(5.0, 0.0, 5.0, 0.0)
					};
					Grid.SetColumn(block2, 1);
					grid2.Children.Add(grid3);
					grid2.Children.Add(grid4);
					grid2.Children.Add(block2);
					spEditor.Children.Add(grid2);
					return;
				}
				if (type == typeof(FontFamily)) {
					var binding3 = new Binding(bindingName) {
						Mode = BindingMode.TwoWay
					};
					var list = (from f in new InstalledFontCollection().Families select new ComboBoxItem { Content = f.Name, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch, FontFamily = new FontFamily(f.Name) }).ToList<ComboBoxItem>();
					var combo = new ComboBox {
						ItemsSource = list
					};
					combo.SetBinding(FontFamilyProperty, binding3);
					combo.SelectionChanged += delegate {
						if (combo.SelectedItem != null) {
							var selectedItem = (ComboBoxItem)combo.SelectedItem;
							combo.FontFamily = selectedItem.FontFamily;
						}
					};
					predicate = c => c.FontFamily.ToString() == value.ToString();
					combo.SelectedItem = combo.Items.Cast<ComboBoxItem>().FirstOrDefault<ComboBoxItem>(predicate);
					Grid.SetColumn(combo, 1);
					element.Children.Add(combo);
				}
				else if (type == typeof(FontWeight)) {
					var binding4 = new Binding(bindingName) {
						Mode = BindingMode.TwoWay
					};
					var combo = new ComboBox {
						ItemsSource = Resources["fontWeights"] as FontWeight[]
					};
					combo.SetBinding(FontWeightProperty, binding4);
					combo.SelectionChanged += delegate {
						if (combo.SelectedItem != null) {
							combo.FontWeight = (FontWeight)combo.SelectedItem;
						}
					};
					combo.SelectedItem = (FontWeight)value;
					Grid.SetColumn(combo, 1);
					element.Children.Add(combo);
				}
			}
			spEditor.Children.Add(element);
		}

		private void cbEditorType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if ((cbEditorType.SelectedItem == null) || (spEditor == null)) return;
			spEditor.Children.Clear();
			switch (cbEditorType.SelectedIndex) {
				case 0:
					LoadMenuEdits();
					return;

				case 1:
					LoadListViewEdits();
					return;

				case 2:
					LoadTextBoxEdits();
					return;

				case 3:
					LoadButtonEdits();
					return;

				case 4:
					LoadCheckBoxEdits();
					return;

				case 5:
					LoadTabEdits();
					return;
			}
		}

		private void LoadButtonEdits() {
			AddItem("", "Common", "");
			AddItem("Back Color:", Theme.ButtonBackgroundBrush, "ButtonBackgroundBrush");
			AddItem("Mouse Over Color:", Theme.ButtonMouseOverBrush, "ButtonMouseOverBrush");
			AddItem("Pressed Color:", Theme.ButtonPressedBrush, "ButtonPressedBrush");
			AddItem("Disabled Color:", Theme.ButtonDisabledBrush, "ButtonDisabledBrush");
			AddItem("", "Font", "");
			AddItem("Font Family:", Theme.ButtonFontFamily, "ButtonFontFamily");
			AddItem("Font Weight:", Theme.ButtonFontWeight, "ButtonFontWeight");
			AddItem("Font Size:", Theme.ButtonFontSize, "ButtonFontSize");
			AddItem("Fore Color:", Theme.ButtonFontBrush, "ButtonFontBrush");
			AddItem("Mouse Over Color:", Theme.ButtonFontMouseOverBrush, "ButtonFontMouseOverBrush");
			AddItem("Disabled Color:", Theme.ButtonFontDisabledBrush, "ButtonFontDisabledBrush");
		}

		private void LoadCheckBoxEdits() {
			AddItem("", "Common", "");
			AddItem("Back Color:", Theme.CheckBoxBackgroundBrush, "CheckBoxBackgroundBrush");
			AddItem("Mouse Over Color:", Theme.CheckBoxMouseOverBrush, "CheckBoxMouseOverBrush");
			AddItem("Border Thickness:", Theme.TextBoxBorderThickness, "TextBoxBorderThickness");
			AddItem("Border Color:", Theme.TextBoxBorderBrush, "TextBoxBorderBrush");
			AddItem("", "Font", "");
			AddItem("Font Family:", Theme.CheckBoxFontFamily, "CheckBoxFontFamily");
			AddItem("Font Weight:", Theme.CheckBoxFontWeight, "CheckBoxFontWeight");
			AddItem("Font Size:", Theme.CheckBoxFontSize, "CheckBoxFontSize");
			AddItem("Fore Color:", Theme.CheckBoxFontBrush, "CheckBoxFontBrush");
			AddItem("Mouse Over Color:", Theme.CheckBoxFontMouseOverBrush, "CheckBoxFontMouseOverBrush");
		}

		private void LoadListViewEdits() {
			lvGames.Items.Clear();
			var newItem = new Legacy.DummyGame {
				Title = "DUMMY GAME",
				Region = "DUMMY REGION",
				Serial = "DUMMY SERIAL",
				Compatibility = 5,
				ImagePath = ""
			};
			lvGames.Items.Add(newItem);
			var game2 = new Legacy.DummyGame {
				Title = "DUMMY GAME",
				Region = "DUMMY REGION",
				Serial = "DUMMY SERIAL",
				Compatibility = 5,
				ImagePath = ""
			};
			lvGames.Items.Add(game2);
			var game3 = new Legacy.DummyGame {
				Title = "DUMMY GAME",
				Region = "DUMMY REGION",
				Serial = "DUMMY SERIAL",
				Compatibility = 5,
				ImagePath = ""
			};
			lvGames.Items.Add(game3);
			var game4 = new Legacy.DummyGame {
				Title = "DUMMY GAME",
				Region = "DUMMY REGION",
				Serial = "DUMMY SERIAL",
				Compatibility = 5,
				ImagePath = ""
			};
			lvGames.Items.Add(game4);
			AddItem("", "Common", "");
			AddItem("Image Height:", Theme.ListViewImageHeight, "ListViewImageHeight");
			AddItem("Image Width:", Theme.ListViewImageHeight, "ListViewImageWidth");
			AddItem("Tile Image Height:", Theme.TileViewImageHeight, "TileViewImageHeight");
			AddItem("Time Image Width:", Theme.TileViewImageWidth, "TileViewImageWidth");
			AddItem("Back Color:", Theme.ListViewBackgroundBrush, "ListViewBackgroundBrush");
			AddItem("Selection Color:", Theme.ListViewSelectionBrush, "ListViewSelectionBrush");
			AddItem("Alternate Row Color:", Theme.ListViewAlternateRowBrush, "ListViewAlternateRowBrush");
			AddItem("", "Font", "");
			AddItem("Font Family:", Theme.ListViewFontFamily, "ListViewFontFamily");
			AddItem("Font Weight:", Theme.ListViewFontWeight, "ListViewFontWeight");
			AddItem("Font Size:", Theme.ListViewFontSize, "ListViewFontSize");
			AddItem("Fore Color:", Theme.ListViewFontBrush, "ListViewFontBrush");
			AddItem("Mouse Over Color:", Theme.ListViewFontMouseOverBrush, "ListViewFontMouseOverBrush");
			AddItem("Selection Color:", Theme.ListViewFontSelectionBrush, "ListViewFontSelectionBrush");
			AddItem("", "Column", "");
			AddItem("Back Color:", Theme.GridViewHeaderBackgroundBrush, "GridViewHeaderBackgroundBrush");
			AddItem("Border Color:", Theme.GridViewHeaderBorderBrush, "GridViewHeaderBorderBrush");
			AddItem("Border Thickness:", Theme.GridViewHeaderBorderThickness, "GridViewHeaderBorderThickness");
			AddItem("", "Column Font", "");
			AddItem("Font Family:", Theme.GridViewHeaderFontFamily, "GridViewHeaderFontFamily");
			AddItem("Font Weight:", Theme.GridViewHeaderFontWeight, "GridViewHeaderFontWeight");
			AddItem("Font Size:", Theme.GridViewHeaderFontSize, "GridViewHeaderFontSize");
			AddItem("Fore Color:", Theme.GridViewHeaderFontBrush, "GridViewHeaderFontBrush");
			AddItem("Mouse Over Color:", Theme.GridViewMouseOverBrush, "GridViewMouseOverBrush");
		}

		private void LoadMenuEdits() {
			AddItem("", "Sub Items", "");
			AddItem("Back Color:", Theme.MenuSubBackgroundBrush, "MenuSubBackgroundBrush");
			AddItem("Mouse Over Color:", Theme.MenuSubMouseOverBrush, "MenuSubMouseOverBrush");
			AddItem("", "Sub Items Font", "");
			AddItem("Font Family:", Theme.MenuSubFontFamily, "MenuSubFontFamily");
			AddItem("Font Weight:", Theme.MenuSubFontWeight, "MenuSubFontWeight");
			AddItem("Font Size:", Theme.MenuSubFontSize, "MenuSubFontSize");
			AddItem("Fore Color:", Theme.MenuSubFontBrush, "MenuSubFontBrush");
			AddItem("Mouse Over Color:", Theme.MenuSubFontMouseOverBrush, "MenuSubFontMouseOverBrush");
		}

		private void LoadTabEdits() {
			AddItem("", "Common", "");
			AddItem("Back Color:", Theme.TabBackgroundBrush, "TabBackgroundBrush");
			AddItem("", "Font", "");
			AddItem("Font Family:", Theme.TabFontFamily, "TabFontFamily");
			AddItem("Font Weight:", Theme.TabFontWeight, "TabFontWeight");
			AddItem("Font Size:", Theme.TabFontSize, "TabFontSize");
			AddItem("Fore Color:", Theme.TabFontBrush, "TabFontBrush");
			AddItem("", "Sub Items", "");
			AddItem("Back Color:", Theme.TabSubBackgroundBrush, "TabSubBackgroundBrush");
			AddItem("Selection Color:", Theme.TabSubSelectionBrush, "TabSubSelectionBrush");
			AddItem("", "Sub Items Font", "");
			AddItem("Font Family:", Theme.TabSubFontFamily, "TabSubFontFamily");
			AddItem("Font Weight:", Theme.TabSubFontWeight, "TabSubFontWeight");
			AddItem("Font Size:", Theme.TabSubFontSize, "TabSubFontSize");
			AddItem("Fore Color:", Theme.TabSubFontBrush, "TabSubFontBrush");
			AddItem("Mouse Over Color:", Theme.TabSubFontMouseOverBrush, "TabSubFontMouseOverBrush");
		}

		private void LoadTextBoxEdits() {
			AddItem("", "Common", "");
			AddItem("Back Color:", Theme.TextBoxBackgroundBrush, "TextBoxBackgroundBrush");
			AddItem("Border Thickness:", Theme.TextBoxBorderThickness, "TextBoxBorderThickness");
			AddItem("Border Color:", Theme.TextBoxBorderBrush, "TextBoxBorderBrush");
			AddItem("", "Font", "");
			AddItem("Font Family:", Theme.TextBoxFontFamily, "TextBoxFontFamily");
			AddItem("Font Weight:", Theme.TextBoxFontWeight, "TextBoxFontWeight");
			AddItem("Font Size:", Theme.TextBoxFontSize, "TextBoxFontSize");
			AddItem("Fore Color:", Theme.TextBoxFontBrush, "TextBoxFontBrush");
		}

		private void Setup() {
			Closing += wndThemeEditor_Closing;
			Theme.FilePath = Tag.ToString();
			LoadListViewEdits();
		}

		private void wndThemeEditor_Closing(object sender, CancelEventArgs e) {
			if (MessageBox.Show("Save changes?", "PCSX2Bonus", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
			Theme.Save(Theme.FilePath);
			Legacy.Tools.ShowMessage("Theme saved! Changes will be applied on next launch", Legacy.MessageType.Info);
		}

		private void wndThemeEditor_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}

		public Legacy.EditableTheme Theme {
			get {
				return _theme;
			}
			set {
				_theme = value;
			}
		}
	}
}

