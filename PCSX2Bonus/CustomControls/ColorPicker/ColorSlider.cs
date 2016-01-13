using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class ColorSlider : Slider {
		public static readonly DependencyProperty LeftColorProperty = DependencyProperty.Register("LeftColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.Black));
		public static readonly DependencyProperty RightColorProperty = DependencyProperty.Register("RightColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.White));

		static ColorSlider() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
		}

		public Color LeftColor {
			get {
				return (Color)GetValue(LeftColorProperty);
			}
			set {
				SetValue(LeftColorProperty, value);
			}
		}

		public Color RightColor {
			get {
				return (Color)GetValue(RightColorProperty);
			}
			set {
				SetValue(RightColorProperty, value);
			}
		}
	}
}

