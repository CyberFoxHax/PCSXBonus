namespace ColorPicker
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class HsvControl : Control
    {
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(HsvControl.OnHueChanged)));
        private Thumb m_thumb;
        private TranslateTransform m_thumbTransform = new TranslateTransform();
        private bool m_withinUpdate;
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(HsvControl.OnSaturationChanged)));
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(HsvControl));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(HsvControl), new UIPropertyMetadata(Colors.Transparent));
        private const string ThumbName = "PART_Thumb";
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(HsvControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(HsvControl.OnValueChanged)));

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

        static HsvControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HsvControl), new FrameworkPropertyMetadata(typeof(HsvControl)));
            EventManager.RegisterClassHandler(typeof(HsvControl), Thumb.DragDeltaEvent, new DragDeltaEventHandler(HsvControl.OnThumbDragDelta));
        }

        private double LimitValue(double value, double max)
        {
            if (value < 0.0)
            {
                value = 0.0;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_thumb = base.GetTemplateChild("PART_Thumb") as Thumb;
            if (this.m_thumb != null)
            {
                this.UpdateThumbPosition();
                this.m_thumb.RenderTransform = this.m_thumbTransform;
            }
        }

        private static void OnHueChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            HsvControl control = relatedObject as HsvControl;
            if ((control != null) && !control.m_withinUpdate)
            {
                control.UpdateSelectedColor();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.m_thumb != null)
            {
                Point position = e.GetPosition(this);
                this.UpdatePositionAndSaturationAndValue(position.X, position.Y);
                this.m_thumb.RaiseEvent(e);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.UpdateThumbPosition();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private static void OnSaturationChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            HsvControl control = relatedObject as HsvControl;
            if ((control != null) && !control.m_withinUpdate)
            {
                control.UpdateThumbPosition();
            }
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            double positionX = this.m_thumbTransform.X + e.HorizontalChange;
            double positionY = this.m_thumbTransform.Y + e.VerticalChange;
            this.UpdatePositionAndSaturationAndValue(positionX, positionY);
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            (sender as HsvControl).OnThumbDragDelta(e);
        }

        private static void OnValueChanged(DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            HsvControl control = relatedObject as HsvControl;
            if ((control != null) && !control.m_withinUpdate)
            {
                control.UpdateThumbPosition();
            }
        }

        private void UpdatePositionAndSaturationAndValue(double positionX, double positionY)
        {
            positionX = this.LimitValue(positionX, base.ActualWidth);
            positionY = this.LimitValue(positionY, base.ActualHeight);
            this.m_thumbTransform.X = positionX;
            this.m_thumbTransform.Y = positionY;
            this.Saturation = positionX / base.ActualWidth;
            this.Value = 1.0 - (positionY / base.ActualHeight);
            this.UpdateSelectedColor();
        }

        private void UpdateSelectedColor()
        {
            Color selectedColor = this.SelectedColor;
            Color newColor = ColorUtils.ConvertHsvToRgb(this.Hue, this.Saturation, this.Value);
            this.SelectedColor = newColor;
            ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, selectedColor, newColor);
        }

        private void UpdateThumbPosition()
        {
            this.m_thumbTransform.X = this.Saturation * base.ActualWidth;
            this.m_thumbTransform.Y = (1.0 - this.Value) * base.ActualHeight;
            this.SelectedColor = ColorUtils.ConvertHsvToRgb(this.Hue, this.Saturation, this.Value);
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

        public double Saturation
        {
            get
            {
                return (double) base.GetValue(SaturationProperty);
            }
            set
            {
                base.SetValue(SaturationProperty, value);
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

        public double Value
        {
            get
            {
                return (double) base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
    }
}

