using System.Linq;

namespace PCSX2Bonus {
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Text;

	public sealed class IniFile {
		public string path;

		public IniFile(string iniPath) {
			path = iniPath;
		}

		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
		public List<string> GetValues(string category) {
			var lpszReturnBuffer = new byte[0x800];
			GetPrivateProfileSection(category, lpszReturnBuffer, 0x800, path);
			var strArray = Encoding.ASCII.GetString(lpszReturnBuffer).Trim(new char[1]).Split(new char[1]);
			return strArray.Select(str => str.Substring(str.IndexOf("="))).ToList();
		}

		public string Read(string section, string key) {
			var retVal = new StringBuilder(0xff);
			GetPrivateProfileString(section, key, "", retVal, 0xff, path);
			return retVal.ToString();
		}

		public void Write(string section, string key, string value) {
			WritePrivateProfileString(section, key, value, path);
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
	}
}

