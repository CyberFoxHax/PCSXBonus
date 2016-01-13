namespace Wpf.Util
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class GridViewSort
    {
        public static readonly DependencyProperty AutoSortProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty PropertyNameProperty;
        public static readonly DependencyProperty ShowSortGlyphProperty;
        private static readonly DependencyProperty SortedColumnHeaderProperty;
        public static readonly DependencyProperty SortGlyphAscendingProperty;
        public static readonly DependencyProperty SortGlyphDescendingProperty;

        static GridViewSort()
        {
            CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(GridViewSort), new UIPropertyMetadata(null, delegate (DependencyObject o, DependencyPropertyChangedEventArgs e) {
                ItemsControl control = o as ItemsControl;
                if ((control != null) && !GetAutoSort(control))
                {
                    if ((e.OldValue != null) && (e.NewValue == null))
                    {
                        control.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewSort.ColumnHeader_Click));
                    }
                    if ((e.OldValue == null) && (e.NewValue != null))
                    {
                        control.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewSort.ColumnHeader_Click));
                    }
                }
            }));
            AutoSortProperty = DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(false, delegate (DependencyObject o, DependencyPropertyChangedEventArgs e) {
                ListView view = o as ListView;
                if ((view != null) && (GetCommand(view) == null))
                {
                    bool oldValue = (bool) e.OldValue;
                    bool newValue = (bool) e.NewValue;
                    if (oldValue && !newValue)
                    {
                        view.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewSort.ColumnHeader_Click));
                    }
                    if (!oldValue && newValue)
                    {
                        view.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewSort.ColumnHeader_Click));
                    }
                }
            }));
            PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort), new UIPropertyMetadata(null));
            ShowSortGlyphProperty = DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(true));
            SortGlyphAscendingProperty = DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));
            SortGlyphDescendingProperty = DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));
            SortedColumnHeaderProperty = DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));
        }

        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
        {
            AdornerLayer.GetAdornerLayer(columnHeader).Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }

        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            ListSortDirection ascending = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription description = view.SortDescriptions[0];
                if (description.PropertyName == propertyName)
                {
                    if (description.Direction == ListSortDirection.Ascending)
                    {
                        ascending = ListSortDirection.Descending;
                    }
                    else
                    {
                        ascending = ListSortDirection.Ascending;
                    }
                }
                view.SortDescriptions.Clear();
                GridViewColumnHeader columnHeader = GetSortedColumnHeader(listView);
                if (columnHeader != null)
                {
                    RemoveSortGlyph(columnHeader);
                }
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, ascending));
                if (GetShowSortGlyph(listView))
                {
                    AddSortGlyph(sortedColumnHeader, ascending, (ascending == ListSortDirection.Ascending) ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
                }
                SetSortedColumnHeader(listView, sortedColumnHeader);
            }
        }

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader originalSource = e.OriginalSource as GridViewColumnHeader;
            if ((originalSource != null) && (originalSource.Column != null))
            {
                string propertyName = GetPropertyName(originalSource.Column);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView ancestor = GetAncestor<ListView>(originalSource);
                    if (ancestor != null)
                    {
                        ICommand command = GetCommand(ancestor);
                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                            {
                                command.Execute(propertyName);
                            }
                        }
                        else if (GetAutoSort(ancestor))
                        {
                            ApplySort(ancestor.Items, propertyName, ancestor, originalSource);
                        }
                    }
                    Console.WriteLine(propertyName);
                }
            }
        }

        public static T GetAncestor<T>(DependencyObject reference) where T: DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent != null)
            {
                return (T) parent;
            }
            return default(T);
        }

        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoSortProperty);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(CommandProperty);
        }

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string) obj.GetValue(PropertyNameProperty);
        }

        public static bool GetShowSortGlyph(DependencyObject obj)
        {
            return (bool) obj.GetValue(ShowSortGlyphProperty);
        }

        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
        {
            return (GridViewColumnHeader) obj.GetValue(SortedColumnHeaderProperty);
        }

        public static ImageSource GetSortGlyphAscending(DependencyObject obj)
        {
            return (ImageSource) obj.GetValue(SortGlyphAscendingProperty);
        }

        public static ImageSource GetSortGlyphDescending(DependencyObject obj)
        {
            return (ImageSource) obj.GetValue(SortGlyphDescendingProperty);
        }

        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            if (adornerLayer != null)
            {
                Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is SortGlyphAdorner)
                        {
                            adornerLayer.Remove(adorner);
                        }
                    }
                }
            }
        }

        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        public static void SetShowSortGlyph(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowSortGlyphProperty, value);
        }

        private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
        {
            obj.SetValue(SortedColumnHeaderProperty, value);
        }

        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphAscendingProperty, value);
        }

        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphDescendingProperty, value);
        }

        private sealed class SortGlyphAdorner : Adorner
        {
            private GridViewColumnHeader _columnHeader;
            private ListSortDirection _direction;
            private ImageSource _sortGlyph;

            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph) : base(columnHeader)
            {
                this._columnHeader = columnHeader;
                this._direction = direction;
                this._sortGlyph = sortGlyph;
            }

            private Geometry GetDefaultGlyph()
            {
                double x = this._columnHeader.ActualWidth - 13.0;
                double num2 = x + 10.0;
                double num3 = x + 5.0;
                double y = (this._columnHeader.ActualHeight / 2.0) - 3.0;
                double num5 = y + 5.0;
                if (this._direction == ListSortDirection.Ascending)
                {
                    double num6 = y;
                    y = num5;
                    num5 = num6;
                }
                PathSegmentCollection segments = new PathSegmentCollection();
                segments.Add(new LineSegment(new Point(num2, y), true));
                segments.Add(new LineSegment(new Point(num3, num5), true));
                PathFigure figure = new PathFigure(new Point(x, y), segments, true);
                return new PathGeometry(new PathFigureCollection { figure });
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                if (this._sortGlyph != null)
                {
                    double x = this._columnHeader.ActualWidth - 13.0;
                    double y = (this._columnHeader.ActualHeight / 2.0) - 3.0;
                    Rect rectangle = new Rect(x, y, 9.0, 6.0);
                    drawingContext.DrawImage(this._sortGlyph, rectangle);
                }
                else
                {
                    drawingContext.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Gray, 1.0), this.GetDefaultGlyph());
                }
            }
        }
    }
}

