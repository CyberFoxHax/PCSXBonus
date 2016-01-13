using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class ColorComboBox : Control {
		public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorComboBox), new UIPropertyMetadata(false, OnIsDropDownOpenChanged));
		private ColorPicker _mColorPicker;
		private ToggleButton _mToggleButton;
		private bool _mWithinChange;
		public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorComboBox), new UIPropertyMetadata(Colors.Transparent, OnSelectedColorPropertyChanged));

		static ColorComboBox() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorComboBox), new FrameworkPropertyMetadata(typeof(ColorComboBox)));
			EventManager.RegisterClassHandler(typeof(ColorComboBox), ColorPicker.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(OnColorPickerSelectedColorChanged));
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			_mColorPicker = GetTemplateChild("PART_ColorPicker") as ColorPicker;
			_mToggleButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;
			if (_mColorPicker != null) {
				_mColorPicker.SelectedColor = SelectedColor;
			}
		}

		private static void OnColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e) {
			var box = sender as ColorComboBox;
			if (box == null || box._mWithinChange == false) return;
			box._mWithinChange = true;
			if (box._mColorPicker != null)
				box.SelectedColor = box._mColorPicker.SelectedColor;
			box._mWithinChange = false;
		}

		private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var colorComboBox = d as ColorComboBox;
			var newValue = (bool)e.NewValue;
			if (colorComboBox == null || colorComboBox._mToggleButton == null) return;
			Action method = () => colorComboBox._mToggleButton.IsHitTestVisible = !newValue;
			colorComboBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
		}

		private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var box = d as ColorComboBox;
			if (box == null || box._mWithinChange == false) return;
			box._mWithinChange = true;
			if (box._mColorPicker != null)
				box._mColorPicker.SelectedColor = box.SelectedColor;
			box._mWithinChange = false;
		}

		public bool IsDropDownOpen {
			get {
				return (bool)GetValue(IsDropDownOpenProperty);
			}
			set {
				SetValue(IsDropDownOpenProperty, value);
			}
		}

		public Color SelectedColor {
			get {
				return (Color)GetValue(SelectedColorProperty);
			}
			set {
				SetValue(SelectedColorProperty, value);
			}
		}
	}
}

