using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Enumerable = System.Linq.Enumerable;

namespace PCSX2Bonus.Wpf {
	public sealed class GridViewSort {
		public static readonly DependencyProperty AutoSortProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty PropertyNameProperty;
		public static readonly DependencyProperty ShowSortGlyphProperty;
		private static readonly DependencyProperty SortedColumnHeaderProperty;
		public static readonly DependencyProperty SortGlyphAscendingProperty;
		public static readonly DependencyProperty SortGlyphDescendingProperty;

		static GridViewSort() {
			CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(GridViewSort), new UIPropertyMetadata(null, delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
				var control = o as ItemsControl;
				if ((control == null) || GetAutoSort(control)) return;
				if ((e.OldValue != null) && (e.NewValue == null)) {
					control.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
				}
				if ((e.OldValue == null) && (e.NewValue != null)) {
					control.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
				}
			}));
			AutoSortProperty = DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(false, delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
				var view = o as ListView;
				if ((view == null) || (GetCommand(view) != null)) return;
				var oldValue = (bool)e.OldValue;
				var newValue = (bool)e.NewValue;
				if (oldValue && !newValue) {
					view.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
				}
				if (!oldValue && newValue) {
					view.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
				}
			}));
			PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort), new UIPropertyMetadata(null));
			ShowSortGlyphProperty = DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(true));
			SortGlyphAscendingProperty = DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));
			SortGlyphDescendingProperty = DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));
			SortedColumnHeaderProperty = DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));
		}

		private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph) {
			AdornerLayer.GetAdornerLayer(columnHeader).Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
		}

		public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader) {
			var ascending = ListSortDirection.Ascending;
			if (view.SortDescriptions.Count > 0) {
				var description = view.SortDescriptions[0];
				if (description.PropertyName == propertyName) 
					ascending = description.Direction == ListSortDirection.Ascending?
						ListSortDirection.Descending:
						ListSortDirection.Ascending;
				view.SortDescriptions.Clear();
				var columnHeader = GetSortedColumnHeader(listView);
				if (columnHeader != null) {
					RemoveSortGlyph(columnHeader);
				}
			}
			if (string.IsNullOrEmpty(propertyName)) return;

			view.SortDescriptions.Add(new SortDescription(propertyName, @ascending));
			if (GetShowSortGlyph(listView)) 
				AddSortGlyph(sortedColumnHeader, @ascending, (@ascending == ListSortDirection.Ascending) ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
			SetSortedColumnHeader(listView, sortedColumnHeader);
		}

		private static void ColumnHeader_Click(object sender, RoutedEventArgs e) {
			var originalSource = e.OriginalSource as GridViewColumnHeader;

			if ((originalSource == null) || (originalSource.Column == null)) return;
			var propertyName = GetPropertyName(originalSource.Column);

			if (string.IsNullOrEmpty(propertyName)) return;
			var ancestor = GetAncestor<ListView>(originalSource);

			if (ancestor != null) {
				var command = GetCommand(ancestor);
				if (command != null) {
					if (command.CanExecute(propertyName)) {
						command.Execute(propertyName);
					}
				}
				else if (GetAutoSort(ancestor)) {
					ApplySort(ancestor.Items, propertyName, ancestor, originalSource);
				}
			}
			Console.WriteLine(propertyName);
		}

		public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject {
			var parent = VisualTreeHelper.GetParent(reference);
			while (parent is T == false)
				parent = VisualTreeHelper.GetParent(parent);
			return (T)parent;
		}

		public static bool GetAutoSort(DependencyObject obj) {
			return (bool)obj.GetValue(AutoSortProperty);
		}

		public static ICommand GetCommand(DependencyObject obj) {
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static string GetPropertyName(DependencyObject obj) {
			return (string)obj.GetValue(PropertyNameProperty);
		}

		public static bool GetShowSortGlyph(DependencyObject obj) {
			return (bool)obj.GetValue(ShowSortGlyphProperty);
		}

		private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj) {
			return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
		}

		public static ImageSource GetSortGlyphAscending(DependencyObject obj) {
			return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
		}

		public static ImageSource GetSortGlyphDescending(DependencyObject obj) {
			return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
		}

		private static void RemoveSortGlyph(GridViewColumnHeader columnHeader) {
			var adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
			if (adornerLayer == null) return;

			var adorners = adornerLayer.GetAdorners(columnHeader);
			if (adorners == null) return;

			foreach (var adorner in Enumerable.OfType<SortGlyphAdorner>(adorners))
				adornerLayer.Remove(adorner);
		}

		public static void SetAutoSort(DependencyObject obj, bool value) {
			obj.SetValue(AutoSortProperty, value);
		}

		public static void SetCommand(DependencyObject obj, ICommand value) {
			obj.SetValue(CommandProperty, value);
		}

		public static void SetPropertyName(DependencyObject obj, string value) {
			obj.SetValue(PropertyNameProperty, value);
		}

		public static void SetShowSortGlyph(DependencyObject obj, bool value) {
			obj.SetValue(ShowSortGlyphProperty, value);
		}

		private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value) {
			obj.SetValue(SortedColumnHeaderProperty, value);
		}

		public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value) {
			obj.SetValue(SortGlyphAscendingProperty, value);
		}

		public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value) {
			obj.SetValue(SortGlyphDescendingProperty, value);
		}

		private sealed class SortGlyphAdorner : Adorner {
			private readonly GridViewColumnHeader _columnHeader;
			private readonly ListSortDirection _direction;
			private readonly ImageSource _sortGlyph;

			public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
				: base(columnHeader) {
				_columnHeader = columnHeader;
				_direction = direction;
				_sortGlyph = sortGlyph;
			}

			private Geometry GetDefaultGlyph() {
				var x = _columnHeader.ActualWidth - 13.0;
				var num2 = x + 10.0;
				var num3 = x + 5.0;
				var y = (_columnHeader.ActualHeight / 2.0) - 3.0;
				var num5 = y + 5.0;
				if (_direction == ListSortDirection.Ascending) {
					var num6 = y;
					y = num5;
					num5 = num6;
				}
				var segments = new PathSegmentCollection{
					new LineSegment(new Point(num2, y), true),
					new LineSegment(new Point(num3, num5), true)
				};
				var figure = new PathFigure(new Point(x, y), segments, true);
				return new PathGeometry(new PathFigureCollection { figure });
			}

			protected override void OnRender(DrawingContext drawingContext) {
				base.OnRender(drawingContext);
				if (_sortGlyph != null) {
					var x = _columnHeader.ActualWidth - 13.0;
					var y = (_columnHeader.ActualHeight / 2.0) - 3.0;
					var rectangle = new Rect(x, y, 9.0, 6.0);
					drawingContext.DrawImage(_sortGlyph, rectangle);
				}
				else {
					drawingContext.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Gray, 1.0), GetDefaultGlyph());
				}
			}
		}
	}
}

