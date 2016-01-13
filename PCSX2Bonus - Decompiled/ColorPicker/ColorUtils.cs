namespace ColorPicker
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Media;

    public static class ColorUtils
    {
        private static Color BuildColor(double red, double green, double blue, double m)
        {
            return Color.FromArgb(0xff, (byte) (((red + m) * 255.0) + 0.5), (byte) (((green + m) * 255.0) + 0.5), (byte) (((blue + m) * 255.0) + 0.5));
        }

        public static Color ConvertHsvToRgb(double hue, double saturation, double value)
        {
            double red = value * saturation;
            if (hue == 360.0)
            {
                hue = 0.0;
            }
            double num2 = hue / 60.0;
            double green = red * (1.0 - Math.Abs((double) ((num2 % 2.0) - 1.0)));
            double m = value - red;
            switch (((int) num2))
            {
                case 0:
                    return BuildColor(red, green, 0.0, m);

                case 1:
                    return BuildColor(green, red, 0.0, m);

                case 2:
                    return BuildColor(0.0, red, green, m);

                case 3:
                    return BuildColor(0.0, green, red, m);

                case 4:
                    return BuildColor(green, 0.0, red, m);
            }
            return BuildColor(red, 0.0, green, m);
        }

        public static void ConvertRgbToHsv(Color color, out double hue, out double saturation, out double value)
        {
            double num = ((double) color.R) / 255.0;
            double num2 = ((double) color.G) / 255.0;
            double num3 = ((double) color.B) / 255.0;
            double num4 = Math.Min(num, Math.Min(num2, num3));
            double num5 = Math.Max(num, Math.Max(num2, num3));
            value = num5;
            double num6 = num5 - num4;
            if (value == 0.0)
            {
                saturation = 0.0;
            }
            else
            {
                saturation = num6 / num5;
            }
            if (saturation == 0.0)
            {
                hue = 0.0;
            }
            else if (num == num5)
            {
                hue = (num2 - num3) / num6;
            }
            else if (num2 == num5)
            {
                hue = 2.0 + ((num3 - num) / num6);
            }
            else
            {
                hue = 4.0 + ((num - num2) / num6);
            }
            hue *= 60.0;
            if (hue < 0.0)
            {
                hue += 360.0;
            }
        }

        public static void FireSelectedColorChangedEvent(UIElement issuer, RoutedEvent routedEvent, Color oldColor, Color newColor)
        {
            RoutedPropertyChangedEventArgs<Color> e = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor) {
                RoutedEvent = routedEvent
            };
            issuer.RaiseEvent(e);
        }

        public static string[] GetColorNames()
        {
            PropertyInfo[] properties = typeof(Colors).GetProperties();
            new ColorConverter();
            List<string> list = new List<string>();
            foreach (PropertyInfo info in properties)
            {
                string name = info.Name;
                list.Add(name);
                Color color1 = (Color) ColorConverter.ConvertFromString(name);
            }
            return list.ToArray();
        }

        public static Color[] GetSpectrumColors(int colorCount)
        {
            Color[] colorArray = new Color[colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                double hue = (i * 360.0) / ((double) colorCount);
                colorArray[i] = ConvertHsvToRgb(hue, 1.0, 1.0);
            }
            return colorArray;
        }

        public static bool TestColorConversion()
        {
            for (int i = 0; i <= 0xffffff; i++)
            {
                double num5;
                double num6;
                double num7;
                byte r = (byte) (i & 0xff);
                byte g = (byte) ((i & 0xff00) >> 8);
                byte b = (byte) ((i & 0xff0000) >> 0x10);
                Color color = Color.FromRgb(r, g, b);
                ConvertRgbToHsv(color, out num5, out num6, out num7);
                Color color2 = ConvertHsvToRgb(num5, num6, num7);
                if (color != color2)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

