namespace PCSX2Bonus
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public sealed class DefaultStyles
    {
        public static SolidColorBrush ButtonBackgroundBrush = CreateBrush("#373737");
        public static SolidColorBrush ButtonBorderBrush = CreateBrush("Transparent");
        public static Thickness ButtonBorderThickness = new Thickness(0.0);
        public static SolidColorBrush ButtonDisabledBrush = CreateBrush("#343434");
        public static SolidColorBrush ButtonFontBrush = CreateBrush("White");
        public static SolidColorBrush ButtonFontDisabledBrush = CreateBrush("DimGray");
        public static FontFamily ButtonFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush ButtonFontMouseOverBrush = CreateBrush("White");
        public static double ButtonFontSize = 12.0;
        public static FontWeight ButtonFontWeight = FontWeights.Normal;
        public static SolidColorBrush ButtonMouseOverBrush = CreateBrush("#525252");
        public static SolidColorBrush ButtonPressedBrush = CreateBrush("#0072c6");
        public static SolidColorBrush CheckBoxBackgroundBrush = CreateBrush("#FF3B3B3B");
        public static SolidColorBrush CheckBoxBorderBrush = CreateBrush("Transparent");
        public static Thickness CheckBoxBorderThickness = new Thickness(0.0);
        public static SolidColorBrush CheckBoxFontBrush = CreateBrush("Gray");
        public static FontFamily CheckBoxFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush CheckBoxFontMouseOverBrush = CreateBrush("Gray");
        public static double CheckBoxFontSize = 12.0;
        public static FontWeight CheckBoxFontWeight = FontWeights.Normal;
        public static SolidColorBrush CheckBoxMouseOverBrush = CreateBrush("#4f4f4f");
        public static SolidColorBrush ComboBoxBackgroundBrush = CreateBrush("#FF2B2B2B");
        public static SolidColorBrush ComboBoxBorderBrush = CreateBrush("#FF444444");
        public static Thickness ComboBoxBorderThickness = new Thickness(1.0);
        public static SolidColorBrush ComboBoxFontBrush = CreateBrush("White");
        public static FontFamily ComboBoxFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush ComboBoxFontMouseOverBrush = CreateBrush("White");
        public static double ComboBoxFontSize = 12.0;
        public static FontWeight ComboBoxFontWeight = FontWeights.Normal;
        public static SolidColorBrush ComboBoxMouseOverBrush = CreateBrush("#5e5e5e");
        public static SolidColorBrush ComboBoxSubBackgroundBrush = CreateBrush("#FF2B2B2B");
        public static SolidColorBrush ComboBoxSubFontBrush = CreateBrush("White");
        public static FontFamily ComboBoxSubFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush ComboBoxSubFontMouseOverBrush = CreateBrush("White");
        public static double ComboBoxSubFontSize = 12.0;
        public static FontWeight ComboBoxSubFontWeight = FontWeights.Normal;
        public static SolidColorBrush ComboBoxSubMouseOverBrush = CreateBrush("#FF444444");
        public static SolidColorBrush GridViewHeaderBackgroundBrush = CreateBrush("#FF202020");
        public static SolidColorBrush GridViewHeaderBorderBrush = CreateBrush("#FF202020");
        public static Thickness GridViewHeaderBorderThickness = new Thickness(0.0);
        public static SolidColorBrush GridViewHeaderFontBrush = CreateBrush("White");
        public static FontFamily GridViewHeaderFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush GridViewHeaderFontMouseOverBrush = CreateBrush("#dcdcdc");
        public static double GridViewHeaderFontSize = 12.0;
        public static FontWeight GridViewHeaderFontWeight = FontWeights.Normal;
        public static SolidColorBrush ListViewAlternateRowBrush = CreateBrush("Transparent");
        public static SolidColorBrush ListViewBackgroundBrush = CreateBrush("Transparent");
        public static SolidColorBrush ListViewFontBrush = CreateBrush("Gray");
        public static FontFamily ListViewFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush ListViewFontMouseOverBrush = CreateBrush("White");
        public static SolidColorBrush ListViewFontSelectionBrush = CreateBrush("White");
        public static double ListViewFontSize = 12.0;
        public static FontWeight ListViewFontWeight = FontWeights.Normal;
        public static double ListViewImageHeight = 75.0;
        public static Size ListViewImageSize = new Size(ListViewImageWidth, ListViewImageHeight);
        public static double ListViewImageWidth = 55.0;
        public static SolidColorBrush ListViewSelectionBrush = CreateBrush("#444444");
        public static SolidColorBrush MenuFontBrush = CreateBrush("Gray");
        public static FontFamily MenuFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush MenuFontMouseOverBrush = CreateBrush("White");
        public static double MenuFontSize = 12.0;
        public static FontWeight MenuFontWeight = FontWeights.Normal;
        public static SolidColorBrush MenuSubBackgroundBrush = CreateBrush("#202020");
        public static SolidColorBrush MenuSubFontBrush = CreateBrush("Gray");
        public static FontFamily MenuSubFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush MenuSubFontMouseOverBrush = CreateBrush("White");
        public static double MenuSubFontSize = 12.0;
        public static FontWeight MenuSubFontWeight = FontWeights.Normal;
        public static SolidColorBrush MenuSubMouseOverBrush = CreateBrush("#2b2b2b");
        public static SolidColorBrush TabBackgroundBrush = CreateBrush("#FF2B2B2B");
        public static Thickness TabBorderThickness = new Thickness(0.0);
        public static SolidColorBrush TabFontBrush = CreateBrush("Gray");
        public static FontFamily TabFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush TabFontMouseOverBrush = CreateBrush("White");
        public static double TabFontSize = 12.0;
        public static FontWeight TabFontWeight = FontWeights.Normal;
        public static SolidColorBrush TabMouseOverBrush = CreateBrush("#5e5e5e");
        public static SolidColorBrush TabSubBackgroundBrush = CreateBrush("#202020");
        public static SolidColorBrush TabSubFontBrush = CreateBrush("White");
        public static FontFamily TabSubFontFamily = CreateFontFamily("Segoe UI");
        public static SolidColorBrush TabSubFontMouseOverBrush = CreateBrush("White");
        public static double TabSubFontSize = 12.0;
        public static FontWeight TabSubFontWeight = FontWeights.Normal;
        public static SolidColorBrush TabSubSelectionBrush = CreateBrush("#FF2B2B2B");
        public static SolidColorBrush TextBoxBackgroundBrush = CreateBrush("#6e6e6e");
        public static SolidColorBrush TextBoxBorderBrush = CreateBrush("Transparent");
        public static Thickness TextBoxBorderThickness = new Thickness(0.0);
        public static SolidColorBrush TextBoxFontBrush = CreateBrush("Black");
        public static FontFamily TextBoxFontFamily = CreateFontFamily("Segoe UI");
        public static double TextBoxFontSize = 12.0;
        public static FontWeight TextBoxFontWeight = FontWeights.Normal;
        public static double TileViewImageHeight = 128.0;
        public static double TileViewImageWidth = 90.0;
        public static SolidColorBrush WindowBackgroundBrush = CreateBrush("#2b2b2b");

        private static SolidColorBrush CreateBrush(string color)
        {
            return new SolidColorBrush((Color) ColorConverter.ConvertFromString(color));
        }

        private static FontFamily CreateFontFamily(string name)
        {
            return (FontFamily) new FontFamilyConverter().ConvertFromString(name);
        }
    }
}

