using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class HsvControl : Control {
		public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, OnHueChanged));
		private Thumb _mThumb;
		private readonly TranslateTransform _mThumbTransform = new TranslateTransform();
		public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, OnSaturationChanged));
		public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(HsvControl));
		public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(HsvControl), new UIPropertyMetadata(Colors.Transparent));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, OnValueChanged));

		public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged {
			add {
				AddHandler(SelectedColorChangedEvent, value);
			}
			remove {
				RemoveHandler(SelectedColorChangedEvent, value);
			}
		}

		static HsvControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HsvControl), new FrameworkPropertyMetadata(typeof(HsvControl)));
			EventManager.RegisterClassHandler(typeof(HsvControl), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
		}

		private static double LimitValue(double value, double max) {
			if (value < 0.0) value = 0.0;
			if (value > max) value = max;
			return value;
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			_mThumb = GetTemplateChild("PART_Thumb") as Thumb;
			if (_mThumb == null) return;
			UpdateThumbPosition();
			_mThumb.RenderTransform = _mThumbTransform;
		}

		private static void OnHueChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e) {
			var control = relatedObject as HsvControl;
			if (control != null) {
				control.UpdateSelectedColor();
			}
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if (_mThumb != null) {
				var position = e.GetPosition(this);
				UpdatePositionAndSaturationAndValue(position.X, position.Y);
				_mThumb.RaiseEvent(e);
			}
			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			UpdateThumbPosition();
			base.OnRenderSizeChanged(sizeInfo);
		}

		private static void OnSaturationChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e) {
			var control = relatedObject as HsvControl;
			if (control != null) {
				control.UpdateThumbPosition();
			}
		}

		private void OnThumbDragDelta(DragDeltaEventArgs e) {
			var positionX = _mThumbTransform.X + e.HorizontalChange;
			var positionY = _mThumbTransform.Y + e.VerticalChange;
			UpdatePositionAndSaturationAndValue(positionX, positionY);
		}

		private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e) {
			var hsvControl = sender as HsvControl;
			if (hsvControl != null)
				hsvControl.OnThumbDragDelta(e);
		}

		private static void OnValueChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e) {
			var control = relatedObject as HsvControl;
			if (control != null) {
				control.UpdateThumbPosition();
			}
		}

		private void UpdatePositionAndSaturationAndValue(double positionX, double positionY) {
			positionX = LimitValue(positionX, ActualWidth);
			positionY = LimitValue(positionY, ActualHeight);
			_mThumbTransform.X = positionX;
			_mThumbTransform.Y = positionY;
			Saturation = positionX / ActualWidth;
			Value = 1.0 - positionY / ActualHeight;
			UpdateSelectedColor();
		}

		private void UpdateSelectedColor() {
			var selectedColor = SelectedColor;
			var newColor = ColorUtils.ConvertHsvToRgb(Hue, Saturation, Value);
			SelectedColor = newColor;
			ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, selectedColor, newColor);
		}

		private void UpdateThumbPosition() {
			_mThumbTransform.X = Saturation * ActualWidth;
			_mThumbTransform.Y = (1.0 - Value) * ActualHeight;
			SelectedColor = ColorUtils.ConvertHsvToRgb(Hue, Saturation, Value);
		}

		public double Hue {
			get {
				return (double)GetValue(HueProperty);
			}
			set {
				SetValue(HueProperty, value);
			}
		}

		public double Saturation {
			get {
				return (double)GetValue(SaturationProperty);
			}
			set {
				SetValue(SaturationProperty, value);
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

		public double Value {
			get {
				return (double)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
			}
		}
	}
}

