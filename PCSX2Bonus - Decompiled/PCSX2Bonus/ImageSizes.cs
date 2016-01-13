namespace PCSX2Bonus
{
    using System;
    using System.Windows;

    internal sealed class ImageSizes
    {
        public static double ChildHeight = (TileItemHeight + 10.0);
        public static Size EditableSize = new Size(55.0, 75.0);
        public static Size GridSize = new Size(55.0, 75.0);
        public static Size SmallSize = new Size(43.0, 61.0);
        public static double TileItemHeight = TileItemSize.Height;
        public static Size TileItemSize = new Size(UserStyles.TileViewImageWidth + 22.0, UserStyles.TileViewImageHeight + 35.0);
        public static double TileItemWidth = TileItemSize.Width;
        public static Size TileSize = new Size(90.0, 128.0);
        public static Size TVSize = new Size(305.0, 420.0);
    }
}

