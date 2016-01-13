using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class SpectrumSlider : Slider {
		public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(SpectrumSlider), new UIPropertyMetadata(0.0, OnHuePropertyChanged));
		private bool _mWithinChanging;

		static SpectrumSlider() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumSlider), new FrameworkPropertyMetadata(typeof(SpectrumSlider)));
		}

		public SpectrumSlider() {
			SetBackground();
		}

		private static void OnHuePropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e) {
			var slider = relatedObject as SpectrumSlider;
			if (slider == null || slider._mWithinChanging) return;
			slider._mWithinChanging = true;
			var newValue = (double)e.NewValue;
			slider.Value = 360.0 - newValue;
			slider._mWithinChanging = false;
		}

		protected override void OnValueChanged(double oldValue, double newValue) {
			base.OnValueChanged(oldValue, newValue);
			if (_mWithinChanging || BindingOperations.IsDataBound(this, HueProperty)) return;
			_mWithinChanging = true;
			Hue = 360.0 - newValue;
			_mWithinChanging = false;
		}

		private void SetBackground() {
			var brush = new LinearGradientBrush {
				StartPoint = new Point(0.5, 0.0),
				EndPoint = new Point(0.5, 1.0)
			};
			var spectrumColors = ColorUtils.GetSpectrumColors(30);
			for (var i = 0; i < 30; i++) {
				var offset = (i * 1.0) / 30.0;
				var stop = new GradientStop(spectrumColors[i], offset);
				brush.GradientStops.Add(stop);
			}

			Background = brush;
		}

		public double Hue {
			get {
				return (double)GetValue(HueProperty);
			}
			set {
				SetValue(HueProperty, value);
			}
		}
	}
}

