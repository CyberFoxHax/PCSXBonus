using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using PCSX2Bonus.Properties;
using Extensions = PCSX2Bonus.Legacy.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndSetup {
		internal System.Windows.Controls.Button btnBrowse;
		internal System.Windows.Controls.Button btnBrowseData;
		internal System.Windows.Controls.Button btnOk;
		private bool _setupCompleted;
		internal System.Windows.Controls.TextBox tbPcsx2DataDir;
		internal System.Windows.Controls.TextBox tbPcsx2Dir;

		public wndSetup() {
			InitializeComponent();
			Loaded += wndSetup_Loaded;
		}

		private void btnBrowse_Click(object sender, RoutedEventArgs e) {
			var dialog = new FolderBrowserDialog {
				Description = "Select the directory containing PCSX2"
			};
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			var flag = false;
			var str = string.Empty;
			foreach (var str2 in Directory.GetFiles(dialog.SelectedPath, "*.exe").Where(str2 => (Extensions.Contains(str2, "pcsx2-r", StringComparison.InvariantCultureIgnoreCase) && !Extensions.Contains(str2, "Uninst", StringComparison.InvariantCultureIgnoreCase)) || Path.GetFileName(str2).Equals("pcsx2.exe", StringComparison.InvariantCultureIgnoreCase))) {
				flag = true;
				str = str2;
				break;
			}
			if (!flag)
				Legacy.Tools.ShowMessage("PCSX2 executable could not be found!", Legacy.MessageType.Error);
			else {
				Settings.Default.pcsx2Exe = str;
				tbPcsx2Dir.Text = dialog.SelectedPath;
			}
		}

		private void btnBrowseData_Click(object sender, RoutedEventArgs e) {
			var dialog = new FolderBrowserDialog {
				Description = "Select the directory containing the PCSX2 data folders (bios, inis, logs, memcards, snaps, sstates)"
			};
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

			var first = new[] { "inis", "bios", "logs", "memcards", "snaps", "sstates" };
			var second = (from d in Directory.GetDirectories(dialog.SelectedPath) select new DirectoryInfo(d).Name).ToArray();
			if (first.Except(second).Any()) {
				Legacy.Tools.ShowMessage("A required folder has not been found!", Legacy.MessageType.Error);
			}
			else {
				tbPcsx2DataDir.Text = dialog.SelectedPath;
			}
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (Extensions.IsEmpty(tbPcsx2DataDir.Text) || Extensions.IsEmpty(tbPcsx2Dir.Text)) {
				Legacy.Tools.ShowMessage("Required fields cannot be empty!", Legacy.MessageType.Error);
			}
			else {
				Settings.Default.pcsx2Dir = tbPcsx2Dir.Text;
				Settings.Default.pcsx2DataDir = tbPcsx2DataDir.Text;
				_setupCompleted = true;
				Close();
			}
		}

		private void wndSetup_Closing(object sender, CancelEventArgs e){
			if (_setupCompleted) return;
			if (System.Windows.MessageBox.Show("Setup has not been completed, exit?", "PCSX2Bonus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				System.Windows.Application.Current.Shutdown();
			}
			else {
				e.Cancel = true;
			}
		}

		private void wndSetup_Loaded(object sender, RoutedEventArgs e) {
			Closing += wndSetup_Closing;
		}
	}
}

