namespace PCSX2Bonus
{
    using System;
    using System.Xml.Linq;

    internal sealed class UserSettings
    {
        public static string BonusXml = Path.Combine(RootDir, "mygames.xml");
        public static string ConfigDir = Path.Combine(RootDir, "Configs");
        public static string ImageDir = Path.Combine(RootDir, "Images");
        public static string Pcsx2SS = Path.Combine(Settings.Default.pcsx2DataDir, "sstates");
        public static string RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PCSX2Bonus");
        public static string ScreensDir = Path.Combine(RootDir, "Screenshots");
        public static string ShadersDir = Path.Combine(RootDir, "Shaders");
        public static string ThemesDir = Path.Combine(RootDir, "Themes");
        public static XElement xGames;
    }
}

