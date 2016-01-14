using System;
using System.IO;
using System.Xml.Linq;
using PCSX2Bonus.Properties;

namespace PCSX2Bonus.Legacy {
	internal sealed class UserSettings {
		public static string RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PCSX2Bonus");
		public static string ImageDir = Path.Combine(RootDir, "Images");
		public static string BonusXml = Path.Combine(RootDir, "mygames.xml");
		public static string Pcsx2SS = Path.Combine(Settings.Default.pcsx2DataDir, "sstates");
		public static string ConfigDir = Path.Combine(RootDir, "Configs");
		public static string ScreensDir = Path.Combine(RootDir, "Screenshots");
		public static string ShadersDir = Path.Combine(RootDir, "Shaders");
		public static string ThemesDir = Path.Combine(RootDir, "Themes");
		public static XElement xGames;
	}
}
