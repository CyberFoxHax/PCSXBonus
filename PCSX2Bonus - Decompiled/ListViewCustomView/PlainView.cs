namespace ListViewCustomView
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public sealed class PlainView : ViewBase
    {
        public static readonly DependencyProperty ItemContainerStyleProperty = ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(PlainView));
        public static readonly DependencyProperty ItemHeightProperty = WrapPanel.ItemHeightProperty.AddOwner(typeof(PlainView));
        public static readonly DependencyProperty ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(typeof(PlainView));
        public static readonly DependencyProperty ItemWidthProperty = WrapPanel.ItemWidthProperty.AddOwner(typeof(PlainView));

        protected override object DefaultStyleKey
        {
            get
            {
                return new ComponentResourceKey(base.GetType(), "myPlainViewDSK");
            }
        }

        public Style ItemContainerStyle
        {
            get
            {
                return (Style) base.GetValue(ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ItemContainerStyleProperty, value);
            }
        }

        public double ItemHeight
        {
            get
            {
                return (double) base.GetValue(ItemHeightProperty);
            }
            set
            {
                base.SetValue(ItemHeightProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(ItemTemplateProperty);
            }
            set
            {
                base.SetValue(ItemTemplateProperty, value);
            }
        }

        public double ItemWidth
        {
            get
            {
                return (double) base.GetValue(ItemWidthProperty);
            }
            set
            {
                base.SetValue(ItemWidthProperty, value);
            }
        }
    }
}

