namespace Wpf.Tile
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public sealed class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        private bool _canHScroll;
        private bool _canVScroll;
        private Size _extent = new Size(0.0, 0.0);
        private Point _offset;
        private ScrollViewer _owner;
        private TranslateTransform _trans = new TranslateTransform();
        private Size _viewport = new Size(0.0, 0.0);
        public static readonly DependencyProperty ChildHeightProperty = DependencyProperty.RegisterAttached("ChildHeight", typeof(double), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(int), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty TileProperty = DependencyProperty.RegisterAttached("Tile", typeof(bool), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public VirtualizingTilePanel()
        {
            base.RenderTransform = this._trans;
        }

        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            if (this.Tile)
            {
                int num = this.CalculateChildrenPerRow(finalSize);
                int num2 = itemIndex / num;
                int num3 = itemIndex % num;
                child.Arrange(new Rect(num3 * this.ChildHeight, num2 * this.ChildHeight, this.ChildHeight, this.ChildHeight));
            }
            else
            {
                double width = this.CalculateChildWidth(finalSize);
                int num5 = itemIndex / this.Columns;
                int num6 = itemIndex % this.Columns;
                child.Arrange(new Rect(num6 * width, num5 * this.ChildHeight, width, this.ChildHeight));
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
            this.UpdateScrollInfo(finalSize);
            for (int i = 0; i < base.Children.Count; i++)
            {
                UIElement child = base.Children[i];
                int itemIndex = itemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
                this.ArrangeChild(itemIndex, child, finalSize);
            }
            return finalSize;
        }

        private int CalculateChildrenPerRow(Size availableSize)
        {
            if (availableSize.Width == double.PositiveInfinity)
            {
                return base.Children.Count;
            }
            return Math.Max(1, (int) Math.Floor((double) (availableSize.Width / this.ChildHeight)));
        }

        private double CalculateChildWidth(Size availableSize)
        {
            return (availableSize.Width / ((double) this.Columns));
        }

        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            if (this.Tile)
            {
                int num = this.CalculateChildrenPerRow(availableSize);
                return new Size(num * this.ChildHeight, this.ChildHeight * Math.Ceiling((double) (((double) itemCount) / ((double) num))));
            }
            double num2 = this.CalculateChildWidth(availableSize);
            return new Size(this.Columns * num2, this.ChildHeight * Math.Ceiling((double) (((double) itemCount) / ((double) this.Columns))));
        }

        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection internalChildren = base.InternalChildren;
            IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
            for (int i = internalChildren.Count - 1; i >= 0; i--)
            {
                GeneratorPosition position = new GeneratorPosition(i, 0);
                int num2 = itemContainerGenerator.IndexFromGeneratorPosition(position);
                if ((num2 < minDesiredGenerated) || (num2 > maxDesiredGenerated))
                {
                    itemContainerGenerator.Remove(position, 1);
                    base.RemoveInternalChildRange(i, 1);
                }
            }
        }

        private Size GetChildSize(Size availableSize)
        {
            return new Size(this.Tile ? this.ChildHeight : this.CalculateChildWidth(availableSize), this.ChildHeight);
        }

        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            if (this.Tile)
            {
                int num = this.CalculateChildrenPerRow(this._extent);
                firstVisibleItemIndex = ((int) Math.Floor((double) (this._offset.Y / this.ChildHeight))) * num;
                lastVisibleItemIndex = (((int) Math.Ceiling((double) ((this._offset.Y + this._viewport.Height) / this.ChildHeight))) * num) - 1;
                ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
                int num2 = itemsOwner.HasItems ? itemsOwner.Items.Count : 0;
                if (lastVisibleItemIndex >= num2)
                {
                    lastVisibleItemIndex = num2 - 1;
                }
            }
            else
            {
                double num3 = this.CalculateChildWidth(this._extent);
                firstVisibleItemIndex = ((int) Math.Floor((double) (this._offset.Y / num3))) * this.Columns;
                lastVisibleItemIndex = (((int) Math.Ceiling((double) ((this._offset.Y + this._viewport.Height) / this.ChildHeight))) * this.Columns) - 1;
                ItemsControl control2 = ItemsControl.GetItemsOwner(this);
                int num4 = control2.HasItems ? control2.Items.Count : 0;
                if (lastVisibleItemIndex >= num4)
                {
                    lastVisibleItemIndex = num4 - 1;
                }
            }
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + 10.0);
        }

        public void LineLeft()
        {
            throw new InvalidOperationException();
        }

        public void LineRight()
        {
            throw new InvalidOperationException();
        }

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - 10.0);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int num;
            int num2;
            this.UpdateScrollInfo(availableSize);
            this.GetVisibleRange(out num, out num2);
            UIElementCollection internalChildren = base.InternalChildren;
            IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
            GeneratorPosition position = itemContainerGenerator.GeneratorPositionFromIndex(num);
            int index = (position.Offset == 0) ? position.Index : (position.Index + 1);
            using (itemContainerGenerator.StartAt(position, GeneratorDirection.Forward, true))
            {
                int num4 = num;
                while (num4 <= num2)
                {
                    bool flag;
                    UIElement child = itemContainerGenerator.GenerateNext(out flag) as UIElement;
                    if (flag)
                    {
                        if (index >= internalChildren.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            base.InsertInternalChild(index, child);
                        }
                        itemContainerGenerator.PrepareItemContainer(child);
                    }
                    child.Measure(this.GetChildSize(availableSize));
                    num4++;
                    index++;
                }
            }
            this.CleanUpItems(num, num2);
            return availableSize;
        }

        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + 100.0);
        }

        public void MouseWheelLeft()
        {
            throw new InvalidOperationException();
        }

        public void MouseWheelRight()
        {
            throw new InvalidOperationException();
        }

        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - 100.0);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    base.RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    return;
            }
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this._viewport.Height);
        }

        public void PageLeft()
        {
            throw new InvalidOperationException();
        }

        public void PageRight()
        {
            throw new InvalidOperationException();
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this._viewport.Height);
        }

        public void SetHorizontalOffset(double offset)
        {
            throw new InvalidOperationException();
        }

        public void SetVerticalOffset(double offset)
        {
            if ((offset < 0.0) || (this._viewport.Height >= this._extent.Height))
            {
                offset = 0.0;
            }
            else if ((offset + this._viewport.Height) >= this._extent.Height)
            {
                offset = this._extent.Height - this._viewport.Height;
            }
            this._offset.Y = offset;
            if (this._owner != null)
            {
                this._owner.InvalidateScrollInfo();
            }
            this._trans.Y = -offset;
            base.InvalidateMeasure();
        }

        private void UpdateScrollInfo(Size availableSize)
        {
            ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsOwner.HasItems ? itemsOwner.Items.Count : 0;
            Size size = this.CalculateExtent(availableSize, itemCount);
            if (size != this._extent)
            {
                this._extent = size;
                if (this._owner != null)
                {
                    this._owner.InvalidateScrollInfo();
                }
            }
            if (availableSize != this._viewport)
            {
                this._viewport = availableSize;
                if (this._owner != null)
                {
                    this._owner.InvalidateScrollInfo();
                }
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                return this._canHScroll;
            }
            set
            {
                this._canHScroll = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return this._canVScroll;
            }
            set
            {
                this._canVScroll = value;
            }
        }

        public double ChildHeight
        {
            get
            {
                return (double) base.GetValue(ChildHeightProperty);
            }
            set
            {
                base.SetValue(ChildHeightProperty, value);
            }
        }

        public int Columns
        {
            get
            {
                return (int) base.GetValue(ColumnsProperty);
            }
            set
            {
                base.SetValue(ColumnsProperty, value);
            }
        }

        public double ExtentHeight
        {
            get
            {
                return this._extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                return this._extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this._offset.X;
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                return this._owner;
            }
            set
            {
                this._owner = value;
            }
        }

        public bool Tile
        {
            get
            {
                return (bool) base.GetValue(TileProperty);
            }
            set
            {
                base.SetValue(TileProperty, value);
            }
        }

        public double VerticalOffset
        {
            get
            {
                return this._offset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return this._viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return this._viewport.Width;
            }
        }
    }
}

