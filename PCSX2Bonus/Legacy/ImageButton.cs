using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PCSX2Bonus.Legacy {
	public sealed class ImageButton : ToggleButton {
		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
		public static readonly DependencyProperty MouseOverColorProperty = DependencyProperty.Register("MouseOverColor", typeof(SolidColorBrush), typeof(ImageButton), new PropertyMetadata(null));
		public static readonly DependencyProperty MouseOverImageProperty = DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

		static ImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}

		public ImageSource InActiveImage {
			get {
				return (ImageSource)GetValue(ImageProperty);
			}
			set {
				SetValue(ImageProperty, value);
			}
		}

		public SolidColorBrush MouseOverColor {
			get {
				return (SolidColorBrush)GetValue(MouseOverColorProperty);
			}
			set {
				SetValue(MouseOverColorProperty, value);
			}
		}

		public ImageSource MouseOverImage {
			get {
				return (ImageSource)GetValue(MouseOverImageProperty);
			}
			set {
				SetValue(MouseOverImageProperty, value);
			}
		}
	}
}

