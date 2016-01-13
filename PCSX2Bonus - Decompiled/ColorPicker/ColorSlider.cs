namespace ColorPicker
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public sealed class ColorSlider : Slider
    {
        public static readonly DependencyProperty LeftColorProperty = DependencyProperty.Register("LeftColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.Black));
        public static readonly DependencyProperty RightColorProperty = DependencyProperty.Register("RightColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.White));

        static ColorSlider()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public Color LeftColor
        {
            get
            {
                return (Color) base.GetValue(LeftColorProperty);
            }
            set
            {
                base.SetValue(LeftColorProperty, value);
            }
        }

        public Color RightColor
        {
            get
            {
                return (Color) base.GetValue(RightColorProperty);
            }
            set
            {
                base.SetValue(RightColorProperty, value);
            }
        }
    }
}

