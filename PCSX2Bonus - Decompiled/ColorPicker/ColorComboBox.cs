namespace ColorPicker
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Threading;

    public sealed class ColorComboBox : Control
    {
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorComboBox), new UIPropertyMetadata(false, new PropertyChangedCallback(ColorComboBox.OnIsDropDownOpenChanged)));
        private ColorPicker.ColorPicker m_colorPicker;
        private UIElement m_popup;
        private ToggleButton m_toggleButton;
        private bool m_withinChange;
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorComboBox), new UIPropertyMetadata(Colors.Transparent, new PropertyChangedCallback(ColorComboBox.OnSelectedColorPropertyChanged)));

        static ColorComboBox()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorComboBox), new FrameworkPropertyMetadata(typeof(ColorComboBox)));
            EventManager.RegisterClassHandler(typeof(ColorComboBox), ColorPicker.ColorPicker.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(ColorComboBox.OnColorPickerSelectedColorChanged));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_popup = base.GetTemplateChild("PART_Popup") as UIElement;
            this.m_colorPicker = base.GetTemplateChild("PART_ColorPicker") as ColorPicker.ColorPicker;
            this.m_toggleButton = base.GetTemplateChild("PART_ToggleButton") as ToggleButton;
            if (this.m_colorPicker != null)
            {
                this.m_colorPicker.SelectedColor = this.SelectedColor;
            }
        }

        private static void OnColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            ColorComboBox box = sender as ColorComboBox;
            if (!box.m_withinChange)
            {
                box.m_withinChange = true;
                if (box.m_colorPicker != null)
                {
                    box.SelectedColor = box.m_colorPicker.SelectedColor;
                }
                box.m_withinChange = false;
            }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Action method = null;
            ColorComboBox colorComboBox = d as ColorComboBox;
            bool newValue = (bool) e.NewValue;
            if (colorComboBox.m_toggleButton != null)
            {
                if (method == null)
                {
                    method = () => colorComboBox.m_toggleButton.IsHitTestVisible = !newValue;
                }
                colorComboBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorComboBox box = d as ColorComboBox;
            if (!box.m_withinChange)
            {
                box.m_withinChange = true;
                if (box.m_colorPicker != null)
                {
                    box.m_colorPicker.SelectedColor = box.SelectedColor;
                }
                box.m_withinChange = false;
            }
        }

        public bool IsDropDownOpen
        {
            get
            {
                return (bool) base.GetValue(IsDropDownOpenProperty);
            }
            set
            {
                base.SetValue(IsDropDownOpenProperty, value);
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

