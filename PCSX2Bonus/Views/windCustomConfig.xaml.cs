using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PCSX2Bonus.Properties;
using Extensions = PCSX2Bonus.Legacy.Extensions;

namespace PCSX2Bonus.Views {
	public sealed class wndCustomConfig : Window, IComponentConnector {
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
		private Legacy.Game g;
		internal ListBox lbBios;
		private Legacy.IniFile pcsx2_ini;
		private Legacy.IniFile pcsx2_ui;
		private Legacy.IniFile pcsx2_vm;
		internal TextBlock tbInfo;

		public wndCustomConfig() {
			InitializeComponent();
			base.Owner = Application.Current.MainWindow;
			base.Loaded += wndCustomConfig_Loaded;
		}

		private void btnApply_Click(object sender, RoutedEventArgs e) {
			SaveSettings();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			base.Close();
		}

		private void btnConfig_Click(object sender, RoutedEventArgs e) {
			var mainWindow = Application.Current.MainWindow;
			var str = Path.Combine(Legacy.UserSettings.ConfigDir, g.FileSafeTitle);
			var p = new Process {
				EnableRaisingEvents = true
			};
			p.StartInfo.FileName = Settings.Default.pcsx2Exe;
			p.StartInfo.WorkingDirectory = Settings.Default.pcsx2Dir;
			p.StartInfo.Arguments = string.Format(" --cfgpath={0}{1}{0}", "\"", str);
			p.Exited += delegate(object o, EventArgs x) {
				if (p != null) {
					p.Dispose();
				}
				Application.Current.Dispatcher.Invoke(delegate {
					Legacy.Toaster.Instance.ShowToast("Emulator Settings Saved", 0xdac);
					mainWindow.Show();
					UpdateSettings();
					ShowDialog();
					Activate();
				});
			};
			mainWindow.Hide();
			base.Hide();
			p.Start();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			SaveSettings();
			base.Close();
		}

