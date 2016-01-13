namespace ColorPicker
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    public sealed class SpectrumSlider : Slider
    {
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(SpectrumSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(SpectrumSlider.OnHuePropertyChanged)));
        private bool m_withinChanging;

        static SpectrumSlider()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumSlider), new FrameworkPropertyMetadata(typeof(SpectrumSlider)));
        }

        public SpectrumSlider()
        {
            this.SetBackground();
        }

        private static void OnHuePropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            SpectrumSlider slider = relatedObject as SpectrumSlider;
            if ((slider != null) && !slider.m_withinChanging)
            {
                slider.m_withinChanging = true;
                double newValue = (double) e.NewValue;
                slider.Value = 360.0 - newValue;
                slider.m_withinChanging = false;
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (!this.m_withinChanging && !BindingOperations.IsDataBound(this, HueProperty))
            {
                this.m_withinChanging = true;
                this.Hue = 360.0 - newValue;
                this.m_withinChanging = false;
            }
        }

        private void SetBackground()
        {
            LinearGradientBrush brush = new LinearGradientBrush {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0)
            };
            Color[] spectrumColors = ColorUtils.GetSpectrumColors(30);
            for (int i = 0; i < 30; i++)
            {
                double offset = (i * 1.0) / 30.0;
                GradientStop stop = new GradientStop(spectrumColors[i], offset);
                brush.GradientStops.Add(stop);
            }
            base.Background = brush;
        }

        public double Hue
        {
            get
            {
                return (double) base.GetValue(HueProperty);
            }
            set
            {
                base.SetValue(HueProperty, value);
            }
        }
    }
}

