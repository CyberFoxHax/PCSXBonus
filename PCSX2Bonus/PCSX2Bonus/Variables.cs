using System.IO;
using System.Xml.Linq;
using PCSX2Bonus.Properties;

namespace PCSX2Bonus.PCSX2Bonus
{
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