		private string GetValue(string text) {
			var str = string.Empty;
			foreach (var str2 in text.Split(new char[] { ' ' })) {
				if (str2.Contains("ROMconf")) {
					var str3 = Legacy.Tools.RemoveInvalidXMLChars(str2);
					var s = str3.Remove(str3.IndexOf("-")).Remove(0, 1).Insert(4, "/").Insert(7, "/");
					var str5 = string.Empty;
					try {
						str5 = str3.Remove(0, str3.IndexOf("PS2")).Replace("PS2", "").Remove(4).Insert(2, ".");
					}
					catch {
						str5 = "1.00";
					}
					var str6 = string.Empty;
					if (str3.Contains("EC")) {
						str6 = "Europe";
					}
					else if (str3.Contains("AC")) {
						str6 = "USA";
					}
					else if (str3.Contains("WC")) {
						str6 = "Japan";
					}
					else {
						str6 = "Japan";
					}
					if (str3.Contains("142424")) {
						str6 = "USA";
					}
					var time = DateTime.Parse(s);
					var str7 = string.Format("{0}/{1}/{2}", time.Day, time.Month, time.Year);
					return string.Format("{2} v{0} ({1}) Console", str5, str7, str6);
				}
			}
			return str;
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent() {
			if (!_contentLoaded) {
				_contentLoaded = true;
				var resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndcustomconfig.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		private async void LoadBios() {
			var currentBios = Extensions.Unescape(pcsx2_ui.Read("Filenames", "BIOS"));
			var bioses = new List<Legacy.Bios>();
			await Task.Run(delegate {
				foreach (var str in Directory.GetFiles(Settings.Default.pcsx2DataDir + @"\bios")) {
					using (var reader = new StreamReader(str)) {
						string str2;
						while ((str2 = reader.ReadLine()) != null) {
							if (str2.Contains("ROMconf")) {
								var bytes = (from i in Encoding.UTF8.GetBytes(str2)
											 where i != 0
											 select i).ToArray<byte>();
								var src = Encoding.UTF8.GetString(bytes);
								var str4 = Extensions.Between(src, (string) "OSDSYS", (string) "@rom");
								if (Extensions.IsEmpty(str4)) {
									str4 = Extensions.Between(src, (string) "OSDSYS", (string) "@");
								}
								var item = new Legacy.Bios {
									DisplayInfo = GetValue(str4),
									Tag = str,
									Location = str
								};
								bioses.Add(item);
							}
						}
					}
				}
			});
			bioses.ForEach(b => lbBios.Items.Add(b));
			var enumerator = ((IEnumerable)lbBios.Items).GetEnumerator();
			try {
				while (enumerator.MoveNext()) {
					var current = enumerator.Current;
					var bios = (Legacy.Bios)current;
					if (bios.Tag.ToString() != currentBios) continue;
					lbBios.SelectedItem = bioses;
					return;
				}
			}
			finally {
				var disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
		}

		private void SaveSettings() {
			var str = cbNoGui.IsChecked.Value ? "true" : "false";
			var str2 = cbUseCd.IsChecked.Value ? "true" : "false";
			var str3 = cbNoHacks.IsChecked.Value ? "true" : "false";
			var str4 = cbFullBoot.IsChecked.Value ? "true" : "false";
			var str5 = cbEnableCheats.IsChecked.Value ? "enabled" : "disabled";
			pcsx2_ini.Write("Boot", "NoGUI", str);
			pcsx2_ini.Write("Boot", "UseCD", str2);
			pcsx2_ini.Write("Boot", "NoHacks", str3);
			pcsx2_ini.Write("Boot", "FullBoot", str4);
			pcsx2_ini.Write("Boot", "EnableCheats", str5);
			pcsx2_vm.Write("EmuCore", "EnableCheats", str5);
			if (lbBios.SelectedItem != null) {
				pcsx2_ui.Write("Filenames", "BIOS", Extensions.Escape(((Legacy.Bios)lbBios.SelectedItem).Tag.ToString()));
			}
		}

		private void Setup() {
			g = (Legacy.Game)base.Tag;
			pcsx2_ini = new Legacy.IniFile(Path.Combine(Legacy.UserSettings.ConfigDir, g.FileSafeTitle) + @"\PCSX2Bonus.ini");
			pcsx2_vm = new Legacy.IniFile(Path.Combine(Legacy.UserSettings.ConfigDir, Path.Combine(g.FileSafeTitle, "PCSX2_vm.ini")));
			pcsx2_ui = new Legacy.IniFile(Path.Combine(Legacy.UserSettings.ConfigDir, Path.Combine(g.FileSafeTitle, "PCSX2_ui.ini")));
			base.Title = "Viewing configuration for " + g.Title;
			UpdateSettings();
			LoadBios();
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 1:
					cbNoGui = (CheckBox)target;
					return;

				case 2:
					cbUseCd = (CheckBox)target;
					return;

				case 3:
					cbNoHacks = (CheckBox)target;
					return;

				case 4:
					cbFullBoot = (CheckBox)target;
					return;

				case 5:
					cbEnableCheats = (CheckBox)target;
					return;

				case 6:
					lbBios = (ListBox)target;
					return;

				case 7:
					btnConfig = (Button)target;
					btnConfig.Click += btnConfig_Click;
					return;

				case 8:
					btnOk = (Button)target;
					btnOk.Click += btnOk_Click;
					return;

				case 9:
					btnApply = (Button)target;
					btnApply.Click += btnApply_Click;
					return;

				case 10:
					btnCancel = (Button)target;
					btnCancel.Click += btnCancel_Click;
					return;

				case 11:
					tbInfo = (TextBlock)target;
					return;
			}
			_contentLoaded = true;
		}

		private void UpdateSettings() {
			var str = pcsx2_ini.Read("Boot", "NoGUI");
			var str2 = pcsx2_ini.Read("Boot", "UseCD");
			var str3 = pcsx2_ini.Read("Boot", "NoHacks");
			var str4 = pcsx2_ini.Read("Boot", "FullBoot");
			var str5 = pcsx2_vm.Read("EmuCore", "EnableCheats");
			cbNoGui.IsChecked = str == "true";
			cbUseCd.IsChecked = str2 == "true";
			cbNoHacks.IsChecked = str3 == "true";
			cbFullBoot.IsChecked = str4 == "true";
			cbEnableCheats.IsChecked = str5 == "enabled";
		}

		private void wndCustomConfig_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}

	}
}
