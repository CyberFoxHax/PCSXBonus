using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.CSharp;
using PCSX2Bonus.Properties;
using Extensions = PCSX2Bonus.Legacy.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndGenerateExecutable {
		internal System.Windows.Controls.Button btnBrowseIcon;
		internal System.Windows.Controls.Button btnBrowseOutPath;
		internal System.Windows.Controls.Button btnCancel;
		internal System.Windows.Controls.Button btnOk;
		internal System.Windows.Controls.CheckBox cbFullBoot;
		internal System.Windows.Controls.CheckBox cbNoDisc;
		internal System.Windows.Controls.CheckBox cbNoGui;
		internal System.Windows.Controls.CheckBox cbNoHacks;
		internal System.Windows.Controls.CheckBox cbUseDefault;
		internal System.Windows.Controls.CheckBox cbUseDefaultIcon;
		private Legacy.Game g;
		internal System.Windows.Controls.TextBox tbIconPath;
		internal System.Windows.Controls.TextBox tbOutputPath;

		public wndGenerateExecutable() {
			InitializeComponent();
			Owner = System.Windows.Application.Current.MainWindow;
			Loaded += wndGenerateExecutable_Loaded;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (Extensions.IsEmpty(tbOutputPath.Text))
				Legacy.Tools.ShowMessage("Output path cannot be empty!", Legacy.MessageType.Error);
			else {
				var path = string.Empty;
				try {
					var stopwatch = new Stopwatch();
					stopwatch.Start();
					var location = g.Location;
					var newValue = cbNoHacks.IsChecked == true ? "--nohacks" : string.Empty;
					var str4 = cbNoGui.IsChecked == true ? "--nogui" : string.Empty;
					var str5 = cbNoDisc.IsChecked == true ? "--nodisc" : string.Empty;
					var str6 = cbFullBoot.IsChecked == true ? "--fullboot" : string.Empty;
					var str7 = cbUseDefault.IsChecked == true
						? "--cfgpath=\"\"" + Legacy.UserSettings.ConfigDir + @"\" + g.FileSafeTitle + "\"\""
						: string.Empty;
					var outputName = tbOutputPath.Text + @"\" + g.FileSafeTitle + ".exe";
					var providerOptions = new Dictionary<string, string>{{"CompilerVersion", "v4.0"}};
					var provider = new CSharpCodeProvider(providerOptions);
					var options = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, outputName, true) {
						GenerateExecutable = true
					};
					if (cbUseDefaultIcon.IsChecked != null && cbUseDefaultIcon.IsChecked.Value) {
						path = Legacy.UserSettings.ImageDir + @"\" + g.FileSafeTitle + ".ico";
						CreateIcon(path);
						options.CompilerOptions = string.Format("/target:winexe /optimize /win32icon:{1}{0}{1}", path, "\"");
					}
					else if (!cbUseDefault.IsChecked == true) {
						path = tbIconPath.Text;
						options.CompilerOptions = string.Format("/target:winexe /optimize /win32icon:{1}{0}{1}", path, "\"");
					}
					if (string.IsNullOrWhiteSpace(path))
						options.CompilerOptions = "/target:winexe /optimize";
					options.ReferencedAssemblies.Add("System.dll");
					options.IncludeDebugInformation = false;
					var str9 = Settings.Default.pcsx2Exe;
					var file = new Legacy.IniFile(Legacy.UserSettings.ConfigDir + @"\" + g.Title + @"\Settings.ini");
					if (!string.IsNullOrWhiteSpace(file.Read("Additional Executables", "Default"))) {
						str9 = file.Read("Additional Executables", "Default");
					}
					var directoryName = Path.GetDirectoryName(str9);
					var str11 =
						Properties.Resources.executableTemplate.Replace("{1}", str9)
							.Replace("{2}", newValue)
							.Replace("{3}", str4)
							.Replace("{4}", str5)
							.Replace("{5}", str6)
							.Replace("{6}", location)
							.Replace("{7}", "\"\"")
							.Replace("{8}", str7)
							.Replace("{9}", directoryName);
					provider.CompileAssemblyFromSource(options, str11)
						.Errors.Cast<CompilerError>()
						.ToList()
						.ForEach(error => Console.WriteLine(error.ErrorText));
					stopwatch.Stop();
					System.Windows.MessageBox.Show(
						string.Concat("Successfully compiled the executable at ", outputName, "\n[", stopwatch.ElapsedMilliseconds, "ms]"),
						"PCSX2Bonus", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				}
				catch (Exception exception) {
					Legacy.Tools.ShowMessage("There was an error building the executable.\nReason: " + exception.Message,
						Legacy.MessageType.Error);
				}
				var name = Path.GetDirectoryName(path);
				if (name != null && File.Exists(path) && name.Contains("PCSX2Bonus"))
					File.Delete(path);
			}
		}

		private void CreateIcon(string path, bool useDefault = true) {
			var str = Legacy.UserSettings.ImageDir + @"\" + g.FileSafeTitle + ".ico";
			if (useDefault) {
				using (var stream = new FileStream(str, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
					Properties.Resources.icon.Save(stream);
					return;
				}
			}
			var icon = new Icon(path);
			using (var stream2 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				icon.Save(stream2);
				icon.Dispose();
			}
		}

		private void Setup() {
			cbUseDefault.IsEnabled = Directory.Exists(Legacy.UserSettings.ConfigDir + @"\" + g.Title);
			cbUseDefaultIcon.Checked += delegate {
				tbIconPath.IsEnabled = false;
				btnBrowseIcon.IsEnabled = false;
			};
			cbUseDefaultIcon.Unchecked += delegate {
				tbIconPath.IsEnabled = true;
				btnBrowseIcon.IsEnabled = true;
			};
			btnBrowseIcon.Click += delegate {
				var dialog = new Microsoft.Win32.OpenFileDialog {
					Filter = "Icon Files | *.ico"
				};
				if (dialog.ShowDialog() == true)
					tbIconPath.Text = dialog.FileName;
			};
			btnBrowseOutPath.Click += delegate {
				var dialog = new FolderBrowserDialog();
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					tbOutputPath.Text = dialog.SelectedPath;
			};
			btnCancel.Click += (o, e) => Close();
			btnOk.Click += btnOk_Click;
		}

		private void wndGenerateExecutable_Loaded(object sender, RoutedEventArgs e) {
			g = (Legacy.Game)Tag;
			Title = string.Format("Generate Executable [{0}]", g.Title);
			Setup();
		}
	}
}

