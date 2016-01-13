namespace PCSX2Bonus
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class IniFile
    {
        public string path;

        public IniFile(string INIPath)
        {
            this.path = INIPath;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        public List<string> GetValues(string category)
        {
            byte[] lpszReturnBuffer = new byte[0x800];
            GetPrivateProfileSection(category, lpszReturnBuffer, 0x800, this.path);
            string[] strArray = Encoding.ASCII.GetString(lpszReturnBuffer).Trim(new char[1]).Split(new char[1]);
            List<string> list = new List<string>();
            foreach (string str in strArray)
            {
                list.Add(str.Substring(str.IndexOf("=")));
            }
            return list;
        }

        public string Read(string Section, string Key)
        {
            StringBuilder retVal = new StringBuilder(0xff);
            GetPrivateProfileString(Section, Key, "", retVal, 0xff, this.path);
            return retVal.ToString();
        }

        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    }
}

