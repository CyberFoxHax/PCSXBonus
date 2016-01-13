using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PCSX2Bonus.Wpf {
	public sealed class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo {
		private Size _extent = new Size(0.0, 0.0);
		private Point _offset;
		private ScrollViewer _owner;
		private readonly TranslateTransform _trans = new TranslateTransform();
		private Size _viewport = new Size(0.0, 0.0);
		public static readonly DependencyProperty ChildHeightProperty = DependencyProperty.RegisterAttached("ChildHeight", typeof(double), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(int), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty TileProperty = DependencyProperty.RegisterAttached("Tile", typeof(bool), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public VirtualizingTilePanel() {
			RenderTransform = _trans;
		}

		private void ArrangeChild(int itemIndex, UIElement child, Size finalSize) {
			if (Tile) {
				var num = CalculateChildrenPerRow(finalSize);
				var num2 = itemIndex / num;
				var num3 = itemIndex % num;
				child.Arrange(new Rect(num3 * ChildHeight, num2 * ChildHeight, ChildHeight, ChildHeight));
			}
			else {
				var width = CalculateChildWidth(finalSize);
				var num5 = itemIndex / Columns;
				var num6 = itemIndex % Columns;
				child.Arrange(new Rect(num6 * width, num5 * ChildHeight, width, ChildHeight));
			}
		}

		protected override Size ArrangeOverride(Size finalSize) {
			var itemContainerGenerator = ItemContainerGenerator;
			UpdateScrollInfo(finalSize);
			for (var i = 0; i < Children.Count; i++) {
				var child = Children[i];
				var itemIndex = itemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
				ArrangeChild(itemIndex, child, finalSize);
			}
			return finalSize;
		}

		private int CalculateChildrenPerRow(Size availableSize){
			return double.IsPositiveInfinity(availableSize.Width) ?
				Children.Count:
				Math.Max(1, (int)Math.Floor(availableSize.Width / ChildHeight));
		}

		private double CalculateChildWidth(Size availableSize) {
			return availableSize.Width / Columns;
		}

		private Size CalculateExtent(Size availableSize, int itemCount) {
			if (Tile) {
				var num = CalculateChildrenPerRow(availableSize);
				return new Size(num * ChildHeight, ChildHeight * Math.Ceiling(itemCount / ((double)num)));
			}
			var num2 = CalculateChildWidth(availableSize);
			return new Size(Columns * num2, ChildHeight * Math.Ceiling(itemCount / ((double)Columns)));
		}

		private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated) {
			var internalChildren = InternalChildren;
			var itemContainerGenerator = ItemContainerGenerator;
			for (var i = internalChildren.Count - 1; i >= 0; i--) {
				var position = new GeneratorPosition(i, 0);
				var num2 = itemContainerGenerator.IndexFromGeneratorPosition(position);
				if ((num2 >= minDesiredGenerated) && (num2 <= maxDesiredGenerated)) continue;
				itemContainerGenerator.Remove(position, 1);
				RemoveInternalChildRange(i, 1);
			}
		}

		private Size GetChildSize(Size availableSize) {
			return new Size(Tile ? ChildHeight : CalculateChildWidth(availableSize), ChildHeight);
		}

		private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex) {
			if (Tile) {
				var num = CalculateChildrenPerRow(_extent);
				firstVisibleItemIndex = ((int)Math.Floor(_offset.Y / ChildHeight)) * num;
				lastVisibleItemIndex = (((int)Math.Ceiling((_offset.Y + _viewport.Height) / ChildHeight)) * num) - 1;
				var itemsOwner = ItemsControl.GetItemsOwner(this);
				var num2 = itemsOwner.HasItems ? itemsOwner.Items.Count : 0;
				if (lastVisibleItemIndex >= num2) {
					lastVisibleItemIndex = num2 - 1;
				}
			}
			else {
				var num3 = CalculateChildWidth(_extent);
				firstVisibleItemIndex = ((int)Math.Floor(_offset.Y / num3)) * Columns;
				lastVisibleItemIndex = (((int)Math.Ceiling((_offset.Y + _viewport.Height) / ChildHeight)) * Columns) - 1;
				var control2 = ItemsControl.GetItemsOwner(this);
				var num4 = control2.HasItems ? control2.Items.Count : 0;
				if (lastVisibleItemIndex >= num4) {
					lastVisibleItemIndex = num4 - 1;
				}
			}
		}

		public void LineDown() {
			SetVerticalOffset(VerticalOffset + 10.0);
		}

		public void LineLeft() {
			throw new InvalidOperationException();
		}

		public void LineRight() {
			throw new InvalidOperationException();
		}

		public void LineUp() {
			SetVerticalOffset(VerticalOffset - 10.0);
		}

		public Rect MakeVisible(Visual visual, Rect rectangle) {
			return new Rect();
		}

		protected override Size MeasureOverride(Size availableSize) {
			int num;
			int num2;
			UpdateScrollInfo(availableSize);
			GetVisibleRange(out num, out num2);
			var internalChildren = InternalChildren;
			var itemContainerGenerator = ItemContainerGenerator;
			var position = itemContainerGenerator.GeneratorPositionFromIndex(num);
			var index = (position.Offset == 0) ? position.Index : (position.Index + 1);
			using (itemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true)) {
				var num4 = num;
				while (num4 <= num2) {
					bool flag;
					var child = itemContainerGenerator.GenerateNext(out flag) as UIElement;
					if (flag){
						if (child != null){
							if (index >= internalChildren.Count)
								AddInternalChild(child);
							else
								InsertInternalChild(index, child);
						}
						itemContainerGenerator.PrepareItemContainer(child);
					}
					if (child != null)
						child.Measure(GetChildSize(availableSize));
					num4++;
					index++;
				}
			}
			CleanUpItems(num, num2);
			return availableSize;
		}

		public void MouseWheelDown() {
			SetVerticalOffset(VerticalOffset + 100.0);
		}

		public void MouseWheelLeft() {
			throw new InvalidOperationException();
		}

		public void MouseWheelRight() {
			throw new InvalidOperationException();
		}

		public void MouseWheelUp() {
			SetVerticalOffset(VerticalOffset - 100.0);
		}

		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args) {
			switch (args.Action) {
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					return;
			}
		}

		public void PageDown() {
			SetVerticalOffset(VerticalOffset + _viewport.Height);
		}

		public void PageLeft() {
			throw new InvalidOperationException();
		}

		public void PageRight() {
			throw new InvalidOperationException();
		}

		public void PageUp() {
			SetVerticalOffset(VerticalOffset - _viewport.Height);
		}

		public void SetHorizontalOffset(double offset) {
			throw new InvalidOperationException();
		}

		public void SetVerticalOffset(double offset) {
			if ((offset < 0.0) || (_viewport.Height >= _extent.Height)) {
				offset = 0.0;
			}
			else if ((offset + _viewport.Height) >= _extent.Height) {
				offset = _extent.Height - _viewport.Height;
			}
			_offset.Y = offset;
			if (_owner != null) {
				_owner.InvalidateScrollInfo();
			}
			_trans.Y = -offset;
			InvalidateMeasure();
		}

		private void UpdateScrollInfo(Size availableSize) {
			var itemsOwner = ItemsControl.GetItemsOwner(this);
			var itemCount = itemsOwner.HasItems ? itemsOwner.Items.Count : 0;
			var size = CalculateExtent(availableSize, itemCount);
			if (size != _extent) {
				_extent = size;
				if (_owner != null) {
					_owner.InvalidateScrollInfo();
				}
			}
			if (availableSize != _viewport) {
				_viewport = availableSize;
				if (_owner != null) {
					_owner.InvalidateScrollInfo();
				}
			}
		}

		public bool CanHorizontallyScroll { get; set; }

		public bool CanVerticallyScroll { get; set; }

		public double ChildHeight {
			get {
				return (double)GetValue(ChildHeightProperty);
			}
			set {
				SetValue(ChildHeightProperty, value);
			}
		}

		public int Columns {
			get {
				return (int)GetValue(ColumnsProperty);
			}
			set {
				SetValue(ColumnsProperty, value);
			}
		}

		public double ExtentHeight {
			get {
				return _extent.Height;
			}
		}

		public double ExtentWidth {
			get {
				return _extent.Width;
			}
		}

		public double HorizontalOffset {
			get {
				return _offset.X;
			}
		}

		public ScrollViewer ScrollOwner {
			get {
				return _owner;
			}
			set {
				_owner = value;
			}
		}

		public bool Tile {
			get {
				return (bool)GetValue(TileProperty);
			}
			set {
				SetValue(TileProperty, value);
			}
		}

		public double VerticalOffset {
			get {
				return _offset.Y;
			}
		}

		public double ViewportHeight {
			get {
				return _viewport.Height;
			}
		}

		public double ViewportWidth {
			get {
				return _viewport.Width;
			}
		}
	}
}

