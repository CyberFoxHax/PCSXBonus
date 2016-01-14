using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PCSX2Bonus.Legacy
{
	public sealed class ImageButton : ToggleButton
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
        public static readonly DependencyProperty MouseOverColorProperty = DependencyProperty.Register("MouseOverColor", typeof(SolidColorBrush), typeof(ImageButton), new PropertyMetadata(null));
        public static readonly DependencyProperty MouseOverImageProperty = DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

        static ImageButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageSource InActiveImage
        {
            get
            {
                return (ImageSource) base.GetValue(ImageProperty);
            }
            set
            {
                base.SetValue(ImageProperty, value);
            }
        }

        public SolidColorBrush MouseOverColor
        {
            get
            {
                return (SolidColorBrush) base.GetValue(MouseOverColorProperty);
            }
            set
            {
                base.SetValue(MouseOverColorProperty, value);
            }
        }

        public ImageSource MouseOverImage
        {
            get
            {
                return (ImageSource) base.GetValue(MouseOverImageProperty);
            }
            set
            {
                base.SetValue(MouseOverImageProperty, value);
            }
        }
    }
}

