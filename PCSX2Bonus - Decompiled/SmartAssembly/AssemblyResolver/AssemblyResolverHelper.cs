namespace SmartAssembly.AssemblyResolver
{
    using SmartAssembly.Zip;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class AssemblyResolverHelper
    {
        internal const string BindList = "{71461f04-2faa-4bb9-a0dd-28a79101b599}";
        private static Dictionary<string, Assembly> hashtable = new Dictionary<string, Assembly>();
        private const int MOVEFILE_DELAY_UNTIL_REBOOT = 4;

        internal static void Attach()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolverHelper.ResolveAssembly);
            }
            catch
            {
            }
        }

        [DllImport("kernel32")]
        private static extern bool MoveFileEx(string existingFileName, string newFileName, int flags);
        internal static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            AssemblyInfo info = new AssemblyInfo(e.Name);
            string assemblyFullName = info.GetAssemblyFullName(false);
            string str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(assemblyFullName));
            string[] strArray = "ezJiMjQwNDRmLTY0NDgtNDczMi1hMGQ4LTlhNGIzNzg0MWY0N30sIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49M2U1NjM1MDY5M2Y3MzU1ZQ==,[z]{5afba4e7-4cbd-48f4-ae87-5cb4a100aac0},ezJiMjQwNDRmLTY0NDgtNDczMi1hMGQ4LTlhNGIzNzg0MWY0N30=,[z]{5afba4e7-4cbd-48f4-ae87-5cb4a100aac0},SHRtbEFnaWxpdHlQYWNrLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWJkMzE5YjE5ZWFmM2I0M2E=,[z]{22a11d39-8946-4a47-958b-a3b1fb426f2d},SHRtbEFnaWxpdHlQYWNr,[z]{22a11d39-8946-4a47-958b-a3b1fb426f2d}".Split(new char[] { ',' });
            string key = string.Empty;
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < (strArray.Length - 1); i += 2)
            {
                if (strArray[i] == str2)
                {
                    key = strArray[i + 1];
                    break;
                }
            }
            if ((key.Length == 0) && (info.PublicKeyToken.Length == 0))
            {
                str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(info.Name));
                for (int j = 0; j < (strArray.Length - 1); j += 2)
                {
                    if (strArray[j] == str2)
                    {
                        key = strArray[j + 1];
                        break;
                    }
                }
            }
            if (key.Length > 0)
            {
                if (key[0] == '[')
                {
                    int index = key.IndexOf(']');
                    string str4 = key.Substring(1, index - 1);
                    flag = str4.IndexOf('z') >= 0;
                    flag2 = str4.IndexOf('t') >= 0;
                    key = key.Substring(index + 1);
                }
                lock (hashtable)
                {
                    if (hashtable.ContainsKey(key))
                    {
                        return hashtable[key];
                    }
                    Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(key);
                    if (manifestResourceStream != null)
                    {
                        int length = (int) manifestResourceStream.Length;
                        byte[] buffer = new byte[length];
                        manifestResourceStream.Read(buffer, 0, length);
                        if (flag)
                        {
                            buffer = SimpleZip.Unzip(buffer);
                        }
                        Assembly assembly = null;
                        if (!flag2)
                        {
                            try
                            {
                                assembly = Assembly.Load(buffer);
                            }
                            catch (FileLoadException)
                            {
                                flag2 = true;
                            }
                            catch (BadImageFormatException)
                            {
                                flag2 = true;
                            }
                        }
                        if (flag2)
                        {
                            try
                            {
                                string path = string.Format(@"{0}{1}\", Path.GetTempPath(), key);
                                Directory.CreateDirectory(path);
                                string str6 = path + info.Name + ".dll";
                                if (!File.Exists(str6))
                                {
                                    FileStream stream2 = File.OpenWrite(str6);
                                    stream2.Write(buffer, 0, buffer.Length);
                                    stream2.Close();
                                    MoveFileEx(str6, null, 4);
                                    MoveFileEx(path, null, 4);
                                }
                                assembly = Assembly.LoadFile(str6);
                            }
                            catch
                            {
                            }
                        }
                        hashtable[key] = assembly;
                        return assembly;
                    }
                }
            }
            return null;
        }

        internal static bool IsWebApplication
        {
            get
            {
                try
                {
                    switch (Process.GetCurrentProcess().MainModule.ModuleName.ToLower())
                    {
                        case "w3wp.exe":
                            return true;

                        case "aspnet_wp.exe":
                            return true;
                    }
                }
                catch
                {
                }
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AssemblyInfo
        {
            public string Name;
            public System.Version Version;
            public string Culture;
            public string PublicKeyToken;
            public string GetAssemblyFullName(bool includeVersion)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this.Name);
                if (includeVersion && (this.Version != null))
                {
                    builder.Append(", Version=");
                    builder.Append(this.Version);
                }
                builder.Append(", Culture=");
                builder.Append((this.Culture.Length == 0) ? "neutral" : this.Culture);
                builder.Append(", PublicKeyToken=");
                builder.Append((this.PublicKeyToken.Length == 0) ? "null" : this.PublicKeyToken);
                return builder.ToString();
            }

            public AssemblyInfo(string assemblyFullName)
            {
                this.Version = null;
                this.Culture = string.Empty;
                this.PublicKeyToken = string.Empty;
                this.Name = string.Empty;
                foreach (string str in assemblyFullName.Split(new char[] { ',' }))
                {
                    string str2 = str.Trim();
                    if (str2.StartsWith("Version="))
                    {
                        this.Version = new System.Version(str2.Substring(8));
                    }
                    else if (str2.StartsWith("Culture="))
                    {
                        this.Culture = str2.Substring(8);
                        if (this.Culture == "neutral")
                        {
                            this.Culture = string.Empty;
                        }
                    }
                    else if (str2.StartsWith("PublicKeyToken="))
                    {
                        this.PublicKeyToken = str2.Substring(15);
                        if (this.PublicKeyToken == "null")
                        {
                            this.PublicKeyToken = string.Empty;
                        }
                    }
                    else
                    {
                        this.Name = str2;
                    }
                }
            }
        }
    }
}

