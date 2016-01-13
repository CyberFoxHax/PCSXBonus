namespace ColorPicker
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Media;

    public static class ColorNames
    {
        private static Dictionary<Color, string> m_colorNames = new Dictionary<Color, string>();

        static ColorNames()
        {
            FillColorNames();
        }

        public static void FillColorNames()
        {
            Type type = typeof(Colors);
            foreach (PropertyInfo info in type.GetProperties())
            {
                string name = info.Name;
                Color key = (Color) info.GetValue(null, null);
                switch (name)
                {
                    case "Aqua":
                        key.R = (byte) (key.R + 1);
                        break;

                    case "Fuchsia":
                        key.G = (byte) (key.G + 1);
                        break;
                }
                m_colorNames.Add(key, name);
            }
        }

        public static string GetColorName(Color colorToSeek)
        {
            if (m_colorNames.ContainsKey(colorToSeek))
            {
                return m_colorNames[colorToSeek];
            }
            return colorToSeek.ToString();
        }
    }
}

