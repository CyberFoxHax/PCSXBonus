using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class ColorPicker : Control {
		private const string AlphaColorSliderName = "PART_AlphaColorSlider";
		private const string BlueColorSliderName = "PART_BlueColorSlider";
		public static readonly DependencyProperty FixedSliderColorProperty = DependencyProperty.Register("FixedSliderColor", typeof(bool), typeof(SpectrumSlider), new UIPropertyMetadata(false, OnFixedSliderColorPropertyChanged));
		private const string GreenColorSliderName = "PART_GreenColorSlider";
		private const string HsvControlName = "PART_HsvControl";
		private ColorSlider _mAlphaColorSlider;
		private ColorSlider _mBlueColorSlider;
		private ColorSlider _mGreenColorSlider;
		private HsvControl _mHsvControl;
		private ColorSlider _mRedColorSlider;
		private SpectrumSlider _mSpectrumSlider;
		private bool _mTemplateApplied;
		private bool _mWithinChange;
		private const string RedColorSliderName = "PART_RedColorSlider";
		public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));
		public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new UIPropertyMetadata(Colors.Black, OnSelectedColorPropertyChanged));
		private const string SpectrumSliderName = "PART_SpectrumSlider1";

		public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged {
			add {
				AddHandler(SelectedColorChangedEvent, value);
			}
			remove {
				RemoveHandler(SelectedColorChangedEvent, value);
			}
		}

		static ColorPicker() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
			EventManager.RegisterClassHandler(typeof(ColorPicker), RangeBase.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(OnSliderValueChanged));
			EventManager.RegisterClassHandler(typeof(ColorPicker), HsvControl.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(OnHsvControlSelectedColorChanged));
		}

		private Color GetHsvColor() {
			var selectedColor = _mHsvControl.SelectedColor;
			selectedColor.A = (byte)_mAlphaColorSlider.Value;
			return selectedColor;
		}

		private Color GetRgbColor() {
			return Color.FromArgb((byte)_mAlphaColorSlider.Value, (byte)_mRedColorSlider.Value, (byte)_mGreenColorSlider.Value, (byte)_mBlueColorSlider.Value);
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			_mRedColorSlider = GetTemplateChild("PART_RedColorSlider") as ColorSlider;
			_mGreenColorSlider = GetTemplateChild("PART_GreenColorSlider") as ColorSlider;
			_mBlueColorSlider = GetTemplateChild("PART_BlueColorSlider") as ColorSlider;
			_mAlphaColorSlider = GetTemplateChild("PART_AlphaColorSlider") as ColorSlider;
			_mSpectrumSlider = GetTemplateChild("PART_SpectrumSlider1") as SpectrumSlider;
			_mHsvControl = GetTemplateChild("PART_HsvControl") as HsvControl;
			_mTemplateApplied = true;
			UpdateControlColors(SelectedColor);
		}

		private static void OnFixedSliderColorPropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e){
			var colorPicker = relatedObject as ColorPicker;
			if (colorPicker != null) colorPicker.UpdateColorSlidersBackground();
		}

		private void OnHsvControlSelectedColorChanged(RoutedPropertyChangedEventArgs<Color> e) {
			if (_mWithinChange) return;

			_mWithinChange = true;
			var hsvColor = GetHsvColor();
			UpdateRgbColors(hsvColor);
			UpdateSelectedColor(hsvColor);
			_mWithinChange = false;
		}

		private static void OnHsvControlSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e){
			var colorPicker = sender as ColorPicker;
			if (colorPicker != null) colorPicker.OnHsvControlSelectedColorChanged(e);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if ((e.Property == IsVisibleProperty) && ((bool)e.NewValue)) {
				Focus();
			}
			base.OnPropertyChanged(e);
		}

		private void OnSelectedColorPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (!_mTemplateApplied || _mWithinChange) return;
			_mWithinChange = true;
			var newValue = (Color)e.NewValue;
			UpdateControlColors(newValue);
			_mWithinChange = false;
		}

		private static void OnSelectedColorPropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e){
			var colorPicker = relatedObject as ColorPicker;
			if (colorPicker != null) colorPicker.OnSelectedColorPropertyChanged(e);
		}

		private void OnSliderValueChanged(RoutedPropertyChangedEventArgs<double> e) {
			if (_mWithinChange) return;
			_mWithinChange = true;
			if (((e.OriginalSource == _mRedColorSlider) || (e.OriginalSource == _mGreenColorSlider)) || ((e.OriginalSource == _mBlueColorSlider) || (e.OriginalSource == _mAlphaColorSlider))) {
				var rgbColor = GetRgbColor();
				UpdateHsvControlColor(rgbColor);
				UpdateSelectedColor(rgbColor);
			}
			else if (e.OriginalSource == _mSpectrumSlider) {
				var hue = _mSpectrumSlider.Hue;
				UpdateHsvControlHue(hue);
				var hsvColor = GetHsvColor();
				UpdateRgbColors(hsvColor);
				UpdateSelectedColor(hsvColor);
			}
			_mWithinChange = false;
		}

		private static void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){
			var colorPicker = sender as ColorPicker;
			if (colorPicker != null) colorPicker.OnSliderValueChanged(e);
		}

		private void SetColorSliderBackground(ColorSlider colorSlider, Color leftColor, Color rightColor) {
			colorSlider.LeftColor = leftColor;
			colorSlider.RightColor = rightColor;
		}

		private void UpdateColorSlidersBackground() {
			if (FixedSliderColor) {
				SetColorSliderBackground(_mRedColorSlider, Colors.Red, Colors.Red);
				SetColorSliderBackground(_mGreenColorSlider, Colors.Green, Colors.Green);
				SetColorSliderBackground(_mBlueColorSlider, Colors.Blue, Colors.Blue);
				SetColorSliderBackground(_mAlphaColorSlider, Colors.Transparent, Colors.White);
			}
			else {
				var r = SelectedColor.R;
				var g = SelectedColor.G;
				var b = SelectedColor.B;
				SetColorSliderBackground(_mRedColorSlider, Color.FromRgb(0, g, b), Color.FromRgb(0xff, g, b));
				SetColorSliderBackground(_mGreenColorSlider, Color.FromRgb(r, 0, b), Color.FromRgb(r, 0xff, b));
				SetColorSliderBackground(_mBlueColorSlider, Color.FromRgb(r, g, 0), Color.FromRgb(r, g, 0xff));
				SetColorSliderBackground(_mAlphaColorSlider, Color.FromArgb(0, r, g, b), Color.FromArgb(0xff, r, g, b));
			}
		}

		private void UpdateControlColors(Color newColor) {
			UpdateRgbColors(newColor);
			UpdateSpectrumColor(newColor);
			UpdateHsvControlColor(newColor);
			UpdateColorSlidersBackground();
		}

		private void UpdateHsvControlColor(Color newColor) {
			double num;
			double num2;
			double num3;
			ColorUtils.ConvertRgbToHsv(newColor, out num, out num2, out num3);
			if ((num2 != 0.0) && (num3 != 0.0)) {
				_mHsvControl.Hue = num;
			}
			_mHsvControl.Saturation = num2;
			_mHsvControl.Value = num3;
			_mSpectrumSlider.Hue = _mHsvControl.Hue;
		}

		private void UpdateHsvControlHue(double hue) {
			_mHsvControl.Hue = hue;
		}

		private void UpdateRgbColors(Color newColor) {
			_mAlphaColorSlider.Value = newColor.A;
			_mRedColorSlider.Value = newColor.R;
			_mGreenColorSlider.Value = newColor.G;
			_mBlueColorSlider.Value = newColor.B;
		}

		private void UpdateSelectedColor(Color newColor) {
			var selectedColor = SelectedColor;
			SelectedColor = newColor;
			if (!FixedSliderColor) {
				UpdateColorSlidersBackground();
			}
			ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, selectedColor, newColor);
		}

		private void UpdateSpectrumColor(Color newColor) {
		}

		public bool FixedSliderColor {
			get {
				return (bool)GetValue(FixedSliderColorProperty);
			}
			set {
				SetValue(FixedSliderColorProperty, value);
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

