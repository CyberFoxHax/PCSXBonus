namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System.IO;
    using System.Xml.Linq;

    internal sealed class Variables
    {
        public static void GenerateTheme()
        {
            if (!Directory.Exists(UserSettings.ThemesDir))
            {
                Directory.CreateDirectory(UserSettings.ThemesDir);
            }
            if (!File.Exists(Path.Combine(UserSettings.ThemesDir, "default.xml")))
            {
                XElement.Parse(Resources.defaultTheme).Save(Path.Combine(UserSettings.ThemesDir, "default.xml"));
            }
        }
    }
}

