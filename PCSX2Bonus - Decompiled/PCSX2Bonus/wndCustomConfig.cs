namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public sealed class wndCustomConfig : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal Button btnApply;
        internal Button btnCancel;
        internal Button btnConfig;
        internal Button btnOk;
        internal CheckBox cbEnableCheats;
        internal CheckBox cbFullBoot;
        internal CheckBox cbNoGui;
        internal CheckBox cbNoHacks;
        internal CheckBox cbUseCd;
        private Game g;
        internal ListBox lbBios;
        private IniFile pcsx2_ini;
        private IniFile pcsx2_ui;
        private IniFile pcsx2_vm;
        internal TextBlock tbInfo;

        public wndCustomConfig()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndCustomConfig_Loaded);
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.SaveSettings();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;
            string str = Path.Combine(UserSettings.ConfigDir, this.g.FileSafeTitle);
            Process p = new Process {
                EnableRaisingEvents = true
            };
            p.StartInfo.FileName = Settings.Default.pcsx2Exe;
            p.StartInfo.WorkingDirectory = Settings.Default.pcsx2Dir;
            p.StartInfo.Arguments = string.Format(" --cfgpath={0}{1}{0}", "\"", str);
            p.Exited += delegate (object o, EventArgs x) {
                if (p != null)
                {
                    p.Dispose();
                }
                Application.Current.Dispatcher.Invoke(delegate {
                    Toaster.Instance.ShowToast("Emulator Settings Saved", 0xdac);
                    mainWindow.Show();
                    this.UpdateSettings();
                    this.ShowDialog();
                    this.Activate();
                });
            };
            mainWindow.Hide();
            base.Hide();
            p.Start();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.SaveSettings();
            base.Close();
        }

        private string GetValue(string text)
        {
            string str = string.Empty;
            foreach (string str2 in text.Split(new char[] { ' ' }))
            {
                if (str2.Contains("ROMconf"))
                {
                    string str3 = Tools.RemoveInvalidXMLChars(str2);
                    string s = str3.Remove(str3.IndexOf("-")).Remove(0, 1).Insert(4, "/").Insert(7, "/");
                    string str5 = string.Empty;
                    try
                    {
                        str5 = str3.Remove(0, str3.IndexOf("PS2")).Replace("PS2", "").Remove(4).Insert(2, ".");
                    }
                    catch
                    {
                        str5 = "1.00";
                    }
                    string str6 = string.Empty;
                    if (str3.Contains("EC"))
                    {
                        str6 = "Europe";
                    }
                    else if (str3.Contains("AC"))
                    {
                        str6 = "USA";
                    }
                    else if (str3.Contains("WC"))
                    {
                        str6 = "Japan";
                    }
                    else
                    {
                        str6 = "Japan";
                    }
                    if (str3.Contains("142424"))
                    {
                        str6 = "USA";
                    }
                    DateTime time = DateTime.Parse(s);
                    string str7 = string.Format("{0}/{1}/{2}", time.Day, time.Month, time.Year);
                    return string.Format("{2} v{0} ({1}) Console", str5, str7, str6);
                }
            }
            return str;
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndcustomconfig.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private async void LoadBios()
        {
            string currentBios = this.pcsx2_ui.Read("Filenames", "BIOS").Unescape();
            List<Bios> bios = new List<Bios>();
            await Task.Run(delegate {
                foreach (string str in Directory.GetFiles(Settings.Default.pcsx2DataDir + @"\bios"))
                {
                    using (StreamReader reader = new StreamReader(str))
                    {
                        string str2;
                        while ((str2 = reader.ReadLine()) != null)
                        {
                            if (str2.Contains("ROMconf"))
                            {
                                byte[] bytes = (from i in Encoding.UTF8.GetBytes(str2)
                                    where i != 0
                                    select i).ToArray<byte>();
                                string src = Encoding.UTF8.GetString(bytes);
                                string str4 = src.Between("OSDSYS", "@rom");
                                if (str4.IsEmpty())
                                {
                                    str4 = src.Between("OSDSYS", "@");
                                }
                                Bios item = new Bios {
                                    DisplayInfo = this.GetValue(str4),
                                    Tag = str,
                                    Location = str
                                };
                                bios.Add(item);
                            }
                        }
                    }
                }
            });
            bios.ForEach(b => this.lbBios.Items.Add(b));
            IEnumerator enumerator = ((IEnumerable) this.lbBios.Items).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    Bios bios = (Bios) current;
                    if (bios.Tag.ToString() == currentBios)
                    {
                        this.lbBios.SelectedItem = bios;
                        return;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void SaveSettings()
        {
            string str = this.cbNoGui.IsChecked.Value ? "true" : "false";
            string str2 = this.cbUseCd.IsChecked.Value ? "true" : "false";
            string str3 = this.cbNoHacks.IsChecked.Value ? "true" : "false";
            string str4 = this.cbFullBoot.IsChecked.Value ? "true" : "false";
            string str5 = this.cbEnableCheats.IsChecked.Value ? "enabled" : "disabled";
            this.pcsx2_ini.Write("Boot", "NoGUI", str);
            this.pcsx2_ini.Write("Boot", "UseCD", str2);
            this.pcsx2_ini.Write("Boot", "NoHacks", str3);
            this.pcsx2_ini.Write("Boot", "FullBoot", str4);
            this.pcsx2_ini.Write("Boot", "EnableCheats", str5);
            this.pcsx2_vm.Write("EmuCore", "EnableCheats", str5);
            if (this.lbBios.SelectedItem != null)
            {
                this.pcsx2_ui.Write("Filenames", "BIOS", ((Bios) this.lbBios.SelectedItem).Tag.ToString().Escape());
            }
        }

        private void Setup()
        {
            this.g = (Game) base.Tag;
            this.pcsx2_ini = new IniFile(Path.Combine(UserSettings.ConfigDir, this.g.FileSafeTitle) + @"\PCSX2Bonus.ini");
            this.pcsx2_vm = new IniFile(Path.Combine(UserSettings.ConfigDir, Path.Combine(this.g.FileSafeTitle, "PCSX2_vm.ini")));
            this.pcsx2_ui = new IniFile(Path.Combine(UserSettings.ConfigDir, Path.Combine(this.g.FileSafeTitle, "PCSX2_ui.ini")));
            base.Title = "Viewing configuration for " + this.g.Title;
            this.UpdateSettings();
            this.LoadBios();
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.cbNoGui = (CheckBox) target;
                    return;

                case 2:
                    this.cbUseCd = (CheckBox) target;
                    return;

                case 3:
                    this.cbNoHacks = (CheckBox) target;
                    return;

                case 4:
                    this.cbFullBoot = (CheckBox) target;
                    return;

                case 5:
                    this.cbEnableCheats = (CheckBox) target;
                    return;

                case 6:
                    this.lbBios = (ListBox) target;
                    return;

                case 7:
                    this.btnConfig = (Button) target;
                    this.btnConfig.Click += new RoutedEventHandler(this.btnConfig_Click);
                    return;

                case 8:
                    this.btnOk = (Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;

                case 9:
                    this.btnApply = (Button) target;
                    this.btnApply.Click += new RoutedEventHandler(this.btnApply_Click);
                    return;

                case 10:
                    this.btnCancel = (Button) target;
                    this.btnCancel.Click += new RoutedEventHandler(this.btnCancel_Click);
                    return;

                case 11:
                    this.tbInfo = (TextBlock) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void UpdateSettings()
        {
            string str = this.pcsx2_ini.Read("Boot", "NoGUI");
            string str2 = this.pcsx2_ini.Read("Boot", "UseCD");
            string str3 = this.pcsx2_ini.Read("Boot", "NoHacks");
            string str4 = this.pcsx2_ini.Read("Boot", "FullBoot");
            string str5 = this.pcsx2_vm.Read("EmuCore", "EnableCheats");
            this.cbNoGui.IsChecked = new bool?(str == "true");
            this.cbUseCd.IsChecked = new bool?(str2 == "true");
            this.cbNoHacks.IsChecked = new bool?(str3 == "true");
            this.cbFullBoot.IsChecked = new bool?(str4 == "true");
            this.cbEnableCheats.IsChecked = new bool?(str5 == "enabled");
        }

        private void wndCustomConfig_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }

    }
}

