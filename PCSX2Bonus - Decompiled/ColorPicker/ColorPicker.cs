namespace ColorPicker
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public sealed class ColorPicker : Control
    {
        private const string AlphaColorSliderName = "PART_AlphaColorSlider";
        private const string BlueColorSliderName = "PART_BlueColorSlider";
        public static readonly DependencyProperty FixedSliderColorProperty = DependencyProperty.Register("FixedSliderColor", typeof(bool), typeof(SpectrumSlider), new UIPropertyMetadata(false, new PropertyChangedCallback(ColorPicker.ColorPicker.OnFixedSliderColorPropertyChanged)));
        private const string GreenColorSliderName = "PART_GreenColorSlider";
        private const string HsvControlName = "PART_HsvControl";
        private ColorSlider m_alphaColorSlider;
        private ColorSlider m_blueColorSlider;
        private ColorSlider m_greenColorSlider;
        private HsvControl m_hsvControl;
        private ColorSlider m_redColorSlider;
        private SpectrumSlider m_spectrumSlider;
        private bool m_templateApplied;
        private bool m_withinChange;
        private const string RedColorSliderName = "PART_RedColorSlider";
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker.ColorPicker));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker.ColorPicker), new UIPropertyMetadata(Colors.Black, new PropertyChangedCallback(ColorPicker.ColorPicker.OnSelectedColorPropertyChanged)));
        private const string SpectrumSliderName = "PART_SpectrumSlider1";

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add
            {
                base.AddHandler(SelectedColorChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        static ColorPicker()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker.ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker.ColorPicker)));
            EventManager.RegisterClassHandler(typeof(ColorPicker.ColorPicker), RangeBase.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(ColorPicker.ColorPicker.OnSliderValueChanged));
            EventManager.RegisterClassHandler(typeof(ColorPicker.ColorPicker), HsvControl.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(ColorPicker.ColorPicker.OnHsvControlSelectedColorChanged));
        }

        private Color GetHsvColor()
        {
            Color selectedColor = this.m_hsvControl.SelectedColor;
            selectedColor.A = (byte) this.m_alphaColorSlider.Value;
            return selectedColor;
        }

        private Color GetRgbColor()
        {
            return Color.FromArgb((byte) this.m_alphaColorSlider.Value, (byte) this.m_redColorSlider.Value, (byte) this.m_greenColorSlider.Value, (byte) this.m_blueColorSlider.Value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_redColorSlider = base.GetTemplateChild("PART_RedColorSlider") as ColorSlider;
            this.m_greenColorSlider = base.GetTemplateChild("PART_GreenColorSlider") as ColorSlider;
            this.m_blueColorSlider = base.GetTemplateChild("PART_BlueColorSlider") as ColorSlider;
            this.m_alphaColorSlider = base.GetTemplateChild("PART_AlphaColorSlider") as ColorSlider;
            this.m_spectrumSlider = base.GetTemplateChild("PART_SpectrumSlider1") as SpectrumSlider;
            this.m_hsvControl = base.GetTemplateChild("PART_HsvControl") as HsvControl;
            this.m_templateApplied = true;
            this.UpdateControlColors(this.SelectedColor);
        }

        private static void OnFixedSliderColorPropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            (relatedObject as ColorPicker.ColorPicker).UpdateColorSlidersBackground();
        }

        private void OnHsvControlSelectedColorChanged(RoutedPropertyChangedEventArgs<Color> e)
        {
            if (!this.m_withinChange)
            {
                this.m_withinChange = true;
                Color hsvColor = this.GetHsvColor();
                this.UpdateRgbColors(hsvColor);
                this.UpdateSelectedColor(hsvColor);
                this.m_withinChange = false;
            }
        }

        private static void OnHsvControlSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            (sender as ColorPicker.ColorPicker).OnHsvControlSelectedColorChanged(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.Property == UIElement.IsVisibleProperty) && ((bool) e.NewValue))
            {
                base.Focus();
            }
            base.OnPropertyChanged(e);
        }

        private void OnSelectedColorPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.m_templateApplied && !this.m_withinChange)
            {
                this.m_withinChange = true;
                Color newValue = (Color) e.NewValue;
                this.UpdateControlColors(newValue);
                this.m_withinChange = false;
            }
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            (relatedObject as ColorPicker.ColorPicker).OnSelectedColorPropertyChanged(e);
        }

        private void OnSliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.m_withinChange)
            {
                this.m_withinChange = true;
                if (((e.OriginalSource == this.m_redColorSlider) || (e.OriginalSource == this.m_greenColorSlider)) || ((e.OriginalSource == this.m_blueColorSlider) || (e.OriginalSource == this.m_alphaColorSlider)))
                {
                    Color rgbColor = this.GetRgbColor();
                    this.UpdateHsvControlColor(rgbColor);
                    this.UpdateSelectedColor(rgbColor);
                }
                else if (e.OriginalSource == this.m_spectrumSlider)
                {
                    double hue = this.m_spectrumSlider.Hue;
                    this.UpdateHsvControlHue(hue);
                    Color hsvColor = this.GetHsvColor();
                    this.UpdateRgbColors(hsvColor);
                    this.UpdateSelectedColor(hsvColor);
                }
                this.m_withinChange = false;
            }
        }

        private static void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (sender as ColorPicker.ColorPicker).OnSliderValueChanged(e);
        }

        private void SetColorSliderBackground(ColorSlider colorSlider, Color leftColor, Color rightColor)
        {
            colorSlider.LeftColor = leftColor;
            colorSlider.RightColor = rightColor;
        }

        private void UpdateColorSlidersBackground()
        {
            if (this.FixedSliderColor)
            {
                this.SetColorSliderBackground(this.m_redColorSlider, Colors.Red, Colors.Red);
                this.SetColorSliderBackground(this.m_greenColorSlider, Colors.Green, Colors.Green);
                this.SetColorSliderBackground(this.m_blueColorSlider, Colors.Blue, Colors.Blue);
                this.SetColorSliderBackground(this.m_alphaColorSlider, Colors.Transparent, Colors.White);
            }
            else
            {
                byte r = this.SelectedColor.R;
                byte g = this.SelectedColor.G;
                byte b = this.SelectedColor.B;
                this.SetColorSliderBackground(this.m_redColorSlider, Color.FromRgb(0, g, b), Color.FromRgb(0xff, g, b));
                this.SetColorSliderBackground(this.m_greenColorSlider, Color.FromRgb(r, 0, b), Color.FromRgb(r, 0xff, b));
                this.SetColorSliderBackground(this.m_blueColorSlider, Color.FromRgb(r, g, 0), Color.FromRgb(r, g, 0xff));
                this.SetColorSliderBackground(this.m_alphaColorSlider, Color.FromArgb(0, r, g, b), Color.FromArgb(0xff, r, g, b));
            }
        }

        private void UpdateControlColors(Color newColor)
        {
            this.UpdateRgbColors(newColor);
            this.UpdateSpectrumColor(newColor);
            this.UpdateHsvControlColor(newColor);
            this.UpdateColorSlidersBackground();
        }

        private void UpdateHsvControlColor(Color newColor)
        {
            double num;
            double num2;
            double num3;
            ColorUtils.ConvertRgbToHsv(newColor, out num, out num2, out num3);
            if ((num2 != 0.0) && (num3 != 0.0))
            {
                this.m_hsvControl.Hue = num;
            }
            this.m_hsvControl.Saturation = num2;
            this.m_hsvControl.Value = num3;
            this.m_spectrumSlider.Hue = this.m_hsvControl.Hue;
        }

        private void UpdateHsvControlHue(double hue)
        {
            this.m_hsvControl.Hue = hue;
        }

        private void UpdateRgbColors(Color newColor)
        {
            this.m_alphaColorSlider.Value = newColor.A;
            this.m_redColorSlider.Value = newColor.R;
            this.m_greenColorSlider.Value = newColor.G;
            this.m_blueColorSlider.Value = newColor.B;
        }

        private void UpdateSelectedColor(Color newColor)
        {
            Color selectedColor = this.SelectedColor;
            this.SelectedColor = newColor;
            if (!this.FixedSliderColor)
            {
                this.UpdateColorSlidersBackground();
            }
            ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, selectedColor, newColor);
        }

        private void UpdateSpectrumColor(Color newColor)
        {
        }

        public bool FixedSliderColor
        {
            get
            {
                return (bool) base.GetValue(FixedSliderColorProperty);
            }
            set
            {
                base.SetValue(FixedSliderColorProperty, value);
            }
        }

        public Color SelectedColor
        {
            get
            {
                return (Color) base.GetValue(SelectedColorProperty);
            }
            set
            {
                base.SetValue(SelectedColorProperty, value);
            }
        }
    }
}

