using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using PCSX2Bonus.Properties;

namespace PCSX2Bonus.PCSX2Bonus {
	public sealed class wndSetup : Window, IComponentConnector {
		private bool _contentLoaded;
		internal System.Windows.Controls.Button btnBrowse;
		internal System.Windows.Controls.Button btnBrowseData;
		internal System.Windows.Controls.Button btnOk;
		private bool setupCompleted;
		internal System.Windows.Controls.TextBox tbPcsx2DataDir;
		internal System.Windows.Controls.TextBox tbPcsx2Dir;

		public wndSetup() {
			InitializeComponent();
			base.Loaded += wndSetup_Loaded;
		}

		private void btnBrowse_Click(object sender, RoutedEventArgs e) {
			var dialog = new FolderBrowserDialog {
				Description = "Select the directory containing PCSX2"
			};
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			var flag = false;
			var str = string.Empty;
			foreach (var str2 in Directory.GetFiles(dialog.SelectedPath, "*.exe").Where(str2 => (str2.Contains("pcsx2-r", StringComparison.InvariantCultureIgnoreCase) && !str2.Contains("Uninst", StringComparison.InvariantCultureIgnoreCase)) || Path.GetFileName(str2).Equals("pcsx2.exe", StringComparison.InvariantCultureIgnoreCase))) {
				flag = true;
				str = str2;
				break;
			}
			if (!flag)
				Tools.ShowMessage("PCSX2 executable could not be found!", MessageType.Error);
			else {
				Settings.Default.pcsx2Exe = str;
				tbPcsx2Dir.Text = dialog.SelectedPath;
			}
		}

		private void btnBrowseData_Click(object sender, RoutedEventArgs e) {
			var dialog = new FolderBrowserDialog {
				Description = "Select the directory containing the PCSX2 data folders (bios, inis, logs, memcards, snaps, sstates)"
			};
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				var first = new string[] { "inis", "bios", "logs", "memcards", "snaps", "sstates" };
				var second = (from d in Directory.GetDirectories(dialog.SelectedPath) select new DirectoryInfo(d).Name).ToArray<string>();
				if (first.Except<string>(second).Any<string>()) {
					Tools.ShowMessage("A required folder has not been found!", MessageType.Error);
				}
				else {
					tbPcsx2DataDir.Text = dialog.SelectedPath;
				}
			}
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (tbPcsx2DataDir.Text.IsEmpty() || tbPcsx2Dir.Text.IsEmpty()) {
				Tools.ShowMessage("Required fields cannot be empty!", MessageType.Error);
			}
			else {
				Settings.Default.pcsx2Dir = tbPcsx2Dir.Text;
				Settings.Default.pcsx2DataDir = tbPcsx2DataDir.Text;
				setupCompleted = true;
				base.Close();
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent() {
			if (!_contentLoaded) {
				_contentLoaded = true;
				var resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndsetup.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocator);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 1:
					tbPcsx2Dir = (System.Windows.Controls.TextBox)target;
					return;

				case 2:
					btnBrowse = (System.Windows.Controls.Button)target;
					btnBrowse.Click += btnBrowse_Click;
					return;

				case 3:
					tbPcsx2DataDir = (System.Windows.Controls.TextBox)target;
					return;

				case 4:
					btnBrowseData = (System.Windows.Controls.Button)target;
					btnBrowseData.Click += btnBrowseData_Click;
					return;

				case 5:
					btnOk = (System.Windows.Controls.Button)target;
					btnOk.Click += btnOk_Click;
					return;
			}
			_contentLoaded = true;
		}

		private void wndSetup_Closing(object sender, CancelEventArgs e) {
			if (!setupCompleted) {
				if (System.Windows.MessageBox.Show("Setup has not been completed, exit?", "PCSX2Bonus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
					System.Windows.Application.Current.Shutdown();
				}
				else {
					e.Cancel = true;
				}
			}
		}

		private void wndSetup_Loaded(object sender, RoutedEventArgs e) {
			base.Closing += wndSetup_Closing;
		}
	}
}

